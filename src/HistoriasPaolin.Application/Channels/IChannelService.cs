using HistoriasPaolin.Contracts.Channels;

namespace HistoriasPaolin.Application.Channels;

public interface IChannelService
{
    Task<IReadOnlyList<ChannelSummaryDto>> ListAsync(CancellationToken cancellationToken);
    Task<ChannelDetailDto> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<ChannelDetailDto> GetByCodeAsync(string code, CancellationToken cancellationToken);
    Task<ChannelDetailDto> CreateAsync(CreateChannelRequest request, string actor, string correlationId, CancellationToken cancellationToken);
    Task<ChannelDetailDto> UpdateAsync(Guid id, UpdateChannelRequest request, string actor, string correlationId, CancellationToken cancellationToken);
    Task<ChannelDetailDto> UpdateStatusAsync(Guid id, UpdateChannelStatusRequest request, string actor, string correlationId, CancellationToken cancellationToken);
    Task<ChannelBrandDto> GetBrandAsync(Guid id, CancellationToken cancellationToken);
    Task<ChannelBrandDto> UpdateBrandAsync(Guid id, UpdateChannelBrandRequest request, string actor, string correlationId, CancellationToken cancellationToken);
}
