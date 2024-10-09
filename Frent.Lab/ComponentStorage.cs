
using System.Runtime.InteropServices;

namespace Frent.Lab;

internal abstract class Storage<T> : IComponentStorage
    where T : struct
{
    private Archetype _type = null!;
    private T[] _arr = new T[64];
    public Span<T1> AsSpan<T1>()
        where T1 : struct
    {
        if(typeof(T1) == typeof(T))
        {
            return MemoryMarshal.Cast<T, T1>(_arr.AsSpan());
        }

        throw new InvalidOperationException();
    }
    public abstract IComponentStorage Clone();
    public void Init(Archetype archetype) => _type = archetype;
    protected abstract void Update(Span<T> values);
    protected Span<T1> Other<T1>() where T1 : struct => _type[typeof(T1)].AsSpan<T1>();
    public void Update() =>  Update(_arr.AsSpan());
}