
using System.Runtime.InteropServices;

namespace Frent.Lab;

internal abstract class Storage<T> : IComponentStorage<T>
    where T : struct
{
    private Archetype _type = null!;
    private T[] _arr = new T[64];
    private int _index = 0;
    public Span<T1> AsSpan<T1>()
        where T1 : struct
    {
        if(typeof(T1) == typeof(T))
            return MemoryMarshal.Cast<T, T1>(_arr.AsSpan(0, _index));

        throw new InvalidOperationException();
    }
    public abstract IComponentStorage Clone();
    public void Init(Archetype archetype) => _type = archetype;
    protected abstract void Update(Span<T> values);
    protected Span<T1> Other<T1>() where T1 : struct => _type[typeof(T1)].AsSpan<T1>();
    public void Update() =>  Update(_arr.AsSpan(0, _index));
    public void Push(in T t)
    {
        if(_index == _arr.Length)
        {
            Array.Resize(ref _arr, _index * 2);
        }

        _arr[_index++] = t;
    }
    public Span<T> AsSpan() => _arr.AsSpan(0, _index);
}