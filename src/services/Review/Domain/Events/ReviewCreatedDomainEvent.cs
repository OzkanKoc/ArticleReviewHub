using Domain.Entities;
using MediatR;

namespace Domain.Events;

public sealed record ReviewCreatedDomainEvent(Review Review) : INotification;
