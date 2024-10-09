using System.Runtime.CompilerServices;

namespace Frent.Lab;

internal interface IFrentComponent {}
internal interface IComponent : IFrentComponent
{
    void Update();
}

internal class CStorage<T> : Storage<T>
    where T : struct, IComponent
{
    public override IComponentStorage Clone() => new CStorage<T>();
    protected override void Update(Span<T> values)
    {
        foreach(ref var comp in values)
            comp.Update();
    }
}

internal interface IComponent<TArg> : IFrentComponent
{
    void Update(ref TArg arg);
}

internal class CStorage<T, Ta> : Storage<T>
    where T : struct, IComponent<Ta>
    where Ta : struct
{
    public override IComponentStorage Clone() => new CStorage<T, Ta>();
    protected override void Update(Span<T> values)
    {
        Span<Ta> a = Other<Ta>();
        for(int i = 0; i < values.Length; i++)
            Update(ref values[i], ref a[i]);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Update(ref T c, ref Ta a) => c.Update(ref a);
    }
}

internal interface IComponent<TArg1, TArg2> : IFrentComponent
{
    void Update(ref TArg1 arg1, ref TArg2 arg2);
}

internal class CStorage<T, Ta1, Ta2> : Storage<T>
    where T : struct, IComponent<Ta1, Ta2>
    where Ta1 : struct
    where Ta2 : struct
{
    public override IComponentStorage Clone() => new CStorage<T, Ta1, Ta2>();
    protected override void Update(Span<T> values)
    {
        Span<Ta1> a = Other<Ta1>();
        Span<Ta2> b = Other<Ta2>();
        for(int i = 0; i < values.Length; i++)
            Update(ref values[i], ref a[i], ref b[i]);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Update(ref T c, ref Ta1 a, ref Ta2 b) => c.Update(ref a, ref b);
    }
}