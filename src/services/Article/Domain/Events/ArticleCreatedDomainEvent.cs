using Domain.Entities;
using MediatR;

namespace Domain.Events;

public sealed record ArticleCreatedDomainEvent(Article Article) : INotification;
