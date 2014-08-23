using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This improved version of the System.Collections.Generic.List that doesn't release the buffer on Clear(), resulting in better performance and less garbage collection.
/// 
/// based on Futile & NGUI https://github.com/MattRix/Futile/issues/196
/// MattRix: So Michael says I can use BetterList and that it's coming in Unity anyway: https://twitter.com/ArenMook/status/414111431269683200
/// </summary>
public class UKList<T>
{
	#if UNITY_FLASH
	
	List<T> mList = new List<T>();
	
	/// <summary>
	/// Direct access to the buffer. Note that you should not use its 'Length' parameter, but instead use BetterList.size.
	/// </summary>
	
	public T this[int i]
	{
		get { return mList[i]; }
		set { mList[i] = value; }
	}
	
	/// <summary>
	/// Compatibility with the non-flash syntax.
	/// </summary>
	
	public List<T> buffer { get { return mList; } }
	
	/// <summary>
	/// Direct access to the buffer's size. Note that it's only public for speed and efficiency. You shouldn't modify it.
	/// </summary>
	
	public int size { get { return mList.Count; } }
	
	/// <summary>
	/// For 'foreach' functionality.
	/// </summary>
	
	public IEnumerator<T> GetEnumerator () { return mList.GetEnumerator(); }
	
	/// <summary>
	/// Clear the array by resetting its size to zero. Note that the memory is not actually released.
	/// </summary>
	
	public void Clear () { mList.Clear(); }
	
	/// <summary>
	/// Clear the array and release the used memory.
	/// </summary>
	
	public void Release () { mList.Clear(); }
	
	/// <summary>
	/// Add the specified item to the end of the list.
	/// </summary>
	
	public void Add (T item) { mList.Add(item); }
	
	/// <summary>
	/// Remove the specified item from the list. Note that RemoveAt() is faster and is advisable if you already know the index.
	/// </summary>
	
	public void Remove (T item) { mList.Remove(item); }
	
	/// <summary>
	/// Remove an item at the specified index.
	/// </summary>
	
	public void RemoveAt (int index) { mList.RemoveAt(index); }
	
	/// <summary>
	/// Mimic List's ToArray() functionality, except that in this case the list is resized to match the current size.
	/// </summary>
	
	public T[] ToArray () { return mList.ToArray(); }
	
	#else
	
	public int Count {
		get { return size; }
	}
	
	/// <summary>
	/// Direct access to the buffer. Note that you should not use its 'Length' parameter, but instead use BetterList.size.
	/// </summary>
	
	public T[] buffer;
	
	/// <summary>
	/// Direct access to the buffer's size. Note that it's only public for speed and efficiency. You shouldn't modify it.
	/// </summary>
	
	public int size = 0;
	
	/// <summary>
	/// For 'foreach' functionality.
	/// </summary>
	
	public IEnumerator<T> GetEnumerator ()
	{
		if (buffer != null)
		{
			for (int i = 0; i < size; ++i)
			{
				yield return buffer[i];
			}
		}
	}
	
	/// <summary>
	/// Convenience function. I recommend using .buffer instead.
	/// </summary>
	
	public T this[int i]
	{
		get { return buffer[i]; }
		set { buffer[i] = value; }
	}
	
	/// <summary>
	/// Helper function that expands the size of the array, maintaining the content.
	/// </summary>
	
	void AllocateMore ()
	{
		T[] newList = (buffer != null) ? new T[Mathf.Max(buffer.Length << 1, 32)] : new T[32];
		if (buffer != null && size > 0) buffer.CopyTo(newList, 0);
		buffer = newList;
	}
	
	/// <summary>
	/// Trim the unnecessary memory, resizing the buffer to be of 'Length' size.
	/// Call this function only if you are sure that the buffer won't need to resize anytime soon.
	/// </summary>
	
	void Trim ()
	{
		if (size > 0)
		{
			if (size < buffer.Length)
			{
				T[] newList = new T[size];
				for (int i = 0; i < size; ++i) newList[i] = buffer[i];
				buffer = newList;
			}
		}
		else buffer = null;
	}
	
	/// <summary>
	/// Clear the array by resetting its size to zero. Note that the memory is not actually released.
	/// </summary>
	
	public void Clear () { size = 0; }
	
	/// <summary>
	/// Clear the array and release the used memory.
	/// </summary>
	
	public void Release () { size = 0; buffer = null; }
	
	/// <summary>
	/// Add the specified item to the end of the list.
	/// </summary>
	
	public void Add (T item)
	{
		if (buffer == null || size == buffer.Length) AllocateMore();
		buffer[size++] = item;
	}
	
	/// <summary>
	/// Remove the specified item from the list. Note that RemoveAt() is faster and is advisable if you already know the index.
	/// </summary>
	
	public void Remove (T item)
	{
		if (buffer != null)
		{
			EqualityComparer<T> comp = EqualityComparer<T>.Default;
			
			for (int i = 0; i < size; ++i)
			{
				if (comp.Equals(buffer[i], item))
				{
					--size;
					buffer[i] = default(T);
					for (int b = i; b < size; ++b) buffer[b] = buffer[b + 1];
					return;
				}
			}
		}
	}


    public void Insert(int index, T item)
    {
        if (index < 0 || index > size)
        {
            throw new System.ArgumentOutOfRangeException();
        }
        else if (index == size)
        {
            Add(item);
        }
        else 
        {
            Add(default(T));
            for (int b = size - 1; b > index; b--) buffer[b] = buffer[b - 1];
            buffer[index] = item;
        }
    }
    
    /// <summary>
	/// Remove an item at the specified index.
	/// </summary>
	
	public void RemoveAt (int index)
	{
		if (buffer != null && index < size)
		{
			--size;
			buffer[index] = default(T);
			for (int b = index; b < size; ++b) buffer[b] = buffer[b + 1];
		}
	}

    /// <summary>
    /// Removes many items at the specified index.
    /// A call with count = 1 equals RemoveAt
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    public void RemoveManyAt (int index, int count)
    {
        if (buffer != null && index + count - 1 < size)
        {
            size -= count;
            for (int b = index; b < index + count; ++b ) buffer[b] = default(T);
            for (int b = index; b < size; ++b) buffer[b] = buffer[b + count];
        }
    }
	
	/// <summary>
	/// Mimic List's ToArray() functionality, except that in this case the list is resized to match the current size.
	/// </summary>
	
	public T[] ToArray () { Trim(); return buffer; }
	#endif

    public bool Contains(T item)
    {
        if (buffer != null)
        {
            EqualityComparer<T> comp = EqualityComparer<T>.Default;

            for (int i = 0; i < size; ++i)
            {
                if (comp.Equals(buffer[i], item))
                {
                    return true;
                }
            }
        }

        return false;
    }
}