using System.Diagnostics.CodeAnalysis;

namespace Frent.Lab;

public class World
{
    private Dictionary<Type[], Archetype> _archetypes = new(TypeArrEqualityComparer.Inst);
    public World()
    {
        
    }

    public void Create<T>(in T? comp1 = default)
    {
        
    }
}

internal class TypeArrEqualityComparer : EqualityComparer<Type[]>
{
    public static readonly TypeArrEqualityComparer Inst = new();

    public override bool Equals(Type[]? x, Type[]? y)
    {
        if(x == y)
            return true;
        if(x is null || y is null)
            return false;
        
        return x.AsSpan().SequenceEqual(y.AsSpan());
    }

    public override int GetHashCode([DisallowNull] Type[] obj)
    {
        HashCode hashCode = new();
        foreach(var item in obj)
        {
            hashCode.Add(item);
        }
        return hashCode.ToHashCode();
    }
}