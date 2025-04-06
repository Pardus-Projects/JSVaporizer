namespace NuFlexiArch;

public interface IComponent
{
    public abstract IComponentRenderer GetRenderer();
}
public interface IComponentRenderer
{
    public Task<object> RenderAsync(AComponent comp, params object[] args);
}

// In case you need one
public class BlackHole : IComponentRenderer
{
    public Task<object> RenderAsync(AComponent comp, params object[] args)
    {
        var taskSource = new TaskCompletionSource<object>();
        taskSource.SetResult(new());
        return taskSource.Task;
    }
}




