using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This improved queue that doesn't release the buffer on Clear(), resulting in better performance and less garbage collection.
/// </summary>
public class UKQueue<T> {
	private const int CHUNK_SIZE = 8;

	private int nextDequeue = 0;
	private int nextEnqueue = 0;

	private T[] buffer;

	public int Count {
		get { return nextEnqueue - nextDequeue; }
	}

	public void Clear() {
		nextDequeue = 0;
		nextEnqueue = 0;

		// TODO clear buffer?
		//if (buffer == null) return;
		//for (int i = 0; i < buffer.Length; ++i) buffer[i] = default(T);
	}

	private void ResizeIfNecessary() {
		if (buffer == null) {
			buffer = new T[CHUNK_SIZE];
		} else {
			int spaceLeft = buffer.Length - Count;
			if (spaceLeft == 0) {
				T[] newBuffer = new T[buffer.Length + CHUNK_SIZE];

				for (int i = nextDequeue; i < nextEnqueue; ++i) {
					newBuffer[i - nextDequeue] = buffer[i % buffer.Length];
				}

				nextDequeue = 0;
				nextEnqueue = buffer.Length;

				buffer = newBuffer;
			}
		}
	}

	public void Enqueue(T t) {
		ResizeIfNecessary();
		buffer[nextEnqueue % buffer.Length] = t;
		++nextEnqueue;
	}

	public T Dequeue() {
		if (Count == 0 || buffer == null) throw new System.InvalidOperationException();
		++nextDequeue;
		return buffer[(nextDequeue - 1) % buffer.Length];
	} 

	public T Peek() {
		if (Count == 0 || buffer == null) throw new System.InvalidOperationException();
		return buffer[nextDequeue % buffer.Length];
	}

	public T Last() {
		if (Count == 0 || buffer == null) throw new System.InvalidOperationException();
		return this[Count - 1];
	}

	public IEnumerator<T> GetEnumerator ()
	{
		if (buffer != null)
		{
			for (int i = nextDequeue; i < nextEnqueue; ++i)
			{
				yield return buffer[i % buffer.Length];
			}
		}
	}

	public T this[int i]
	{
		get { return buffer[(nextDequeue + i) % buffer.Length]; }
		set { buffer[(nextDequeue + i) % buffer.Length] = value; }
	}
}
