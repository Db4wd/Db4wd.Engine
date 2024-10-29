namespace Db4Wd.Logging;

public sealed class ParameterizedMessageException(string template, params object[] args) : Exception
{
    public string Template => template;

    public object[] Arguments => args;
}