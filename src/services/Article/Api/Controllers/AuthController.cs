using Api.Models.Auth.Request;
using Api.Models.Auth.Response;
using Application.Commands.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Auth
    /// </summary>
    /// <param name="request"></param>
    [HttpPost]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Auth(AuthRequest request)
    {
        var result = await sender.Send(new AuthenticateCommand(request.ApiKey, request.ApiSecret));
        return Ok(new AuthResponse(result));
    }
}
