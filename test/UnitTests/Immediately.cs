using NSubstitute;

namespace UnitTests;

public static class Immediately
{
    public static T Create<T>(Func<T> factory) => factory();

    public static T CreateMock<T>(Action<T> configure) where T : class
    {
        var mock = Substitute.For<T>();
        configure(mock);
        return mock;
    }
}