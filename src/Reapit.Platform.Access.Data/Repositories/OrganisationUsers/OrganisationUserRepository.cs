using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Access.Data.Context;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Repositories.OrganisationUsers;

/// <inheritdoc />
public class OrganisationUserRepository : IOrganisationUserRepository
{
    private readonly AccessDbContext _context;

    /// <summary>Initializes a new instance of the <see cref="OrganisationUserRepository"/> class.</summary>
    /// <param name="context">The database context.</param>
    public OrganisationUserRepository(AccessDbContext context)
        => _context = context;

    /// <inheritdoc/>
    public async Task<OrganisationUser?> GetRelationshipByIdAsync(int id, CancellationToken cancellationToken)
        => await _context.OrganisationUsers
            .SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);

    /// <inheritdoc/>
    public async Task<OrganisationUser?> GetRelationshipAsync(string organisationId, string userId, CancellationToken cancellationToken)
        => await _context.OrganisationUsers
            .SingleOrDefaultAsync(entity => entity.OrganisationId == organisationId && entity.UserId == userId, cancellationToken);

    /// <inheritdoc/>
    public async Task<bool> CreateRelationshipAsync(OrganisationUser entity, CancellationToken cancellationToken)
    {
        _ = await _context.OrganisationUsers.AddAsync(entity, cancellationToken);
        return true;
    }

    /// <inheritdoc/>
    public Task<bool> DeleteRelationshipAsync(OrganisationUser entity, CancellationToken cancellationToken)
    {
        _context.OrganisationUsers.Remove(entity);
        return Task.FromResult(true);
    }
}