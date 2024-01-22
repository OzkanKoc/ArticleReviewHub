using Domain.Entities;
using MediatR;

namespace Domain.Events;

public sealed record ReviewUpdatedDomainEvent(Review Review) : INotification;
