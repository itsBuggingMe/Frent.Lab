using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Frent.Lab;

public struct FastStack<T>(int initalComponents) : IEnumerable<T>
{
    public static FastStack<T> Create(int initalComponents) => new FastStack<T>(initalComponents);

    private T[] _buffer = new T[initalComponents > 0 ? initalComponents : throw new ArgumentException()];
    private int _nextIndex = 0;

    private static readonly bool NeedToWorryAboutGC = RuntimeHelpers.IsReferenceOrContainsReferences<T>();

    public readonly int Count => _nextIndex;
    public readonly T Top => _buffer[_nextIndex - 1];
    public readonly bool HasElements => _nextIndex > 0;
    public readonly ref T this[int i] => ref _buffer[i];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Push(in T item) => PushRef() = item;

    public ref T PushRef()
    {
        if ((uint)_nextIndex < _buffer.Length)
        {
            return ref _buffer[_nextIndex++];
        }
        else
        {
            Array.Resize(ref _buffer, _buffer.Length * 2);
            return ref _buffer[_nextIndex++];
        }
    }

    public void Compact() => Array.Resize(ref _buffer, _nextIndex);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Pop()
    {
        var next = _buffer[--_nextIndex];
        if (NeedToWorryAboutGC)
            _buffer[_nextIndex] = default!;
        return next;
    }

    public void RemoveAtReplace(int index)
    {
        Debug.Assert(Count > 0);
        _buffer[index] = _buffer[--_nextIndex];
        _buffer[_nextIndex] = default!;
    }

    public void Remove(T element)
    {
        var s = AsSpan();
        for (int i = 0; i < s.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(element, s[i]))
            {
                RemoveAtReplace(i);
                return;
            }
        }
    }

    /// <summary>
    /// DO NOT ALTER WHILE SPAN IS IN USE
    /// </summary>
    public readonly Span<T> AsSpan() => new(_buffer, 0, _nextIndex);

    public void Clear()
    {
        if (NeedToWorryAboutGC)
            AsSpan().Clear();
        _nextIndex = 0;
    }


    readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    //foreach uses this as from duck typing
    public readonly FastStackEnumerator GetEnumerator() => new(this);

    //Enumerator for convience
    //slightly slower than using span, and still not safe with modifying stack (needs to be class in order to do that)
    public struct FastStackEnumerator(FastStack<T> stack) : IEnumerator<T>
    {
        private T[] _elements = stack._buffer;
        private int _max = stack._nextIndex;
        private int _index = -1;
        public readonly T Current => _elements[_index];
        readonly object? IEnumerator.Current => _elements[_index];
        public void Dispose() => _elements = null!;
        public bool MoveNext() => ++_index < _max;
        public void Reset() => _index = -1;
    }
}
