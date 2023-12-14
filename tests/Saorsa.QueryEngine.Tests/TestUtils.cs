using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace Saorsa.QueryEngine.Tests;


/// <summary>
/// Common utility methods container.
/// </summary>
public class TestUtils
{
    /// <summary>
    /// Stores the runtime evaluated environment name. Defaults to Test if ASPNETCORE_ENVIRONMENT variable is unset.
    /// </summary>
    public static string RuntimeEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Test";

    /// <summary>
    /// Creates a simple logger for the given category type.
    /// </summary>
    /// <typeparam name="TCategory"></typeparam>
    /// <returns></returns>
    public static ILogger<TCategory> CreateDefaultLogger<TCategory>()
    {
        var serilogLogger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .CreateLogger();

        return  new SerilogLoggerFactory(serilogLogger).CreateLogger<TCategory>();
    }
}
