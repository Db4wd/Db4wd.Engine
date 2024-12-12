// See https://aka.ms/new-console-template for more information

using DbForward.Engine;
using DbForward.Engine.Extensions;
using DbForward.Postgres;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var engine = new DbForwardEngineBuilder("pg4")
    .ConfigureAppConfiguration(config => config.AddEnvironmentVariables("PG4_"))
    .ConfigureServices(services => services
        .AddSingleton<IDb4Extension, PostgresDb4Extension>()
        .AddSingleton<NpgSqlConnectionFactory>()
        .AddSingleton(PostgresSourceReader.Factory))
    .Build();

return await engine.RunAsync("env --help".Split(' '));
