namespace Frent.Lab;

internal interface IComponent
{
    void Update();
}

internal interface IComponent<TArg>
{
    void Update(ref TArg arg);
}

internal interface IComponent<TArg1, TArg2>
{
    void Update(ref TArg1 arg1, ref TArg2 arg2);
}