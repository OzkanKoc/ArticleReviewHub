using Domain.Entities;
using MediatR;

namespace Domain.Events;

public sealed record ReviewDeletedDomainEvent(Review Review) : INotification;
