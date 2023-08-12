using System.Diagnostics;
using CommandDotNet;
using CommandDotNet.Builders.ArgumentDefaults;
using CommandDotNet.Execution;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using CommandDotNet.NameCasing;
using CommandDotNet.Spectre;
using Empowered.Dataverse.Connection.Client.Extensions;
using Empowered.Dataverse.Connection.Store.Extensions;
using Empowered.Dataverse.Connection.Tool.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace Empowered.Dataverse.Connection.Tool;

public static class Program
{
    public static int Main(string[] args)
    {
        var appRunner = new AppRunner<ConnectionCommand>();
        var commandClassTypes = appRunner.GetCommandClassTypes();
        var serviceCollection = new ServiceCollection()
            .AddSingleton<IAnsiConsole>(_ => AnsiConsole.Console)
            .AddConnectionStore()
            .AddDataverseClientFactory();

        foreach (var commandClassType in commandClassTypes)
        {
            serviceCollection.AddScoped(commandClassType.type);
        }

        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables("3MPWRD:")
            .Build();

        appRunner.AddDebugExtensions();
        return appRunner
            .UseDefaultsFromConfig(
                DefaultSources.GetValueFunc(
                    nameof(DefaultSources.EnvVar),
                    key => configuration[key],
                    DefaultSources.EnvVar.GetKeyFromAttribute
                )
            ).UseCancellationHandlers()
            .UseVersionMiddleware()
            .UseTypoSuggestions()
            .UseDefaultsFromConfig()
            .UseNameCasing(Case.KebabCase)
            .UseSpectreAnsiConsole()
            .UseSpectreArgumentPrompter()
            .UseMicrosoftDependencyInjection(
                serviceCollection.BuildServiceProvider(),
                argumentModelResolveStrategy: ResolveStrategy.TryResolve,
                commandClassResolveStrategy: ResolveStrategy.ResolveOrThrow
            ).Run(args);
    }

    [Conditional("DEBUG")]
    private static void AddDebugExtensions(this AppRunner<ConnectionCommand> appRunner)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("local.settings.json", optional: true)
            .Build();

        appRunner
            .UseDebugDirective()
            .UseParseDirective()
            .UseCommandLogger()
            .UseResponseFiles()
            .UseLocalizeDirective()
            .UseTimeDirective()
            .UseDefaultsFromConfig(
                DefaultSources.GetValueFunc(nameof(DefaultSources.AppSetting),
                    key => configuration[key],
                    DefaultSources.AppSetting.GetKeyFromAttribute
                )
            );
    }
}