using HistoriasPaolin.Application.Channels;
using HistoriasPaolin.Contracts.Channels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HistoriasPaolin.Api.Controllers;

[ApiController]
[Route("api/channels")]
public sealed class ChannelsController(IChannelService service) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "historiaspaolin.channels.view")]
    public async Task<ActionResult<IReadOnlyList<ChannelSummaryDto>>> List(CancellationToken cancellationToken) =>
        Ok(await service.ListAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "historiaspaolin.channels.view")]
    public async Task<ActionResult<ChannelDetailDto>> Get(Guid id, CancellationToken cancellationToken) =>
        await ExecuteRead(() => service.GetAsync(id, cancellationToken));

    [HttpGet("by-code/{code}")]
    [Authorize(Policy = "historiaspaolin.channels.view")]
    public async Task<ActionResult<ChannelDetailDto>> GetByCode(string code, CancellationToken cancellationToken) =>
        await ExecuteRead(() => service.GetByCodeAsync(code, cancellationToken));

    [HttpPost]
    [Authorize(Policy = "historiaspaolin.channels.manage")]
    public async Task<ActionResult<ChannelDetailDto>> Create(CreateChannelRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.CreateAsync(request, Actor(), CorrelationId(), cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }
        catch (ChannelValidationException exception)
        {
            return BadRequest(ToValidationProblem(exception));
        }
        catch (ChannelDuplicateCodeException exception)
        {
            return Conflict(Problem(title: "Duplicate channel code.", detail: exception.Message, statusCode: StatusCodes.Status409Conflict));
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "historiaspaolin.channels.manage")]
    public async Task<ActionResult<ChannelDetailDto>> Update(Guid id, UpdateChannelRequest request, CancellationToken cancellationToken) =>
        await ExecuteWrite(() => service.UpdateAsync(id, request, Actor(), CorrelationId(), cancellationToken));

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "historiaspaolin.channels.manage")]
    public async Task<ActionResult<ChannelDetailDto>> UpdateStatus(Guid id, UpdateChannelStatusRequest request, CancellationToken cancellationToken) =>
        await ExecuteWrite(() => service.UpdateStatusAsync(id, request, Actor(), CorrelationId(), cancellationToken));

    [HttpGet("{id:guid}/brand")]
    [Authorize(Policy = "historiaspaolin.channels.view")]
    public async Task<ActionResult<ChannelBrandDto>> GetBrand(Guid id, CancellationToken cancellationToken) =>
        await ExecuteRead(() => service.GetBrandAsync(id, cancellationToken));

    [HttpPut("{id:guid}/brand")]
    [Authorize(Policy = "historiaspaolin.channels.manage")]
    public async Task<ActionResult<ChannelBrandDto>> UpdateBrand(Guid id, UpdateChannelBrandRequest request, CancellationToken cancellationToken) =>
        await ExecuteWrite(() => service.UpdateBrandAsync(id, request, Actor(), CorrelationId(), cancellationToken));

    private async Task<ActionResult<T>> ExecuteRead<T>(Func<Task<T>> action)
    {
        try
        {
            return Ok(await action());
        }
        catch (ChannelNotFoundException exception)
        {
            return NotFound(Problem(title: "Channel not found.", detail: exception.Message, statusCode: StatusCodes.Status404NotFound));
        }
        catch (ChannelCodeNotFoundException exception)
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
        catch (ChannelValidationException exception)
        {
            return BadRequest(ToValidationProblem(exception));
        }
        catch (ChannelDuplicateCodeException exception)
        {
            return Conflict(Problem(title: "Duplicate channel code.", detail: exception.Message, statusCode: StatusCodes.Status409Conflict));
        }
        catch (ChannelConcurrencyException exception)
        {
            return Conflict(Problem(title: "Concurrency conflict.", detail: exception.Message, statusCode: StatusCodes.Status409Conflict));
        }
        catch (ChannelNotFoundException exception)
        {
            return NotFound(Problem(title: "Channel not found.", detail: exception.Message, statusCode: StatusCodes.Status404NotFound));
        }
    }

    private string Actor() => User.Identity?.Name ?? User.FindFirst("sub")?.Value ?? "system";

    private static ValidationProblemDetails ToValidationProblem(ChannelValidationException exception) =>
        new(exception.Errors.ToDictionary(error => error, error => new[] { error }))
        {
            Title = "Channel validation failed.",
            Status = StatusCodes.Status400BadRequest
        };

    private string CorrelationId() =>
        HttpContext.Response.Headers.TryGetValue("X-Correlation-Id", out var correlationId)
            ? correlationId.ToString()
            : HttpContext.TraceIdentifier;
}
