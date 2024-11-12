using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Repositories.OrganisationUsers;

/// <summary>Service for the management of organisation-user relationships in the persistent store.</summary>

public interface IOrganisationUserRepository
{
    /// <summary>Get an organisation-user relationship by it's unique identifier.</summary>
    /// <param name="id">The unique identifier of the relationship.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<OrganisationUser?> GetRelationshipByIdAsync(int id, CancellationToken cancellationToken);
    
    /// <summary>Get an organisation-user relationship by it's relation identifiers.</summary>
    /// <param name="organisationId">The unique identifier of the organisation.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<OrganisationUser?> GetRelationshipAsync(string organisationId, string userId, CancellationToken cancellationToken);
    
    /// <summary>Add a new relationship to the change tracker.</summary>
    /// <param name="entity">The relationship.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<bool> CreateRelationshipAsync(OrganisationUser entity, CancellationToken cancellationToken);
    
    /// <summary>Remove a relationship to the change tracker.</summary>
    /// <param name="entity">The relationship.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<bool> DeleteRelationshipAsync(OrganisationUser entity, CancellationToken cancellationToken);
}