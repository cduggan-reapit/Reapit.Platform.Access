﻿using MediatR;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Dummies.CreateDummy;

/// <summary>Mediator request representing a command to create a new Dummy.</summary>
/// <param name="Name">The name of the object to create.</param>
public record CreateDummyCommand(string Name) : IRequest<Dummy>;