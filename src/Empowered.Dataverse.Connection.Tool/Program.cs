﻿using System.Diagnostics;
using CommandDotNet;
using CommandDotNet.Builders;
using CommandDotNet.Builders.ArgumentDefaults;
using CommandDotNet.Execution;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using CommandDotNet.NameCasing;
using CommandDotNet.Spectre;
using Empowered.Dataverse.Connection.Commands;
using Empowered.Dataverse.Connection.Commands.Extensions;
using Empowered.SpectreConsole.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Spectre.Console;

namespace Empowered.Dataverse.Connection.Tool;

public static class Program
{
    public static AppRunner GetAppRunner(IAnsiConsole? console = null)
    {
        var appRunner = new AppRunner<ConnectionCommand>()
            .UseSpectreAnsiConsole(console)
            .UseSpectreArgumentPrompter();
        var commandClassTypes = appRunner.GetCommandClassTypes();
        var serviceCollection = new ServiceCollection()
            .AddConnectionCommand();

        foreach (var commandClassType in commandClassTypes)
        {
            serviceCollection.TryAddScoped(commandClassType.type);
        }

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
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
            )
            .UseErrorHandler(ErrorHandler)
            .UseCancellationHandlers()
            .UseVersionMiddleware()
            .UseTypoSuggestions()
            .UseDefaultsFromConfig()
            .UseNameCasing(Case.KebabCase)
            .UseMicrosoftDependencyInjection(
                serviceCollection.BuildServiceProvider(),
                argumentModelResolveStrategy: ResolveStrategy.TryResolve,
                commandClassResolveStrategy: ResolveStrategy.ResolveOrThrow
            );
    }

    public static int Main(string[] args) => GetAppRunner().Run(args);

    private static int ErrorHandler(CommandContext? context, Exception exception)
    {
        var errorCode = ExitCodes.Error.GetAwaiter().GetResult();

        if (context?.DependencyResolver == null)
        {
            return errorCode;
        }

        var console = context.DependencyResolver.Resolve<IAnsiConsole>() ?? AnsiConsole.Console;

        console.Error(exception.Message.EscapeMarkup());
        return errorCode;
    }

    [Conditional("DEBUG")]
    private static void AddDebugExtensions(this AppRunner appRunner)
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