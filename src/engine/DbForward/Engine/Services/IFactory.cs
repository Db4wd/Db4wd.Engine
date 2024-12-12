namespace DbForward.Engine.Services;

public interface IFactory<out TService> where TService : class
{
    TService Create();
}

public interface IParameterizedFactory<in TParam, out TService> where TService : class
{
    TService Create(TParam tokenDictionary);
}

public interface IParameterizedFactory<in TParam1, in TParam2, out TService> where TService : class
{
    TService Create(TParam1 param1, TParam2 param2);
}