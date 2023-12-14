using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Saorsa.QueryEngine.Tests;


/// <summary>
/// Base class for tests.
/// </summary>
public class TestBase
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
    /// Gets the shared logger for all tests.
    /// </summary>
    public ILogger<TestBase> Logger { get; private set; } = default!;

    /// <summary>
    /// Shared task cancellation source.
    /// </summary>
    public static readonly CancellationTokenSource DefaultTaskCancellation = new ();

    /// <summary>
    /// Gets the shared task cancellation token.
    /// </summary>
    public static CancellationToken DefaultCancellationToken => DefaultTaskCancellation.Token;

    /// <summary>
    /// Shared cancellation token.
    /// </summary>
    public CancellationToken CancellationToken => DefaultTaskCancellation.Token;

    /// <summary>
    /// Gets the DI global service scope. All child scopes are created from this one.
    /// </summary>
    public IServiceScope TestServiceScope { get; private set; } = default!;

    /// <summary>
    /// Resolves a required service from the service scope.
    /// Raises an exception if the service type is not registered.
    /// </summary>
    /// <typeparam name="T">The type of service in DI container.</typeparam>
    protected virtual T GetRequiredService<T>() where T : notnull
    {
        return TestServiceScope.ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Attempts to resolve an optional service from the service scope.
    /// Returns NULL if service type is not registered.
    /// </summary>
    /// <typeparam name="T">The type of service in DI container.</typeparam>
    protected virtual T? GetService<T>()
    {
        return TestServiceScope.ServiceProvider.GetService<T>();
    }

    /// <summary>
    /// Gets an instance for the DI service provider.
    /// </summary>
    protected virtual IServiceProvider GetServiceProvider()
    {
        return TestBootstrapper.ServiceProvider;
    }

    /// <summary>
    /// One time tests setup, involved once before any of test cases of the test class are run.
    /// </summary>
    [OneTimeSetUp]
    protected virtual Task TestSetupAsync()
    {
        _TestStartedAt = DateTimeOffset.Now;

        var serviceProvider = GetServiceProvider();
        Assert.NotNull(serviceProvider, $"{nameof(GetServiceProvider)} must return a valid provider instance.");

        TestServiceScope = serviceProvider.CreateScope();
        Assert.NotNull(TestServiceScope, $"{nameof(TestServiceScope)} is created successfully.");

        Logger = GetRequiredService<ILogger<TestBase>>();
        Assert.NotNull(Logger, $"{nameof(Logger)} is created successfully.");

        Logger.LogInformation($"{nameof(TestServiceScope)} - {nameof(TestSetupAsync)} - complete.");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Tear down, performs a cleanup after all test cases of the test class have been run.
    /// </summary>
    [OneTimeTearDown]
    public virtual Task TestTearDownAsync()
    {
        Logger.LogInformation($"{nameof(TestServiceScope)} - {nameof(TestTearDownAsync)} - starting tear down.");
        TestServiceScope.Dispose();
        
        _TestFinishedAt = DateTimeOffset.Now;

        Logger.LogInformation($"{GetType().Name} - {nameof(TestTearDownAsync)} - " +
                              $"complete in {(_TestFinishedAt - _TestStartedAt)}.");

        return Task.CompletedTask;
    }
}
