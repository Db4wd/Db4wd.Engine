using Db4Wd.Features;
using Vertical.Cli.Configuration;

namespace Db4Wd.Cli;

public interface IFeatureConfiguration<TBaseOptions> where TBaseOptions : GlobalOptions
{
    void Configure(CliCommand<TBaseOptions> parent, FeatureBuilder builder);
}