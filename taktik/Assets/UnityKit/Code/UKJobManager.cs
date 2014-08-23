using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UKJobManager : MonoBehaviour {
	public enum JobState {
		READY, RUNNING, PAUSED, FINISHED
	}
	
	public class Job {
		public JobState _state = JobState.READY;
		// contains the exception thrown during Run
		public System.Exception _exception = null;
		
		public bool ShouldTerminate = false;
		
		public JobState State {
			get {
				return _state;
			}
		}
		
		public void Pause()
		{
			if (_state == JobState.RUNNING) {
				OnPause();
				_state = JobState.PAUSED;
			}
		}
		
		public void Continue()
		{
			if (_state == JobState.PAUSED) {
				OnContinue();
				_state = JobState.RUNNING;
			}
		}
		
		public void Stop()
		{
			ShouldTerminate = true;
		}
		
		// prior first Run step
		public virtual void OnStart() {}
		// called if Run finishes or there was an exception
		public virtual void OnFinish() {}
		public virtual void OnPause() {}
		public virtual void OnContinue() {}
		// called if Run throws an exception
		public virtual void OnException(System.Exception e) {
            Debug.LogException(e);
        }
		// can throw exceptions, returning an IEnumerator in a yield instruction triggers a subcall (stackable)
		public virtual IEnumerator Run() { yield return null; }

		// uses Time.time
		protected IEnumerator WaitForSeconds(float timeInSec) {
			float startTime = Time.time;
			while (!ShouldTerminate && Time.time - startTime < timeInSec) {
				yield return null;
			}
		}

		// uses Time.realtimeSinceStartup
		protected IEnumerator WaitForSecondsRealtime(float timeInSec) {
			float startTime = Time.realtimeSinceStartup;
			while (!ShouldTerminate && Time.time - startTime < timeInSec) {
				yield return null;
			}
		}

		// waits until condition() gets true
		protected IEnumerator WaitForCondition(Func<bool> condition) {
			while (!ShouldTerminate && condition != null && condition() == false) {
				yield return null;
			}
		}
	}
	
	private static UKJobManager _instance;
	
	public static UKJobManager Instance {
		get {
			if (_instance == null) Setup();
			return _instance;
		}
	}
	
	private static void Setup() {
		var go = new GameObject("JobManager");
		GameObject.DontDestroyOnLoad(go);
		_instance = go.AddComponent<UKJobManager>();
		//_instance._go = go;
	}

	
	private class ActiveJob {
		public Job job;
		public Stack<IEnumerator> enumerators = new Stack<IEnumerator> ();
		public string groupName;
	}
		
	//private GameObject _go = null;
	private List<ActiveJob> _jobs = new List<ActiveJob>();
	private bool _shouldTerminate = false;
	private Dictionary<string, int> _concurrentlyRunningJobsPerGroupLimit = new Dictionary<string, int>();
	private Dictionary<string, int> _activeJobsPerGroup = new Dictionary<string, int>();
	
	void Start() {
		StartCoroutine(Scheduler());
	}
	
	void OnDestroy() {
		_shouldTerminate = true;
	}
	
	private int CountActiveInGroup(string groupName)
	{
		if (_activeJobsPerGroup.ContainsKey(groupName)) return _activeJobsPerGroup[groupName];
		else return 0;
	}
	
	private void IncActiveInGroupName(string groupName, int inc)
	{
		_activeJobsPerGroup[groupName] = CountActiveInGroup(groupName) + inc;
	}
	
	private IEnumerator Scheduler() 
	{
		Debug.Log("scheduler started");
		_shouldTerminate = false;
		
		List<ActiveJob> finishedJobs = new List<ActiveJob>();
			
		while (!_shouldTerminate)
		{
			foreach (var activeJob in _jobs)
			{
				var job = activeJob.job;
				
				int limit = GetLimit(activeJob.groupName);
				
				// start job?
				if (job.State == JobState.READY && (limit == 0 || CountActiveInGroup(activeJob.groupName) < limit))
				{
					job.OnStart();
					IncActiveInGroupName(activeJob.groupName, 1);
					job._state = JobState.RUNNING;
					try 
					{
						activeJob.enumerators.Push(job.Run());
					}
					catch (System.Exception e)
					{
						job._exception = e;
						IncActiveInGroupName(activeJob.groupName, -1);
						job.OnException(e);
						job.OnFinish();
						job._state = JobState.FINISHED;
						activeJob.enumerators.Clear ();
						finishedJobs.Add(activeJob);
					}
				}
				
				// running?
				if (job.State == JobState.RUNNING && activeJob.enumerators != null)
				{
					try 
					{
						Stack<IEnumerator> enums = activeJob.enumerators;
						if (enums.Count > 0) {
							if (enums.Peek().MoveNext() == false) 
							{
								if (enums.Count == 1) {
									// last entry in stack
									IncActiveInGroupName(activeJob.groupName, -1);
									job.OnFinish();
									job._state = JobState.FINISHED;
									enums.Clear();
									finishedJobs.Add(activeJob);
								} else {
									// pop this finished one and continue with the next
									enums.Pop();
								}
							} else {
								// current elements is a sub sequence so add it ontop of the stack
								var cur = enums.Peek().Current;
								if (cur != null && cur is IEnumerator) enums.Push(cur as IEnumerator);
							}
						}
					}
					catch (System.Exception e)
					{
						job._exception = e;
						job.OnException(e);
						IncActiveInGroupName(activeJob.groupName, -1);
						job.OnFinish();
						job._state = JobState.FINISHED;
						activeJob.enumerators.Clear ();
						finishedJobs.Add(activeJob);
					}
				}
				
				// paused but should stop?
				if (job.State == JobState.PAUSED && job.ShouldTerminate)
				{
					IncActiveInGroupName(activeJob.groupName, -1);
					job.OnFinish();
					job._state = JobState.FINISHED;
					activeJob.enumerators.Clear ();
					finishedJobs.Add(activeJob);
				}
			}
			
			// cleanup
			while (finishedJobs.Count > 0) 
			{
				var job = finishedJobs[0];
				_jobs.Remove(job);
				finishedJobs.RemoveAt(0);
			}
			
			yield return null;
		}
		
		Debug.Log("scheduler finished");
	}
	
	public static void Run(Job job)
	{
		Run(job, "default");
	}
	
	public static void Run(Job job, string groupName)
	{
		Instance._jobs.Add(new ActiveJob(){ job = job, groupName = groupName });
	}

	public static void PauseAll()
	{
		foreach (var activeJob in Instance._jobs) 
		{
			activeJob.job.Pause();
		}
	}
	
	public static void ContinueAll()
	{
		foreach (var activeJob in Instance._jobs) 
		{
			activeJob.job.Continue();
		}
	}
	
	public static void StopAll()
	{
		foreach (var activeJob in Instance._jobs) 
		{
			activeJob.job.Stop();
		}
	}
	
	public static IEnumerable<Job> EnumJobs()
	{
		foreach (var activeJob in Instance._jobs) 
		{
			if (activeJob.job != null) yield return activeJob.job;
		}
	}

	public static IEnumerable<Job> EnumJobs(string groupName)
	{
		foreach (var activeJob in Instance._jobs) 
		{
			if (activeJob.job != null && activeJob.groupName == groupName) yield return activeJob.job;
		}
	}
	
	public static IEnumerable<string> EnumGroups()
	{
		List<string> alreadySeen = new List<string>();
		
		foreach (var activeJob in Instance._jobs) 
		{
			if (activeJob.job != null) 
			{
				var n = activeJob.groupName;
				if (alreadySeen.Contains(n) == false)
				{
					yield return activeJob.groupName;
					alreadySeen.Add(n);
				}
			}
		}
	}
	
	// 0 is no limit
	public static int GetLimit(string groupName)
	{
		if (Instance._concurrentlyRunningJobsPerGroupLimit.ContainsKey(groupName)) 
			return Instance._concurrentlyRunningJobsPerGroupLimit[groupName];
		else return 0;
	}
	
	public static void SetLimit(string groupName, int limit)
	{
		if (limit < 0) limit = 0;
		Instance._concurrentlyRunningJobsPerGroupLimit[groupName] = limit;
	}
	
	public static void PauseGroup(string groupName)
	{
		foreach (var j in EnumJobs(groupName)) j.Pause();
	}
	
	public static void ContinueGroup(string groupName)
	{
		foreach (var j in EnumJobs(groupName)) j.Continue();
	}
	
	public static void StopGroup(string groupName)
	{
		foreach (var j in EnumJobs(groupName)) j.Stop();
	}
	
	public static int CountAll()
	{
		return Instance._jobs.Count;
	}
	
	public static int CountGroup(string groupName)
	{
		return EnumJobs(groupName).Count();
	}
}

