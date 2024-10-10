using System.Numerics;

namespace Frent.Lab;

internal class Program
{
    static void Main(string[] args)
    {
        var w = new World();

        for(int i = 0; i < 10_000_000; i++)
        {
            w.Create<Position, Velocity>();
        }

        w.Update();
    }
}

struct Position : IComponent
{
    public Vector2 Value;
    public void Update() { }
}

struct Velocity : IComponent<Position>
{
    public Vector2 Delta;
    public void Update(ref Position arg)
        => arg.Value += Delta;
}