namespace DbForward.Engine.Parsing;

public record SourceHeader(string MigrationId, Dictionary<string, string> Metadata);