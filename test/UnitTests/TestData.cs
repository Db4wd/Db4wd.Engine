using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using DbForward.Models;
using DbForward.Services;
using NSubstitute;

namespace UnitTests;

public static partial class TestData
{
    public static readonly Guid[] Ids = Enumerable.Range(0, 5).Select(_ => Guid.NewGuid()).ToArray();

    public static readonly SourceHeader[] Sources = Enumerable
        .Range(0, 5)
        .Select(i =>
            new SourceHeader($"path/file_{i:D2}.sql", Ids[i], $"v{i}", ReadOnlyDictionary<string, string>.Empty))
        .ToArray();

    public static readonly MigrationEntry[] Entries = Sources
        .Select(source => new MigrationEntry(source.MigrationId,
            source.DbVersion,
            DateTime.Now,
            Path.GetDirectoryName(source.Context)!,
            Path.GetFileName(source.Context),
            RandomNumberGenerator.GetHexString(32)))
        .ToArray();

    public static readonly IDbVersionComparer VersionComparer = Immediately.Create(() =>
    {
        var comparer = Substitute.For<IDbVersionComparer>();
        comparer
            .Compare(Arg.Any<string>(), Arg.Any<string>())
            .Returns(ci => string.Compare(ci.ArgAt<string>(0), ci.ArgAt<string>(1), StringComparison.Ordinal));
        comparer.IsValid(Arg.Any<string>()).Returns(ci => MyRegex().IsMatch(ci.Arg<string>()));
        return comparer;
    });

    public static readonly IAgentContext AgentContext = Immediately.CreateMock<IAgentContext>(mock =>
    {
        mock.Agent.Returns("tester");
        mock.Host.Returns("xunit");
        mock.TimeZoneOffset.Returns(TimeSpan.FromHours(-7));
    });

    [GeneratedRegex(@"v[\d]+")]
    private static partial Regex MyRegex();

    public static readonly Dictionary<string, string> EmptyDictionary = new();
}