using System;

using Microsoft.Extensions.Options;

public static class MockOptionsMonitor 
{
    public static IOptionsMonitor<T> Create<T>(T value) where T : class
        => new MockOptionsMonitor<T>(value);
}
public class MockOptionsMonitor<T> : IOptionsMonitor<T> where T : class
{
    public MockOptionsMonitor(T value)
    {
        CurrentValue = value;
    }

    public T CurrentValue { get; }

    public T Get(string name) => CurrentValue;

    public IDisposable OnChange(Action<T, string> listener)
    {
        return new DummyDisposable();
    }

    class DummyDisposable : IDisposable
    {
        public void Dispose() { }
    }

}
