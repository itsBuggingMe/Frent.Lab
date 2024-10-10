using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Frent.Lab;

public class World
{
    private Dictionary<int, Archetype> _archetypes = new();
    public World()
    {
        
    }

    public void Create<T>(in T comp1 = default)
        where T : struct
    {
        int hash = Hash([typeof(T)]);
        Archetype addTo;
        if (!_archetypes.TryGetValue(hash, out addTo))
            addTo = _archetypes[hash] = new Archetype();

        ((IComponentStorage<T>)addTo[typeof(T)]).Push(comp1);
    }

    public void Create<T1, T2>(in T1 comp1 = default, in T2 comp2 = default)
        where T1 : struct
        where T2 : struct
    {
        int hash = Hash([typeof(T1), typeof(T2)]);
        Archetype addTo = FindOrCreate(_archetypes, hash, () => new());

        var item1 = (IComponentStorage<T1>)FindOrCreate(addTo, typeof(T1), ContainerFactoryClass<T1>.Factory.Clone, out var new1);
        item1.Push(comp1);
        if (new1)
            item1.Init(addTo);
        var item2 = (IComponentStorage<T2>)FindOrCreate(addTo, typeof(T2), ContainerFactoryClass<T2>.Factory.Clone, out var new2);
        item2.Push(comp2);
        if (new2)
            item2.Init(addTo);
    }

    public void Update()
    {
        foreach(var element in _archetypes)
        {
            foreach(var comp in element.Value)
            {
                comp.Value.Update();
            }
        }
    }

    static TValue FindOrCreate<TKey, TValue>(Dictionary<TKey, TValue> source, TKey key, Func<TValue> factory)
        where TKey : notnull
    {
        TValue? result;
        if (!source.TryGetValue(key, out result))
            result = source[key] = factory();
        return result;
    }

    static TValue FindOrCreate<TKey, TValue>(Dictionary<TKey, TValue> source, TKey key, Func<TValue> factory, out bool newed)
        where TKey : notnull
    {
        TValue? result;
        newed = true;
        if (!source.TryGetValue(key, out result))
            return result = source[key] = factory();
        newed = false;
        return result;
    }

    [DebuggerStepThrough]
    private static int Hash(ReadOnlySpan<Type> types)
    {
        HashCode hashCode = new();
        foreach (var item in types)
            hashCode.Add(item);
        return hashCode.ToHashCode();
    }

    static class ContainerFactoryClass<T>
        where T : struct
    {
        private static IComponentStorage? _factCache;
        private static readonly Type[] _typeArray = new Type[] { typeof(T) };
        public static IComponentStorage Factory
        {
            get
            {
                _factCache ??= Generate();
                return _factCache;
            }
        }

        private static IComponentStorage Generate()
        {
            if (AssignableFrom<IComponent>())
                return (IComponentStorage)Activator.CreateInstance(typeof(CStorage<>).MakeGenericType(_typeArray))!;
            if(typeof(T).GetInterfaces().FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IComponent<>)) is { } obj)
            {
                Type[] generics = [.. _typeArray, .. obj.GenericTypeArguments];
                return (IComponentStorage)Activator.CreateInstance(typeof(CStorage<,>).MakeGenericType(generics))!;
            }
            throw new NotImplementedException();
        }

        private static bool AssignableFrom<TIs>()
        {
            return typeof(TIs).IsAssignableFrom(typeof(T));
        }
    }
}