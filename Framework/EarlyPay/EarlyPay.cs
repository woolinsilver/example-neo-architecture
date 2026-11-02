using SGStudios.LocalPrize.GameLogic;
using SGStudios.Progressive.GameLogic;
using SGStudios.Random.GameLogic;

namespace Framework.EarlyPay;

/// <summary>
/// <para>
/// An <see cref="IGameLogic"/> which implements Early Pay.
/// </para>
/// <para>
/// State is not persisted between Start Game and End Game transactions.<br/>
/// The End Game transaction does nothing.
/// </para>
/// </summary>
public abstract class EarlyPayGameLogic<TCommand, TResult>
    : GameLogicBase<State, TCommand, TResult, State, EndCommand, EndResult>
{
    public sealed override State CreateDefaultState() => State.Empty;

    protected abstract Task<Response<TResult>> ExecuteAsync(Request<TCommand> request, IGameLogicRandom random, IGameLogicProgressive progressive, CancellationToken ct);

    public sealed override async Task<StartGameResponse<State, TResult>> StartGameAsync(
        StartGameRequest<State, TCommand> request,
        IGameLogicRandom random,
        IGameLogicProgressive progressive,
        CancellationToken ct = default)
    {
        var response = await ExecuteAsync(RemoveState(request), random, progressive, ct);
        return AddEmptyState(response);
    }

    private static Request<TCommand> RemoveState(StartGameRequest<State, TCommand> request)
    {
        return new Request<TCommand>
        {
            TotalStake = request.Stake,
            Command = request.Command,
            Time = request.StartTime,
        };
    }

    private static StartGameResponse<State, TResult> AddEmptyState(Response<TResult> response)
    {
        return new StartGameResponse<State, TResult>
        {
            Prize = response.TotalPrize,
            State = State.Empty,
            Result = response.Result,
        };
    }

    /// <summary>
    /// Does nothing.
    /// </summary>
    public sealed override EndGameResponse<State, EndResult> EndGame(
        EndGameRequest<State, EndCommand> request)
    {
        return new EndGameResponse<State, EndResult>
        {
            State = State.Empty,
            Result = EndResult.Empty,
        };
    }

}

/// <summary>
/// An Early Pay game does not persist state between Start Game / End Game transactions.
/// </summary>
public class State
{
    public static readonly State Empty = new();
}

/// <summary>
/// An Early Pay game does not require the frontend to send data during End Game.
/// </summary>
public class EndCommand
{
    // nothing to deserialize
}

/// <summary>
/// An Early Pay game does not send data back to the frontend during End Game.
/// </summary>
public class EndResult
{
    public static readonly EndResult Empty = new();
}
