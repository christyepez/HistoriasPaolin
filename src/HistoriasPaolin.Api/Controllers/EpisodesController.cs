using HistoriasPaolin.Application.Episodes;
using HistoriasPaolin.Application.Portal;
using HistoriasPaolin.Contracts.Episodes;
using HistoriasPaolin.Domain.Episodes;
using HistoriasPaolin.Domain.Integration;
using HistoriasPaolin.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HistoriasPaolin.Api.Controllers;

[ApiController]
[Route("api/historiaspaolin/episodes")]
public sealed class EpisodesController(IEpisodeRepository repository, HistoriasPaolinDbContext dbContext) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "historiaspaolin.episodes.view")]
    public async Task<ActionResult<EpisodeResponse>> Get(Guid id, CancellationToken cancellationToken)
    {
        var episode = await repository.GetAsync(id, cancellationToken);
        return episode is null
            ? NotFound()
            : Ok(new EpisodeResponse(episode.Id, episode.Title, episode.Status, episode.EstimatedCostUsd));
    }

    [HttpPost]
    [Authorize(Policy = "historiaspaolin.episodes.create")]
    public async Task<ActionResult<EpisodeResponse>> Create(CreateEpisodeRequest request, CancellationToken cancellationToken)
    {
        var episode = new Episode
        {
            Title = request.Title,
            EstimatedCostUsd = request.EstimatedCostUsd
        };

        await repository.AddAsync(episode, cancellationToken);
        dbContext.OutboxMessages.Add(new OutboxMessage
        {
            TenantId = "default",
            AggregateType = nameof(Episode),
            AggregateId = episode.Id.ToString(),
            EventType = "HistoriasPaolin.EpisodeCreated.v1",
            PayloadJson = $$"""{"episodeId":"{{episode.Id}}","title":"{{episode.Title}}"}""",
            CorrelationId = HttpContext.TraceIdentifier,
            IdempotencyKey = $"episode-created:{episode.Id}"
        });

        await repository.SaveChangesAsync(cancellationToken);
        var response = new EpisodeResponse(episode.Id, episode.Title, episode.Status, episode.EstimatedCostUsd);
        return CreatedAtAction(nameof(Get), new { id = episode.Id }, response);
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Policy = "historiaspaolin.quality.approve")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var episode = await repository.GetAsync(id, cancellationToken);
        if (episode is null)
        {
            return NotFound();
        }

        episode.Status = EpisodeStatuses.Ready;
        await repository.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
