using UnityEngine;
using System;

public class UKMinMaxAvg
{
	public int ResetCount = 100;
	
	public float Min {
		get {
			return _min;
		}
	}
	
	public float Max {
		get {
			return _max;
		}
	}
	
	public float Avg {
		get {
			return _count > 0 ? _sum / _count : 0f;
		}
	}
	
	public float Last {
		get {
			return _last;
		}
	}
	
	public float _min;
	public float _max;
	public float _last;
	public int _count;
	public float _sum;
	
    public UKMinMaxAvg()
    {

    }

    public UKMinMaxAvg(int resetCount)
    {
        ResetCount = resetCount;
    }

	public void Add(float f)
	{
		if (_count > ResetCount) _count = 0;
		
		if (_count == 0)
		{
			_count = 1;
			_last = f;
			_min = f;
			_max = f;
			_sum = f;
		}
		else
		{
			++_count;
			_last = f;
			_min = Mathf.Min(_min, f);
			_max = Mathf.Max(_max, f);
			_sum += f;
		}
	}
	
	public override string ToString ()
	{
		return string.Format ("[MinMaxAvg: Min={0:0.0}, Max={1:0.0}, Avg={2:0.0}, Last={3:0.0}]", Min, Max, Avg, Last);
	}
}
