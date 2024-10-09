namespace Frent.Lab;

internal interface IComponentStorage
{
    void Init(Archetype archetype);
    void Update();
    Span<T> AsSpan<T>() where T : struct;
    IComponentStorage Clone();
}