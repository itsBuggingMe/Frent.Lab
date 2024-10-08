namespace Frent.Lab;

internal class Program
{
    static void Main(string[] args)
    {
        World world = new World();

        for(int i = 0; i < 100000; i++)
        {
            world
                .With<Position>(default)
                .With<Velocity, Position>(default)
                .Complete();
        }

        world.Update();
    }
}

internal struct Position(float x, float y) : IComponent
{
    public float X = x;
    public float Y = y;

    public void Update() => Console.WriteLine("pos");
}

internal struct Velocity(float dx, float dy) : IComponent<Position>
{
    public float dX = dx;
    public float dY = dy;

    public void Update(ref Position a)
    {
        a.X += dX;
        a.Y += dY;
        Console.WriteLine("vel");
    }
}