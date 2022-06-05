namespace IOptionTest.Controllers;

/// <summary>
/// Base class for logging scoped messages when scope in an object
/// </summary>s
public class ObjectScopeControllerBase<T> : ScopeControllerBase where T : class
{
    private readonly Func<T, Dictionary<string, object>> _converter;

    protected ObjectScopeControllerBase(ILogger logger, Func<T, Dictionary<string,object>> converter) : base(logger)
    {
        _converter = converter;
    }


    protected TRetrun Invoke<TRetrun>(T context, Func<TRetrun> func)
    {
        return Invoke(func, _converter(context));
    }

    protected async Task<TReturn> InvokeAsync<TReturn>(T context, Func<Task<TReturn>> func)
    {
        return await InvokeAsync(func, _converter(context));
    }
}

