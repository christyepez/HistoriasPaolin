namespace HistoriasPaolin.Contracts.Episodes;

public sealed record CreateEpisodeRequest(string Title, decimal EstimatedCostUsd = 0m);
