namespace NuFlexiArch;

public interface IComponent
{
    public abstract IComponentRenderer GetRenderer();
}
public interface IComponentRenderer
{
    public object Render(AComponent comp, params object[] args);
}

// In case you need one
public class BlackHole : IComponentRenderer
{
    public object Render(AComponent comp, params object[] args)
    {
        return new();
    }
}




