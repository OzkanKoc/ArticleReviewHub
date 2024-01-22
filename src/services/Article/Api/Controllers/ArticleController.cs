using Api.Models.Article.Request;
using Api.Models.Article.Response;
using Application.Commands.Article.Create;
using Application.Commands.Article.Delete;
using Application.Commands.Article.Update;
using Application.Queries.Articles.GetAll;
using Application.Queries.Articles.GetById;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/articles")]
public class ArticleController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Get Articles
    /// </summary>
    [HttpGet]
    [ProducesResponseType<GetArticlesResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetArticles()
    {
        var result = await sender.Send(new GetArticlesQuery());
        return Ok(result.Adapt<GetArticlesResponse>());
    }

    /// <summary>
    /// Get Article By Id
    /// </summary>
    /// <param name="id">id</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType<GetArticleResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetArticle(int id)
    {
        var result = await sender.Send(new GetArticleByIdQuery(id));
        return Ok(result.Adapt<GetArticleResponse>());
    }

    /// <summary>
    /// Delete Article
    /// </summary>
    /// <param name="id">id</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        await sender.Send(new DeleteArticleCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Update Article
    /// </summary>
    /// <param name="id">id</param>
    /// <param name="request"></param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateArticle(int id, UpdateArticleRequest request)
    {
        await sender.Send(new UpdateArticleCommand(
            id,
            request.Title,
            request.Author,
            request.ArticleContent,
            request.PublishDate,
            request.StarCount));

        return NoContent();
    }

    /// <summary>
    /// Create Article
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateArticle(CreateArticleRequest request)
    {
        var result = await sender.Send(new CreateArticleCommand(
            request.Title,
            request.Author,
            request.ArticleContent,
            request.PublishDate,
            request.StarCount));

        return CreatedAtAction(nameof(GetArticle), new { id = result.Id }, result.Adapt<GetArticleResponse>());
    }
}
