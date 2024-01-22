using Api.Models.Request;
using Api.Models.Response;
using Application.Commands.Reviews.Create;
using Application.Commands.Reviews.Delete;
using Application.Commands.Reviews.Update;
using Application.Queries.Reviews.GetAll;
using Application.Queries.Reviews.GetById;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/reviews")]
public class ReviewController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Get Reviews
    /// </summary>
    [HttpGet]
    [ProducesResponseType<GetReviewsResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReviews()
    {
        var result = await sender.Send(new GetReviewsQuery());
        return Ok(result.Adapt<GetReviewsResponse>());
    }

    /// <summary>
    /// Get Review By Id
    /// </summary>
    /// <param name="id">id</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType<GetReviewResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReview(int id)
    {
        var result = await sender.Send(new GetReviewByIdQuery(id));
        return Ok(result.Adapt<GetReviewResponse>());
    }

    /// <summary>
    /// Delete Review
    /// </summary>
    /// <param name="id">id</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteReview(int id)
    {
        await sender.Send(new DeleteReviewCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Update Review
    /// </summary>
    /// <param name="id">id</param>
    /// <param name="request"></param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateReview(int id, UpdateReviewRequest request)
    {
        await sender.Send(new UpdateReviewCommand(
            id,
            request.ArticleId,
            request.Reviewer,
            request.ReviewerContent));

        return NoContent();
    }

    /// <summary>
    /// Create Review
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateReview(CreateReviewRequest request)
    {
        var result = await sender.Send(new CreateReviewCommand(
            request.ArticleId,
            request.Reviewer,
            request.ReviewerContent));

        return CreatedAtAction(nameof(GetReview), new
        {
            id = result.Id
        }, result.Adapt<GetReviewResponse>());
    }
}
