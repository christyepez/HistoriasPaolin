namespace HistoriasPaolin.Application.Channels;

public sealed class ChannelValidationException(IReadOnlyList<string> errors) : Exception("Channel validation failed.")
{
    public IReadOnlyList<string> Errors { get; } = errors;
}

public sealed class ChannelNotFoundException(Guid id) : Exception($"Channel '{id}' was not found.");

public sealed class ChannelCodeNotFoundException(string code) : Exception($"Channel '{code}' was not found.");

public sealed class ChannelDuplicateCodeException(string code) : Exception($"Channel code '{code}' already exists.");

public sealed class ChannelConcurrencyException() : Exception("Channel was modified by another process.");
