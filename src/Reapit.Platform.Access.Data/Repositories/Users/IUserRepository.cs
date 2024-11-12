using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Repositories.Users;

/// <summary>Service for the management of Users in the persistent store.</summary>
public interface IUserRepository
{
    /// <summary>Get a single user by its unique identifier.</summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user object if the identifier is found in the persistent store; otherwise null.</returns>
    public Task<User?> GetUserByIdAsync(string id, CancellationToken cancellationToken);

    /// <summary>Add a user to the change tracker.</summary>
    /// <param name="user">The user to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the operation is successful.</returns>
    public Task<bool> CreateUserAsync(User user, CancellationToken cancellationToken);

    /// <summary>Record modification of a user in the change tracker.</summary>
    /// <param name="user">The modified user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the operation is successful.</returns>
    public Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken);

    /// <summary>Record deletion of a user in the change tracker.</summary>
    /// <param name="user">The user to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the operation is successful.</returns>
    public Task<bool> DeleteUserAsync(User user, CancellationToken cancellationToken);
}