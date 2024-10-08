global using Archetype = System.Collections.Generic.Dictionary<System.Type, Frent.Lab.IComponentStorage>;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Frent.Lab;

internal class World
{
    internal Dictionary<int, Archetype> _archetypes = [];
    internal List<IComponentStorage> _tmp = [];

    public World With<T>(in T item)
        where T : struct, IComponent
    {
        var storage = new IComponent.Storage<T>();
        _tmp.Add(storage);
        storage.Push<T>(out int _) = item;
        return this;
    }

    public World With<T, TArg>(in T item)
        where T : struct, IComponent<TArg>
        where TArg : struct
    {
        var storage = new IComponent<TArg>.Storage<T>();
        _tmp.Add(storage);
        storage.Push<T>(out int _) = item;
        return this;
    }

    public World With<T, TArg1, TArg2>(in T item)
        where T : struct, IComponent<TArg1, TArg2>
        where TArg1 : struct
        where TArg2 : struct
    {
        var storage = new IComponent<TArg1, TArg2>.Storage<T>();
        _tmp.Add(storage);
        storage.Push<T>(out int _) = item;
        return this;
    }

    public void Complete()
    {
        HashCode hashCode = new();

        foreach (var item in _tmp)
        {
            hashCode.Add(item.Type);
        }
        int code = hashCode.ToHashCode();
        Archetype? dest;
        if (!_archetypes.TryGetValue(code, out dest))
        {
            dest = new Archetype();
            _archetypes.Add(code, dest);
        }

        foreach (var item in _tmp)
        {
            IComponentStorage from = item;
            IComponentStorage to = dest[item.Type];

            from.MoveTo(to);
        }
    }

    public void Update()
    {
        foreach (var item in _archetypes)
        {
            foreach (var kvp in item.Value)
            {
                kvp.Value.Update(this);
            }
        }
    }

    private static int Hash(ReadOnlySpan<Type> types)
    {
        HashCode hs = new HashCode();
        for (int i = 0; i < types.Length; i++)
        {
            hs.Add(types[i]);
        }
        return hs.ToHashCode();
    }

    private static TValue GetOrCreate<TKey, TValue>(Dictionary<TKey, TValue> kvp, TKey key, Func<TValue> factory)
    {
        if (!kvp.TryGetValue(key, out TValue? result))
            result = factory();
        return result;
    }
}

internal interface IComponentStorage
{
    void Init(Archetype archetype);
    Type Type { get; }
    Span<T> AsSpan<T>() where T : struct;
    ref T Push<T>(out int index);
    void MoveTo(IComponentStorage other);
    void Update(World world);
}


static class ComponentData<T>
{
    public static readonly bool IsComponent;
    public static Func<IComponentStorage> Factory = null!;

    public static IComponentStorage CreateStorage()
    {
        if (!IsComponent)
            throw new InvalidOperationException($"{typeof(T).Name} is not component");
        return Factory();
    }

    static ComponentData()
    {
        Type @this = typeof(T);
        IsComponent = typeof(IFrentComponent).IsAssignableFrom(@this);
    }
}

static class ComponentStorageCtors
{
    public static FrozenDictionary<Type, IComponentStorageFactory> _ctors;
    static ComponentStorageCtors()
    {
        _ctors = new Dictionary<Type, IComponentStorage>
        {
            { typeof(IComponent),  GetObj<IComponentStorage.>() }
        }.ToFrozenDictionary();
    }

    private static IComponentStorage GetObj<T>()
    {
        return (IComponentStorage)RuntimeHelpers.GetUninitializedObject(typeof(T));
    }
}

internal interface IComponentStorageFactory
{
    IComponentStorage Create<T>() where T : struct, IFrentComponent;
}