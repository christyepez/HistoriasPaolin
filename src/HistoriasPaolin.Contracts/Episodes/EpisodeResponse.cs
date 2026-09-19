namespace HistoriasPaolin.Contracts.Episodes;

public sealed record EpisodeResponse(Guid Id, string Title, string Status, decimal EstimatedCostUsd);
