using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Repositories.Organisations;

/// <summary>Service for the management of Organisations in the persistent store.</summary>
public interface IOrganisationRepository
{
    /// <summary>Get a single organisation by its unique identifier.</summary>
    /// <param name="id">The unique identifier of the organisation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The organisation object if the identifier is found in the persistent store; otherwise null.</returns>
    public Task<Organisation?> GetOrganisationByIdAsync(string id, CancellationToken cancellationToken);

    /// <summary>Add a organisation to the change tracker.</summary>
    /// <param name="organisation">The organisation to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the operation is successful.</returns>
    public Task<bool> CreateOrganisationAsync(Organisation organisation, CancellationToken cancellationToken);

    /// <summary>Record modification of a organisation in the change tracker.</summary>
    /// <param name="organisation">The modified organisation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the operation is successful.</returns>
    public Task<bool> UpdateOrganisationAsync(Organisation organisation, CancellationToken cancellationToken);

    /// <summary>Record deletion of a organisation in the change tracker.</summary>
    /// <param name="organisation">The organisation to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the operation is successful.</returns>
    public Task<bool> DeleteOrganisationAsync(Organisation organisation, CancellationToken cancellationToken);

    /// <summary>Adds a user to an organisation.</summary>
    /// <param name="member">The organisation-user relationship object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task<bool> AddMemberAsync(OrganisationUser member, CancellationToken cancellationToken);

    /// <summary>Removes a user from an organisation.</summary>
    /// <param name="member">The organisation-user relationship object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task<bool> RemoveMemberAsync(OrganisationUser member, CancellationToken cancellationToken);
}