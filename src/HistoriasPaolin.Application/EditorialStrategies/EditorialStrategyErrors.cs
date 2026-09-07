namespace HistoriasPaolin.Application.EditorialStrategies;

public sealed class EditorialStrategyValidationException(IReadOnlyList<string> errors) : Exception("Editorial strategy validation failed.")
{
    public IReadOnlyList<string> Errors { get; } = errors;
}

public sealed class EditorialStrategyNotFoundException(Guid id) : Exception($"Editorial strategy '{id}' was not found.");

public sealed class EditorialStrategyActiveNotFoundException(Guid channelId) : Exception($"Active editorial strategy for channel '{channelId}' was not found.");

public sealed class EditorialStrategyChannelNotFoundException(Guid channelId) : Exception($"Channel '{channelId}' was not found.");

public sealed class EditorialStrategyInactiveChannelException(Guid channelId) : Exception($"Channel '{channelId}' is inactive.");

public sealed class EditorialStrategyConcurrencyException() : Exception("Editorial strategy was modified by another process.");
