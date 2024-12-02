namespace Frontend.DataBags;


/// <summary>
/// Temporary store of data.
/// </summary>
/// <typeparam name="T"></typeparam>
internal abstract class CascadingDataBagTemplate<T>
{
    T? _value;
    public T? Get()
    {
        var v = _value;
        _value = default;
        return v;
    }
    public T? Peek() => _value;
    public void Set(T value) => _value = value;
}