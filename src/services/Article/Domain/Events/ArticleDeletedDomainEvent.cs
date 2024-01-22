using Domain.Entities;
using MediatR;

namespace Domain.Events;

public sealed record ArticleDeletedDomainEvent(Article Article) : INotification;
