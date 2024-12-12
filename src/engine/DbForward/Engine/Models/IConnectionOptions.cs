namespace DbForward.Engine.Models;

public interface IConnectionOptions
{
    string? ConnectionString { get; set; }
    string? Host { get; set; }
    uint? Port { get; set; }
    string? Database { get; set; }
    string? UserId { get; set; }
    string? Password { get; set; }
    KeyValuePair<string, string>[] Properties { get; set; }
}