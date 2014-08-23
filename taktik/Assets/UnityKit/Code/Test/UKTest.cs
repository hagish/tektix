using UnityEngine;
using System.Collections;

public class UKTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// TestJobManager ();
		TestBetterQueue();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region UKBetterQueue

	void TestBetterQueue() {
		UKQueue<int> q = new UKQueue<int>();

		for (int i = 0; i < 20; ++i) q.Enqueue(i);
		Debug.Log(q.Count);
		Debug.Log(q.Peek());
		Debug.Log(q.Count);
		Debug.Log(q.Peek());
		Debug.Log(q.Count);

		Debug.Log(q.Dequeue());
		Debug.Log(q.Count);

		int x = 20;

		Debug.Log("LOOP");

		while (q.Count > 0) {
			q.Enqueue(x);
			++x;

			if (q.Count > 0) Debug.Log(q.Dequeue());
			if (q.Count > 0) Debug.Log(q.Dequeue());
		}

		Debug.Log("DONE");

        try
        {
	    	Debug.Log(q.Dequeue());
        }
        catch (System.InvalidOperationException)
        {
            // should throw an exception
        }
	}

	#endregion

	#region UKJobManager

	public class TestJob : UKJobManager.Job {
		public override void OnStart ()
		{
			Debug.Log ("job started");
		}

		public override void OnFinish ()
		{
			Debug.Log ("job finished");
		}

		public override IEnumerator Run ()
		{
			int runsLeft = 10;
			Debug.Log ("job run started");
			while (!ShouldTerminate && runsLeft >= 0)
			{
				--runsLeft;
				Debug.Log(string.Format("job state {0}", _state));
				yield return WaitForSeconds (1f);
			}
			float t = Time.time;
			yield return WaitForCondition (() => Time.time - t > 3f);
			Debug.Log ("job run finished");
		}
	}

	void TestJobManager ()
	{
		UKJobManager.Run (new TestJob ());
	}

	#endregion
}
