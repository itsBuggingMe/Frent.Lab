using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Frent.Lab;

internal interface IFrentComponent;
internal interface IComponent : IFrentComponent
{
    void Update() { }

    internal class Creation : IComponentStorageFactory
    {
        public IComponentStorage Create<T>() where T : struct, IFrentComponent
            => new Storage<T>();
    }

    internal class Storage<TSelf> : StorageBase<TSelf>
        where TSelf : struct, IComponent
    {
        public override void Update(World world)
        {
            foreach(ref var item in _data.AsSpan())
                item.Update();
        }
    }
}

internal interface IComponent<T>
    where T : struct
{
    void Update(ref T a);

    internal class Storage<TSelf> : StorageBase<TSelf>
        where TSelf : struct, IComponent<T>
    {
        IComponentStorage _pstorage = null!;

        public override void Init(Archetype archetype)
        {
            _pstorage = archetype[typeof(T)];
        }

        public override void Update(World world)
        {
            Span<T> span = _pstorage.AsSpan<T>();
            Span<TSelf> self = _data.AsSpan();
            for(int i = 0; i < self.Length; i++)
            {
                self[i].Update(ref span[i]);
            }
        }
    }
}

internal interface IComponent<T1, T2>
    where T1 : struct
    where T2 : struct
{
    void Update(ref T1 a1, ref T2 a2);

    internal class Storage<TSelf> : StorageBase<TSelf>
        where TSelf : struct, IComponent<T1, T2>
    {
        
        IComponentStorage _pstorage1 = null!;
        IComponentStorage _pstorage2 = null!;
        public override void Init(Archetype archetype)
        {
            _pstorage1 = archetype[typeof(T1)];
            _pstorage2 = archetype[typeof(T2)];
        }

        public override void Update(World world)
        {
            Span<T1> span1 = _pstorage1.AsSpan<T1>();
            Span<T2> span2 = _pstorage2.AsSpan<T2>();
            Span<TSelf> self = _data.AsSpan();

            for (int i = 0; i < self.Length; i++)
            {
                self[i].Update(ref span1[i], ref span2[i]);
            }
        }
    }
}


internal abstract class StorageBase<TComponent> : IComponentStorage
    where TComponent : struct
{
    public abstract void Update(World world);

    protected FastStack<TComponent> _data = FastStack<TComponent>.Create(4);

    public Type Type => typeof(TComponent);
    
    public ref T Push<T>(out int index)
    {
        if (typeof(T) != typeof(TComponent))
        {
            throw new InvalidCastException();
        }
        index = _data.Count;

        return ref Unsafe.As<TComponent, T>(ref _data.PushRef());
    }

    public Span<T> AsSpan<T>()
        where T : struct
    {
        if (typeof(T) != typeof(TComponent))
        {
            throw new InvalidCastException();
        }

        var span = _data.AsSpan();
        return MemoryMarshal.Cast<TComponent, T>(span);
    }

    public virtual void Init(Archetype archetype) { }

    public void MoveTo(IComponentStorage other)
    {
        other.Push<TComponent>(out _) = _data[0];
    }

    //internal static Span<TTo> UnsafeCastCheck<TFrom, TTo, TInterface>()
    //    where TTo : TFrom
    //{
    //
    //}
}