using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Framework.Contracts;
using Framework.EarlyPay;
using Mermaid.Profile;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using SGStudios.LocalPrize.GameLogic;
using SGStudios.Money;
using SGStudios.Progressive.Rigging;
using SGStudios.Random.Rigging;
using Xunit.Abstractions;

namespace Test;

public class IntegrationTest
{
    private static readonly DateTimeOffset StartTime = DateTimeOffset.Parse("2000-01-01T00:00:00+00:00");

    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    protected readonly ITestOutputHelper Output;
    private readonly ILoggerFactory _loggerFactory;

    protected IntegrationTest(ITestOutputHelper output)
    {
        Output = output;
        _loggerFactory = new LoggerFactory().AddSerilog(
            new LoggerConfiguration()
                .WriteTo.Sink(new XunitSink(output))
                .CreateLogger());
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection([])
            .Build();
    }

    protected async Task<MermaidEntryPoint> CreateGameLogicAsync()
    {
        var logic = new MermaidEntryPoint();

        await logic.InitializeAsync(
            configuration: BuildConfiguration(),
            pluginLoader: ExplodingPluginLoader.Instance,
            loggerFactory: _loggerFactory
        );

        return logic;
    }

    protected IRiggedGameLogicRandom CreateRandom()
    {
        return RiggedGameLogicRandomFactory.Create(_loggerFactory.CreateLogger<RiggedGameLogicRandom>());
    }

    protected IRiggedGameLogicProgressive CreateProgressive()
    {
        return RiggedGameLogicProgressiveFactory.Create(_loggerFactory.CreateLogger<RiggedGameLogicProgressive>());
    }

    protected static StartGameRequest<State, Command> CreateRequest(Currency? currency = null, decimal stakePerTicket = 1.00m, int numberOfTickets = 1)
    {
        return new StartGameRequest<State, Command>
        {
            State = State.Empty,
            Command = new Command
            {
                StakePerTicket = stakePerTicket,
                NumberOfTickets = numberOfTickets,
            },
            StartTime = StartTime,
            Stake = (currency ?? Currency.GBP).Amount(numberOfTickets * stakePerTicket)
        };
    }

    protected static string Serialize(object @object)
    {
        return JsonSerializer.Serialize(@object, JsonSerializerOptions);
    }
}

internal class ExplodingPluginLoader : IPluginLoader
{
    public static readonly IPluginLoader Instance = new ExplodingPluginLoader();

    public Assembly LoadPlugin(string filename)
    {
        // not permitted to reflectively load DLLs
        throw new UnreachableException();
    }
}

internal class XunitSink(ITestOutputHelper output) : ILogEventSink
{
    public void Emit(LogEvent logEvent) => output.WriteLine(logEvent.RenderMessage());
}
