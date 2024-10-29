using Db4Wd.Engine;
using Db4Wd.Postgres;
using Db4Wd.Postgres.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Versions = Db4Wd.Postgres.Schema;

var engine = new EngineHostBuilder(
        "pg4", 
        "Manages migrations in a Postgres database")
    .AddExtension<PostgresExtension>()
    .ConfigureServices(services => services
        .AddSingleton<SchemaManager>()
        .AddSingleton<NpgConnectionFactory>()
        .AddSingleton<IPostgresConnector, Versions.V1.MigrationConnector>()
        .AddSingleton<IPostgresConnector, Versions.V2.MigrationConnector>()
    )
    .ConfigureAppConfiguration(builder => builder.AddEnvironmentVariables("PG4_"))
    .Build();

string[] initArgs = ["init", "--env:/Users/dan/pg-local.json", "--prop:commandTimeout=30", "-v:trace"];
string[] updateArgs = ["update-connector", "--latest", "--env:/Users/dan/pg-local.json", "--prop:commandTimeout=30", "-v:trace"];
string[] versionArgs = ["version"];
string[] rootHelpArgs = ["--help"];
string[] locksHelpArgs = ["locks", "--help"];
string[] listLocksArgs = ["locks", "list", "--env:/Users/dan/pg-local.json", "-v:trace"];
string[] deleteLockArgs = ["locks", "delete", "--id", "96490cb7-81bf-477a-8c1b-2f304dd3f1e2", "--env:/Users/dan/pg-local.json", "-v:trace"];
string[] deleteLocksArgs = ["locks", "delete", "--all", "--env:/Users/dan/pg-local.json", "-v:trace"];

var result = await engine.ExecuteAsync(args);
return result;    