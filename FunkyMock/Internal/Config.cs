using Microsoft.CodeAnalysis.Diagnostics;

namespace FunkyMock.Internal;

public record Config(bool ExplicitImplementation);

public static class ConfigPipeline
{
    public static Config Select (AnalyzerConfigOptionsProvider provider, CancellationToken _)
    {
        var cfg = new Config(
            ExplicitImplementation :true
        );

        if ((provider?.GlobalOptions.TryGetValue("funky.explicit_interfaces", out var val) ?? false) && val == "false")
        {
            cfg = cfg with {ExplicitImplementation = false};
        }

        return (cfg);
    }
}
