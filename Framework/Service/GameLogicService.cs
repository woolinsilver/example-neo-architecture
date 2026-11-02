using Framework.Contracts;
using Framework.Domain;
using Framework.EarlyPay;
using Framework.Profiles;
using Framework.Tickets;
using SGStudios.Money;
using SGStudios.Progressive.GameLogic;
using SGStudios.Random;
using SGStudios.Random.GameLogic;
using SGStudios.Random.Generator;

namespace Framework.Service;

public class GameLogicService<TTier, TTicket>(ITicketFactory<TTier, TTicket> factory, IProfile<TTier> profile)
    where TTier : Tier
    where TTicket : Ticket
{
    public async Task<Response<Result<TTicket>>> ExecuteAsync(
        Request<Command> request,
        IGameLogicRandom random,
        IGameLogicProgressive _, // progressives ignored in example code
        CancellationToken ct = default)
    {
        profile.Validate(request);

        var paytable = profile.SelectPaytable(request.Command.StakePerTicket, request.Time);

        int count = request.Command.NumberOfTickets;

        var data = await GetTicketRandomData(paytable, count, random, ct);

        TTicket[] tickets = CreateTickets(data, paytable);

        Currency currency = request.TotalStake.Currency;
        CurrencyAmount prize = currency.Amount(TotalPrize(tickets));

        return new Response<Result<TTicket>>
        {
            TotalPrize = prize,
            Result = new Result<TTicket>
            {
                Tickets = tickets
            }
        };
    }

    private static async Task<TicketRandomData[]> GetTicketRandomData(
        Paytable<TTier> paytable,
        int count,
        IGameLogicRandom random,
        CancellationToken ct)
    {
        var request = new RandomRequest.Builder();

        // one scaled number for each ticket
        for (int i = 0; i < count; i++)
            request.RequestNumber(paytable.TotalWeight);

        // a single seed for a "parent" generator
        request.RequestSeed();

        // talk to server
        var data = await random.GetRandomDataAsync(request.Build(), ct);

        // unpack data from server
        (uint[] numbers, Seed seed) = (data.GetNumbers(), data.GetSeeds().Single());

        // use single seed to create many child generators
        IEnumerable<IDeterministicGenerator> generators = SpawnGenerators(seed, count);

        // return one TicketRandomData for each ticket
        return [
            ..numbers.Zip(generators, (number, generator) => new TicketRandomData
            {
                Number = number,
                Generator = generator
            })
        ];

    }

    /// <summary>
    /// Use a single seed from an <see cref="IGameLogicRandom"/> to create many local generators.
    /// </summary>
    private static IEnumerable<IDeterministicGenerator> SpawnGenerators(Seed seed, int count)
    {
        IDeterministicGenerator parent = seed.CreateSystemRandom();
        return Enumerable.Range(0, count)
            .Select(_ => GenerateSeed(parent).CreateSystemRandom());
    }

    /// <summary>
    /// Use an existing generator as a factory for seeds.
    /// </summary>
    private static Seed GenerateSeed(IDeterministicGenerator generator)
    {
        return Seed.From(generator.GetRandomNumber(RandomLimits.MaximumUpperBound));
    }

    /// <summary>
    /// Use <see cref="TicketRandomData"/> to deterministically create tickets.
    /// </summary>
    private TTicket[] CreateTickets(TicketRandomData[] data, Paytable<TTier> paytable)
    {
        TTicket[] tickets = data.Select(each =>
        {
            var tier = paytable.SelectTier(each.Number);
            return factory.CreateTicket(tier, each.Generator);
        }).ToArray();
        return tickets;
    }

    private static decimal TotalPrize(TTicket[] tickets)
    {
        return tickets.Select(ticket => ticket.Prize).Sum();
    }
}
