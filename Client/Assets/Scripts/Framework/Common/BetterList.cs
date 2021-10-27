//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System;

/// <summary>
/// This improved version of the System.Collections.Generic.List that doesn't release the buffer on Clear(),
/// resulting in better performance and less garbage collection.
/// PRO: BetterList performs faster than List when you Add and Remove items (although slower if you remove from the beginning).
/// CON: BetterList performs worse when sorting the list. If your operations involve sorting, use the standard List instead.
/// </summary>

public class BetterList<T>
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
	/// Insert an item at the specified index, pushing the entries back.
	/// </summary>

	public void Insert (int index, T item)
	{
		if (index > -1 && index < mList.Count) mList.Insert(index, item);
		else mList.Add(item);
	}

	/// <summary>
	/// Returns 'true' if the specified item is within the list.
	/// </summary>

	public bool Contains (T item) { return mList.Contains(item); }

	/// <summary>
	/// Return the index of the specified item.
	/// </summary>

	public int IndexOf (T item) { return mList.IndexOf(item); }

	/// <summary>
	/// Remove the specified item from the list. Note that RemoveAt() is faster and is advisable if you already know the index.
	/// </summary>

	public bool Remove (T item) { return mList.Remove(item); }

	/// <summary>
	/// Remove an item at the specified index.
	/// </summary>

	public void RemoveAt (int index) { mList.RemoveAt(index); }

	/// <summary>
	/// Remove an item from the end.
	/// </summary>

	public T Pop ()
	{
		if (buffer != null && size != 0)
		{
			T val = buffer[mList.Count - 1];
			mList.RemoveAt(mList.Count - 1);
			return val;
		}
		return default(T);
	}

	/// <summary>
	/// Mimic List's ToArray() functionality, except that in this case the list is resized to match the current size.
	/// </summary>

	public T[] ToArray () { return mList.ToArray(); }

	/// <summary>
	/// List.Sort equivalent.
	/// </summary>

	public void Sort (System.Comparison<T> comparer) { mList.Sort(comparer); }

#else

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

    [DebuggerHidden]
    [DebuggerStepThrough]
    public IEnumerator<T> GetEnumerator()
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

    [DebuggerHidden]
    public T this[int i]
    {
        get { return buffer[i]; }
        set { buffer[i] = value; }
    }

    /// <summary>
    /// Clear the array by resetting its size to zero. Note that the memory is not actually released.
    /// </summary>

    public void Clear() { size = 0; }

    /// <summary>
    /// Clear the array and release the used memory.
    /// </summary>

    public void Release() { size = 0; RecycleBuffer(buffer); buffer = null; }

    /// <summary>
    /// Add the specified item to the end of the list.
    /// </summary>

    public void Add(T item)
    {
        if (buffer == null || size == buffer.Length) AllocateMore();
        buffer[size++] = item;
    }

    public void AddRange(T[] collection, int rangeCount)
    {
        InsertRange(size, collection, rangeCount);
    }

    /// <summary>
    /// Insert an item at the specified index, pushing the entries back.
    /// </summary>

    public void Insert(int index, T item)
    {
        if (buffer == null || size == buffer.Length) AllocateMore();

        if (index > -1 && index < size)
        {
            Array.ConstrainedCopy(buffer, index, buffer, index + 1, size - index);
            buffer[index] = item;
            ++size;
        }
        else Add(item);
    }

    public void InsertRange(int index, T[] collection, int rangeCount)
    {
        InsertRange(index, 0, collection, rangeCount);
    }

    public void InsertRange(int index, int collectionStartIndex, T[] collection, int rangeCount)
    {
        if (collection == null)
        {
            return;
        }

        if ((uint)index > (uint)size || (uint)collectionStartIndex > (uint)size)
        {
            throw new ArgumentOutOfRangeException();
        }

        rangeCount = Mathf.Min(collection.Length, rangeCount);
        if (rangeCount > 0)
        {
            if (buffer == null || buffer.Length < size + rangeCount)
            {
                AllocateMore(size + rangeCount);
            }

            if (index < size)
            {
                Array.ConstrainedCopy(buffer, index, buffer, index + rangeCount, size - index);
            }

            if (buffer == collection)
            {
                Array.ConstrainedCopy(buffer, collectionStartIndex, buffer, index, rangeCount);
            }
            else
            {
                Array.ConstrainedCopy(collection, collectionStartIndex, buffer, index, rangeCount);
            }
            size += rangeCount;
        }
    }

    /// <summary>
    /// Returns 'true' if the specified item is within the list.
    /// </summary>

    public bool Contains(T item)
    {
        if (buffer == null) return false;
        EqualityComparer<T> c = EqualityComparer<T>.Default;
        for (int i = 0; i < size; i++)
        {
            if (c.Equals(buffer[i], item)) return true;
        }
        return false;
    }

    /// <summary>
    /// Return the index of the specified item.
    /// </summary>

    public int IndexOf(T item)
    {
        return Array.IndexOf(buffer, item, 0, size);
    }

    /// <summary>
    /// Remove the specified item from the list. Note that RemoveAt() is faster and is advisable if you already know the index.
    /// </summary>

    public bool Remove(T item)
    {
        if (buffer != null)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }
        return false;
    }

    /// <summary>
    /// Remove an item at the specified index.
    /// </summary>

    public void RemoveAt(int index)
    {
        if (buffer != null && index > -1 && index < size)
        {
            --size;
            Array.ConstrainedCopy(buffer, index + 1, buffer, index, size - index);
            buffer[size] = default(T);
        }
    }

    /// <summary>
    /// Remove an item from the end.
    /// </summary>

    public T Pop()
    {
        if (buffer != null && size != 0)
        {
            T val = buffer[--size];
            buffer[size] = default(T);
            return val;
        }
        return default(T);
    }

    /// <summary>
    /// Mimic List's ToArray() functionality, except that in this case the list is resized to match the current size.
    /// </summary>

    public T[] ToArray() { Trim(); return buffer; }

    //class Comparer : System.Collections.IComparer
    //{
    //    public System.Comparison<T> func;
    //    public int Compare (object x, object y) { return func((T)x, (T)y); }
    //}

    //Comparer mComp = new Comparer();

    /// <summary>
    /// List.Sort equivalent. Doing Array.Sort causes GC allocations.
    /// </summary>

    //public void Sort (System.Comparison<T> comparer)
    //{
    //    if (size > 0)
    //    {
    //        mComp.func = comparer;
    //        System.Array.Sort(buffer, 0, size, mComp);
    //    }
    //}

    /// <summary>
    /// List.Sort equivalent. Manual sorting causes no GC allocations.
    /// </summary>

    [DebuggerHidden]
    [DebuggerStepThrough]
    public void Sort(CompareFunc comparer)
    {
        int start = 0;
        int max = size - 1;
        bool changed = true;

        while (changed)
        {
            changed = false;

            for (int i = start; i < max; ++i)
            {
                // Compare the two values
                if (comparer(buffer[i], buffer[i + 1]) > 0)
                {
                    // Swap the values
                    T temp = buffer[i];
                    buffer[i] = buffer[i + 1];
                    buffer[i + 1] = temp;
                    changed = true;
                }
                else if (!changed)
                {
                    // Nothing has changed -- we can start here next time
                    start = (i == 0) ? 0 : i - 1;
                }
            }
        }
    }

    /// <summary>
    /// Comparison function should return -1 if left is less than right, 1 if left is greater than right, and 0 if they match.
    /// </summary>

    public delegate int CompareFunc(T left, T right);

    //~BetterList()
    //{
    //    lock (SyncRoot)
    //    {
    //        RecycleBuffer(buffer);
    //    }
    //}

    /// <summary>
    /// Helper function that expands the size of the array, maintaining the content.
    /// </summary>

    void AllocateMore(int min = 1)
    {
        int newBufferLength = (buffer != null) ? Mathf.Max(buffer.Length << 1, 32) : 32;
        if (newBufferLength < min) newBufferLength = (Mathf.Max(32, min) / 32) * 32 << 1;
        BufferPoolNode fondBufferNode = GetCachedBufferPoolNode(newBufferLength, false);

        if (fondBufferNode != null && fondBufferNode.bufferPool.Count > 0)
        {
            LinkedListNode<T[]> firstNode = fondBufferNode.bufferPool.First;
            T[] curBuffer = firstNode.Value;
            if (buffer != null && size > 0)
            {
                Array.ConstrainedCopy(buffer, 0, curBuffer, 0, size);
            }

            buffer = curBuffer;
            fondBufferNode.bufferPool.RemoveFirst();
            firstNode.Value = null;
            _invalidNode.AddLast(firstNode);
            return;
        }

        T[] newList = new T[newBufferLength];
        if (buffer != null && size > 0)
        {
            Array.ConstrainedCopy(buffer, 0, newList, 0, size);
        }

        RecycleBuffer(buffer);
        buffer = newList;
    }

    /// Trim the unnecessary memory, resizing the buffer to be of 'Length' size.
    /// Call this function only if you are sure that the buffer won't need to resize anytime soon.
    /// </summary>

    void Trim()
    {
        if (size > 0)
        {
            if (size < buffer.Length)
            {
                T[] newList = new T[size];
                Array.ConstrainedCopy(buffer, 0, newList, 0, size);
                RecycleBuffer(buffer);
                buffer = newList;
            }
        }
        else
        {
            RecycleBuffer(buffer);
            buffer = null;
        }
    }

    void RecycleBuffer(T[] dirtyBuffer)
    {
        if (dirtyBuffer == null)
            return;

        BufferPoolNode fondBufferNode = GetCachedBufferPoolNode(dirtyBuffer.Length, true);
        AppendBufferNode(fondBufferNode.bufferPool, dirtyBuffer);
    }

    //object SyncRoot
    //{
    //    get
    //    {
    //        if (_syncRoot == null)
    //        {
    //            System.Threading.Interlocked.CompareExchange<System.Object>(ref _syncRoot, new System.Object(), null);
    //        }
    //        return _syncRoot;
    //    }
    //}

    BufferPoolNode GetCachedBufferPoolNode(int requiredLength, bool bStrictLength)
    {
        BufferPoolNode fondBufferNode = null;

        if (_bufferMap.Count > 0)
        {
            var itorNode = _bufferMap.First;

            while (itorNode != null)
            {
                bool bNodeAvailable = bStrictLength ? itorNode.Value.bufferLength == requiredLength : itorNode.Value.bufferLength >= requiredLength;
                if (bNodeAvailable)
                {
                    fondBufferNode = itorNode.Value;
                    break;
                }

                itorNode = itorNode.Next;
            }
        }

        if (fondBufferNode == null)
        {
            fondBufferNode = new BufferPoolNode();
            fondBufferNode.bufferLength = requiredLength;
            fondBufferNode.bufferPool = new LinkedList<T[]>();

            bool bInserted = false;
            var itorNode = _bufferMap.First;
            while (itorNode != null)
            {
                if (itorNode.Value.bufferLength > requiredLength)
                {
                    _bufferMap.AddBefore(itorNode, fondBufferNode);
                    bInserted = true;
                    break;
                }

                itorNode = itorNode.Next;
            }
            if (!bInserted) _bufferMap.AddLast(fondBufferNode);
        }

        return fondBufferNode;
    }

    void AppendBufferNode(LinkedList<T[]> bufferPool, T[] recyclingBuffer)
    {
        LinkedListNode<T[]> newNode = null;
        if (_invalidNode.Count > 0)
        {
            newNode = _invalidNode.Last;
            _invalidNode.RemoveLast();
            newNode.Value = recyclingBuffer;
        }

        if (newNode != null)
            bufferPool.AddLast(newNode);
        else
            bufferPool.AddLast(recyclingBuffer);
    }

    //static private object _syncRoot;
    static private LinkedList<T[]> _invalidNode = new LinkedList<T[]>();
    static private LinkedList<BufferPoolNode> _bufferMap = new LinkedList<BufferPoolNode>();

    class BufferPoolNode
    {
        public int bufferLength = 0;
        public LinkedList<T[]> bufferPool = new LinkedList<T[]>();
    }
#endif
}
