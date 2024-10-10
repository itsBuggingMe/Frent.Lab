namespace Frent.Lab;

internal interface IComponentStorage
{
    void Init(Archetype archetype);
    void Update();
    Span<T> AsSpan<T>() where T : struct;
    IComponentStorage Clone();
}

internal interface IComponentStorage<T> : IComponentStorage
    where T : struct
{
    void Push(in T t);
    Span<T> AsSpan();
}