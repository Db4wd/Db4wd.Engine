namespace DbForward.Engine.Models;

public interface INewEnvironmentOptions : IConnectionOptions
{
    FileInfo OutputFile { get; set; }
    bool Overwrite { get; set; }
}