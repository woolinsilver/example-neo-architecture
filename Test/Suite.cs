using Framework.Contracts;
using Framework.EarlyPay;
using SGStudios.LocalPrize.GameLogic;
using SGStudios.Money;
using SGStudios.Random.Rigging;
using Xunit;
using Xunit.Abstractions;

namespace Test;

public class Suite(ITestOutputHelper output) : IntegrationTest(output)
{
    [Fact]
    public async Task CanBeConstructed()
    {
        await CreateGameLogicAsync();
    }

    [Fact]
    public async Task CanSerializeResult()
    {
        var logic = await CreateGameLogicAsync();
        var response = await logic.StartGameAsync(
            CreateRequest(stakePerTicket: 1.00m),
            CreateRandom(),
            CreateProgressive()
        );
        var json = Serialize(response.Result);
        Assert.NotEmpty(json);
        Output.WriteLine(json);
    }

    [Theory]
    [InlineData(10, "Lose", 0.00)]
    [InlineData(75, "ThreeTridents", 2.00)]
    [InlineData(98, "ThreePearls", 8.00)]
    public async Task CanForceTiers(uint randomValue, string expectedTier, decimal expectedPrize)
    {
        var logic = await CreateGameLogicAsync();
        var random = CreateRandom();
        random.Inject(new RandomInjection { Numbers = [randomValue] });

        var response = await logic.StartGameAsync(
            CreateRequest(stakePerTicket: 1.00m),
            random,
            CreateProgressive()
        );

        Assert.Equal(expectedTier, response.Result.Tickets.First().Tier);
        Assert.Equal(expectedPrize, response.Prize.Amount);
    }

    [Theory]
    [InlineData(0.50)]
    [InlineData(2.00)]
    public async Task InvalidStakeThrows(decimal invalidStake)
    {
        var logic = await CreateGameLogicAsync();
        var request = CreateRequest(stakePerTicket: invalidStake);
        await Assert.ThrowsAnyAsync<Exception>(() => logic.StartGameAsync(request, CreateRandom(), CreateProgressive()));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task InvalidTicketCountThrows(int numberOfTickets)
    {
        var logic = await CreateGameLogicAsync();
        var request = CreateRequest(stakePerTicket: 1.00m, numberOfTickets: numberOfTickets);
        await Assert.ThrowsAnyAsync<Exception>(() => logic.StartGameAsync(request, CreateRandom(), CreateProgressive()));
    }

    [Fact]
    public async Task StakeMismatchThrows()
    {
        var logic = await CreateGameLogicAsync();
        var request = new StartGameRequest<State, Command>
        {
            State = State.Empty,
            Command = new Command
            {
                StakePerTicket = 1.00m,
                NumberOfTickets = 1,
            },
            StartTime = DateTimeOffset.Now,
            Stake = Currency.GBP.Amount(2.00m) // mismatch
        };

        await Assert.ThrowsAnyAsync<Exception>(() => logic.StartGameAsync(request, CreateRandom(), CreateProgressive()));
    }

    [Fact]
    public void InvalidCurrencyThrows()
    {
        Assert.Throws<UnsupportedCurrencyException>(() => CreateRequest(currency: Currency.FromCode("INVALID")));
    }
}
