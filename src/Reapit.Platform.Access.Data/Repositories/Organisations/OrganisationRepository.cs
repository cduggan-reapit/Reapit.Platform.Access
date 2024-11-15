﻿using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Access.Data.Context;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Repositories.Organisations;

/// <inheritdoc/>
public class OrganisationRepository : IOrganisationRepository
{
    private readonly AccessDbContext _context;

    /// <summary>Initializes a new instance of the <see cref="OrganisationRepository"/> class.</summary>
    /// <param name="context">The database context.</param>
    public OrganisationRepository(AccessDbContext context)
        => _context = context;

    /// <inheritdoc/>
    public async Task<Organisation?> GetOrganisationByIdAsync(string id, CancellationToken cancellationToken) 
        => await _context.Organisations
            .Include(o => o.Users)
            .SingleOrDefaultAsync(organisation => organisation.Id == id, cancellationToken);

    /// <inheritdoc/>
    public async Task<bool> CreateOrganisationAsync(Organisation organisation, CancellationToken cancellationToken)
    {
        _ = await _context.Organisations.AddAsync(organisation, cancellationToken);
        return true;
    }

    /// <inheritdoc/>
    public Task<bool> UpdateOrganisationAsync(Organisation organisation, CancellationToken cancellationToken)
    {
        _context.Organisations.Update(organisation);
        return Task.FromResult(true);
    }

    /// <inheritdoc/>
    public Task<bool> DeleteOrganisationAsync(Organisation organisation, CancellationToken cancellationToken)
    {
        _context.Organisations.Remove(organisation);
        return Task.FromResult(true);
    }
}