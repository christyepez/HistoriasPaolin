using HistoriasPaolin.Contracts.EditorialStrategies;

namespace HistoriasPaolin.Application.EditorialStrategies;

public interface IEditorialStrategyService
{
    Task<IReadOnlyList<EditorialStrategySummaryDto>> ListAsync(Guid channelId, CancellationToken cancellationToken);
    Task<EditorialStrategyDetailDto> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<EditorialStrategyDetailDto> GetActiveAsync(Guid channelId, CancellationToken cancellationToken);
    Task<EditorialStrategyDetailDto> CreateAsync(Guid channelId, CreateEditorialStrategyRequest request, string actor, string correlationId, CancellationToken cancellationToken);
    Task<EditorialStrategyDetailDto> UpdateAsync(Guid id, UpdateEditorialStrategyRequest request, string actor, string correlationId, CancellationToken cancellationToken);
    Task<EditorialStrategyDetailDto> ActivateAsync(Guid id, string actor, string correlationId, CancellationToken cancellationToken);
    Task<EditorialStrategyDetailDto> DeactivateAsync(Guid id, string actor, string correlationId, CancellationToken cancellationToken);
    Task<IReadOnlyList<EditorialPillarDto>> GetPillarsAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<EditorialPillarDto>> UpdatePillarsAsync(Guid id, UpdateEditorialPillarsRequest request, string actor, string correlationId, CancellationToken cancellationToken);
    Task<IReadOnlyList<EditorialTopicDto>> GetTopicsAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<EditorialTopicDto>> UpdateTopicsAsync(Guid id, UpdateEditorialTopicsRequest request, string actor, string correlationId, CancellationToken cancellationToken);
    Task<IReadOnlyList<EditorialRestrictionDto>> GetRestrictionsAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<EditorialRestrictionDto>> UpdateRestrictionsAsync(Guid id, UpdateEditorialRestrictionsRequest request, string actor, string correlationId, CancellationToken cancellationToken);
}
