using CommandDotNet;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;

namespace Empowered.Dataverse.CommandDotnet.Xunit.Extensions;

public static class AppRunnerScenarioExtensions
{
    public static AppRunnerResult RunInMem(
        this AppRunner runner,
        string args,
        Func<ITestConsole, string>? onReadLine = null,
        IEnumerable<string>? pipedInput = null)
    {
        return runner.RunInMem(args, Ambient.WriteLine, onReadLine, pipedInput);
    }

    public static AppRunnerResult RunInMem(
        this AppRunner runner,
        string[] args,
        Func<ITestConsole, string>? onReadLine = null,
        IEnumerable<string>? pipedInput = null
    )
    {
        return runner.RunInMem(args, Ambient.WriteLine, onReadLine, pipedInput);
    }

    public static AppRunnerResult Verify(this AppRunner appRunner, IScenario scenario)
    {
        // use Test.Default to force testing of TestConfig.GetDefaultFromSubClass()
        return appRunner.Verify(Ambient.WriteLine, new TestConfig
        {
            PrintCommandDotNetLogs = false,
            OnError = new TestConfig.OnErrorConfig
            {
                Print = new TestConfig.PrintConfig
                {
                    ConsoleOutput = true,
                    AppConfig = false,
                    CommandContext = false,
                    ParseReport = false,
                    All = false
                }
            },
            OnSuccess = new TestConfig.OnSuccessConfig
            {
                Print = new TestConfig.PrintConfig
                {
                    ConsoleOutput = true,
                    AppConfig = false,
                    ParseReport = false,
                    CommandContext = false,
                    All = false
                }
            }
        }, scenario);
    }
}