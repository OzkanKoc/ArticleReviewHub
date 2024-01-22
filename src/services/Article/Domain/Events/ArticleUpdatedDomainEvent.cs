using Domain.Entities;
using MediatR;

namespace Domain.Events;

public sealed record ArticleUpdatedDomainEvent(Article Article) : INotification;
