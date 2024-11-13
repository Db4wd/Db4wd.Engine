using DbForward.Engine;
using DbForward.Extensions;
using DbForward.Postgres;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = new EngineHostBuilder(
    Constants.ToolName,
    "Manages migrations in a Postgres database");

builder
    .ConfigureAppConfiguration(config => config.AddEnvironmentVariables("PGFWD_DB_"))
    .ConfigureServices(services => services
        .AddSingleton<IConnectionFactory, DefaultConnectionFactory>()
        .AddSingleton<IMigrationScopeFactory, MigrationScopeFactory>()
        .AddSingleton<ISchemaInitializer, SchemaInitializer>()
        .AddSingleton<IMetadataContext, PostgresMetadataContext>()
    )
    .AddExtension<PostgresDatabaseExtensions>()
    .AddEnvironmentFileOption("PGFWD_ENV")
    .Build();

var engine = builder.Build();

// pg-forward about
// pg-forward init
// pg-forward new
// pg-forward apply
// pg-forward rollback
// pg-forward status
// pg-forward new
// pg-forward history --id
// pg-forward detail --id
// pg-forward search --params
// pg-forward blob --id
// pg-forward audit
// pg-forward metadata insert --id
//            metadata delete

var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
var common = $"--log-level:debug --env:{profile}/pg-local.json";
var init = $"init {common}";
var newTemplate = $"new {profile}/migrations/migration_00001.sql --tag task=DEV_10056 {common}";
var apply = $"apply --base-path {profile}/migrations --statement-log-level debug --tag:build=a0029eff10299 {common}";
var rollback = $"rollback --base-path:{profile}/migrations --target-version:2024-11-10.00011 --statement-log-level:debug {common}";
var history = $"history b614878c-2513-4646-ae8d-841caddab30e {common}";
var detail = $"detail b614878c-2513-4646-ae8d-841caddab30e {common}";
var status = $"status {common}";
var audit = $"audit --base-path {profile}/migrations {common}";
var source = $"source b614878c-2513-4646-ae8d-841caddab30e {common}";
var sourceToFile = $"source b614878c-2513-4646-ae8d-841caddab30e --out {profile}/migration_00001.restored.sql {common}";


return await engine.ExecuteAsync(["pgfwd"]);    