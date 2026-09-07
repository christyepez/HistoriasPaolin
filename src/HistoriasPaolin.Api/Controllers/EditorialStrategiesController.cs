using HistoriasPaolin.Application.EditorialStrategies;
using HistoriasPaolin.Contracts.EditorialStrategies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HistoriasPaolin.Api.Controllers;

[ApiController]
public sealed class EditorialStrategiesController(IEditorialStrategyService service) : ControllerBase
{
    [HttpGet("api/channels/{channelId:guid}/editorial-strategies")]
    [Authorize(Policy = "historiaspaolin.editorial.view")]
    public async Task<ActionResult<IReadOnlyList<EditorialStrategySummaryDto>>> List(Guid channelId, CancellationToken cancellationToken) =>
        await ExecuteRead(() => service.ListAsync(channelId, cancellationToken));

    [HttpGet("api/editorial-strategies/{id:guid}")]
    [Authorize(Policy = "historiaspaolin.editorial.view")]
    public async Task<ActionResult<EditorialStrategyDetailDto>> Get(Guid id, CancellationToken cancellationToken) =>
        await ExecuteRead(() => service.GetAsync(id, cancellationToken));

    [HttpGet("api/channels/{channelId:guid}/editorial-strategy/active")]
    [Authorize(Policy = "historiaspaolin.editorial.view")]
    public async Task<ActionResult<EditorialStrategyDetailDto>> GetActive(Guid channelId, CancellationToken cancellationToken) =>
        await ExecuteRead(() => service.GetActiveAsync(channelId, cancellationToken));

    [HttpPost("api/channels/{channelId:guid}/editorial-strategies")]
    [Authorize(Policy = "historiaspaolin.editorial.manage")]
    public async Task<ActionResult<EditorialStrategyDetailDto>> Create(Guid channelId, CreateEditorialStrategyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.CreateAsync(channelId, request, Actor(), CorrelationId(), cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }
        catch (EditorialStrategyValidationException exception)
        {
            return BadRequest(ToValidationProblem(exception));
        }
        catch (EditorialStrategyChannelNotFoundException exception)
        {
            return NotFound(Problem(title: "Channel not found.", detail: exception.Message, statusCode: StatusCodes.Status404NotFound));
        }
        catch (EditorialStrategyInactiveChannelException exception)
        {
            return Conflict(Problem(title: "Channel is inactive.", detail: exception.Message, statusCode: StatusCodes.Status409Conflict));
        }
    }

    [HttpPut("api/editorial-strategies/{id:guid}")]
    [Authorize(Policy = "historiaspaolin.editorial.manage")]
    public async Task<ActionResult<EditorialStrategyDetailDto>> Update(Guid id, UpdateEditorialStrategyRequest request, CancellationToken cancellationToken) =>
        await ExecuteWrite(() => service.UpdateAsync(id, request, Actor(), CorrelationId(), cancellationToken));

    [HttpPost("api/editorial-strategies/{id:guid}/activate")]
    [Authorize(Policy = "historiaspaolin.editorial.activate")]
    public async Task<ActionResult<EditorialStrategyDetailDto>> Activate(Guid id, CancellationToken cancellationToken) =>
        await ExecuteWrite(() => service.ActivateAsync(id, Actor(), CorrelationId(), cancellationToken));

    [HttpPost("api/editorial-strategies/{id:guid}/deactivate")]
    [Authorize(Policy = "historiaspaolin.editorial.activate")]
    public async Task<ActionResult<EditorialStrategyDetailDto>> Deactivate(Guid id, CancellationToken cancellationToken) =>
        await ExecuteWrite(() => service.DeactivateAsync(id, Actor(), CorrelationId(), cancellationToken));

    [HttpGet("api/editorial-strategies/{id:guid}/pillars")]
    [Authorize(Policy = "historiaspaolin.editorial.view")]
    public async Task<ActionResult<IReadOnlyList<EditorialPillarDto>>> GetPillars(Guid id, CancellationToken cancellationToken) =>
        await ExecuteRead(() => service.GetPillarsAsync(id, cancellationToken));

    [HttpPut("api/editorial-strategies/{id:guid}/pillars")]
    [Authorize(Policy = "historiaspaolin.editorial.manage")]
    public async Task<ActionResult<IReadOnlyList<EditorialPillarDto>>> UpdatePillars(Guid id, UpdateEditorialPillarsRequest request, CancellationToken cancellationToken) =>
        await ExecuteWrite(() => service.UpdatePillarsAsync(id, request, Actor(), CorrelationId(), cancellationToken));

    [HttpGet("api/editorial-strategies/{id:guid}/topics")]
    [Authorize(Policy = "historiaspaolin.editorial.view")]
    public async Task<ActionResult<IReadOnlyList<EditorialTopicDto>>> GetTopics(Guid id, CancellationToken cancellationToken) =>
        await ExecuteRead(() => service.GetTopicsAsync(id, cancellationToken));

    [HttpPut("api/editorial-strategies/{id:guid}/topics")]
    [Authorize(Policy = "historiaspaolin.editorial.manage")]
    public async Task<ActionResult<IReadOnlyList<EditorialTopicDto>>> UpdateTopics(Guid id, UpdateEditorialTopicsRequest request, CancellationToken cancellationToken) =>
        await ExecuteWrite(() => service.UpdateTopicsAsync(id, request, Actor(), CorrelationId(), cancellationToken));

    [HttpGet("api/editorial-strategies/{id:guid}/restrictions")]
    [Authorize(Policy = "historiaspaolin.editorial.view")]
    public async Task<ActionResult<IReadOnlyList<EditorialRestrictionDto>>> GetRestrictions(Guid id, CancellationToken cancellationToken) =>
        await ExecuteRead(() => service.GetRestrictionsAsync(id, cancellationToken));

    [HttpPut("api/editorial-strategies/{id:guid}/restrictions")]
    [Authorize(Policy = "historiaspaolin.editorial.manage")]
    public async Task<ActionResult<IReadOnlyList<EditorialRestrictionDto>>> UpdateRestrictions(Guid id, UpdateEditorialRestrictionsRequest request, CancellationToken cancellationToken) =>
        await ExecuteWrite(() => service.UpdateRestrictionsAsync(id, request, Actor(), CorrelationId(), cancellationToken));

    private async Task<ActionResult<T>> ExecuteRead<T>(Func<Task<T>> action)
    {
        try
        {
            return Ok(await action());
        }
        catch (EditorialStrategyNotFoundException exception)
        {
            return NotFound(Problem(title: "Editorial strategy not found.", detail: exception.Message, statusCode: StatusCodes.Status404NotFound));
        }
        catch (EditorialStrategyActiveNotFoundException exception)
        {
            return NotFound(Problem(title: "Active editorial strategy not found.", detail: exception.Message, statusCode: StatusCodes.Status404NotFound));
        }
        catch (EditorialStrategyChannelNotFoundException exception)
        {
            return NotFound(Problem(title: "Channel not found.", detail: exception.Message, statusCode: StatusCodes.Status404NotFound));
        }
    }

    private async Task<ActionResult<T>> ExecuteWrite<T>(Func<Task<T>> action)
    {
        try
        {
            return Ok(await action());
        }
        catch (EditorialStrategyValidationException exception)
        {
            return BadRequest(ToValidationProblem(exception));
        }
        catch (EditorialStrategyNotFoundException exception)
        {
            return NotFound(Problem(title: "Editorial strategy not found.", detail: exception.Message, statusCode: StatusCodes.Status404NotFound));
        }
        catch (EditorialStrategyConcurrencyException exception)
        {
            return Conflict(Problem(title: "Concurrency conflict.", detail: exception.Message, statusCode: StatusCodes.Status409Conflict));
        }
        catch (EditorialStrategyInactiveChannelException exception)
        {
            return Conflict(Problem(title: "Channel is inactive.", detail: exception.Message, statusCode: StatusCodes.Status409Conflict));
        }
    }

    private string Actor() => User.Identity?.Name ?? User.FindFirst("sub")?.Value ?? "system";

    private static ValidationProblemDetails ToValidationProblem(EditorialStrategyValidationException exception) =>
        new(exception.Errors.ToDictionary(error => error, error => new[] { error }))
        {
            Title = "Editorial strategy validation failed.",
            Status = StatusCodes.Status400BadRequest
        };

    private string CorrelationId() =>
        HttpContext.Response.Headers.TryGetValue("X-Correlation-Id", out var correlationId)
            ? correlationId.ToString()
            : HttpContext.TraceIdentifier;
}
