using Application.Common.Repositories;
using Application.Common.Token;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.Auth;

public sealed record AuthenticateCommand(string ApiKey, string ApiSecret) : IRequest<string>;

internal sealed class AuthenticateCommandHandler(IRepository<Identity> repository, ITokenProvider tokenProvider) : IRequestHandler<AuthenticateCommand, string>
{
    public async Task<string> Handle(AuthenticateCommand request, CancellationToken cancellationToken)
    {
        var identity = await repository.Table.AsNoTracking().FirstOrDefaultAsync(x => x.ApiKey == request.ApiKey, cancellationToken);

        if (identity is null || identity.ApiSecret != request.ApiSecret)
        {
            throw new CustomException(ErrorType.Unauthorized);
        }

        return tokenProvider.GenerateToken(identity.ApiKey, identity.Id);
    }
}
