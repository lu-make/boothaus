namespace Boothaus.GUI.Services;

public class EingabemaskeResult<T>
{
    public bool Success { get; set; }
    public T? Value { get; set; }
}
