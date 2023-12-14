using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Saorsa.QueryEngine.Tests;

/// <summary>
/// Global test bootstrapper.
/// </summary>
public class TestBootstrapper
{
    /// <summary>
    /// Container for the start time of the bootstrap.
    /// </summary>
    private DateTimeOffset _TestStartedAt;

    /// <summary>
    /// Container for the finish time of the bootstrap.
    /// </summary>
    private DateTimeOffset? _TestFinishedAt;

    /// <summary>
    /// Shared cancellation token for all tests in the Bootstrapper scope.
    /// </summary>
    public static readonly CancellationTokenSource DefaultTaskCancellation = new ();

    /// <summary>
    /// Gets the shared logger for the bootstrapper.
    /// </summary>
    public static ILogger<TestBootstrapper> Logger { get; private set; } = default!;

    /// <summary>
    /// Gets the global DI service provider.
    /// </summary>
    public static ServiceProvider ServiceProvider { get; private set; } = default!;
        
    /// <summary>
    /// Gets the global DI scope. Used to create child scopes on a test class level.
    /// </summary>
    public static IServiceScope GlobalScope { get; private set; } = default!;

    /// <summary>
    /// Gets the DI container with services.
    /// </summary>
    public static IServiceCollection Services { get; } = new ServiceCollection();
    

    /// <summary>
    /// Performs global tests setup, before any of the test classes and their test methods have been run.
    /// </summary>
    [OneTimeSetUp]
    public virtual async Task TestsSetupAsync()
    {
        _TestStartedAt = DateTimeOffset.Now;

        await SetupLoggingAsync();

        ServiceProvider = Services.BuildServiceProvider();

        GlobalScope = ServiceProvider.CreateScope();

        Logger = GlobalScope.ServiceProvider.GetRequiredService<ILogger<TestBootstrapper>>();
        
        Logger.LogInformation($"{GetType().Name} - {nameof(TestsSetupAsync)} - complete.");
    }

    /// <summary>
    /// Performs global tests cleanup, after all test classes and their test methods have been run.
    /// </summary>
    [OneTimeTearDown]
    public virtual Task TestsTearDownAsync()
    {
        Logger.LogInformation($"{GetType().Name} - {nameof(TestsTearDownAsync)} - starting tear down.");

        GlobalScope.Dispose();
        ServiceProvider.Dispose();
        
        _TestFinishedAt = DateTimeOffset.Now;

        Logger.LogInformation($"{GetType().Name} - {nameof(TestsTearDownAsync)} - " +
                              $"complete in {(_TestFinishedAt - _TestStartedAt)}.");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Initializes the service collection container for DI.
    /// </summary>
    protected virtual Task SetupLoggingAsync()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel
                .Debug()
            .WriteTo
                .Console(outputTemplate:"{Timestamp:HH:mm:ss} {Level} {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        Services
            .AddLogging(lb => lb.AddSerilog(dispose: true));
        
        Log.Logger.Debug($"{GetType().Name} - {nameof(SetupLoggingAsync)} - complete.");
        return Task.CompletedTask;
    }
}
