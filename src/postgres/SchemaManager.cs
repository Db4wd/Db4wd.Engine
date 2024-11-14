using System.Collections.ObjectModel;
using System.Reflection;
using Dapper;
using DbForward.Constants;
using DbForward.Extensions;
using DbForward.Models;
using DbForward.Services;
using Microsoft.Extensions.Logging;

namespace DbForward.Postgres;

public interface ISchemaManager
{
    /// <summary>
    /// Performs a migration on the internal metadata schema.
    /// </summary>
    /// <param name="parent">Parent extension</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns><c>bool</c></returns>
    Task<bool> InitializeAsync(IDatabaseExtension parent, CancellationToken cancellationToken);

    /// <summary>
    /// Gets whether the schema is initialized.
    /// </summary>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task that returns <c>bool</c></returns>
    Task<bool> IsSchemaInitializedAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Performs post operation checks.
    /// </summary>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns></returns>
    Task PostCheckOperationAsync(CancellationToken cancellationToken);
}

internal sealed class SchemaManager(
    IConnectionFactory connectionFactory,
    IFileSystem fileSystem,
    IAgentContext agentContext,
    IMigrationOperator migrationOperator
    ) : ISchemaManager
{
    /// <inheritdoc />
    public async Task<bool> InitializeAsync(IDatabaseExtension parent, CancellationToken cancellationToken)
    {
        var postgresPath = Path.Combine(
            Constants.RootAssetPath,
            "Postgres",
            "metadata_schema.sql");
        var emptyDictionary = ReadOnlyDictionary<string, string>.Empty;
        var parameters = new MigrationParameters(SourceOperation.Migrate,
            agentContext,
            fileSystem,
            ConventionalSourceReader.Instance,
            async (operation, tracker, token) => await parent.CreateMigrationScopeAsync(operation, tracker, token),
            null,
            [new SourceHeader(postgresPath,
                Guid.NewGuid(), 
                DbVersion.Create(0),
                emptyDictionary)],
            Tokens: new Dictionary<string, string>
            {
                ["__schema__"] = Constants.SchemaName
            }, 
            Metadata: new Dictionary<string, string>
            {
                ["pgfwd/npgSqlConnectionVersion"] = "8.0.5",
                ["pgfwd/dapperVersion"] = "2.1.35",
                ["pgfwd/internalMigration"] = "true"
            }, 
            LogLevel.Debug);

        var response = await migrationOperator.ApplyAsync(parameters, cancellationToken);

        return response.Response == OperationResponse.Successful;
    }

    public async Task<bool> IsSchemaInitializedAsync(CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.CreateAsync(cancellationToken);

        var count = await connection.QuerySingleAsync<int>(
            """
            select count(*)
            from information_schema.tables
            where table_schema=@schema;
            """,
            new {schema = Constants.SchemaName});

        return count == 6;
    }

    /// <inheritdoc />
    public async Task PostCheckOperationAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}