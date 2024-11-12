using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Access.Data.Context;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Repositories.Users;

/// <inheritdoc/>
public class UserRepository : IUserRepository
{
    private readonly AccessDbContext _context;

    /// <summary>Initializes a new instance of the <see cref="UserRepository"/> class.</summary>
    /// <param name="context">The database context.</param>
    public UserRepository(AccessDbContext context)
        => _context = context;

    /// <inheritdoc/>
    public async Task<User?> GetUserByIdAsync(string id, CancellationToken cancellationToken) 
        => await _context.Users
            .SingleOrDefaultAsync(user => user.Id == id, cancellationToken);

    /// <inheritdoc/>
    public async Task<bool> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        _ = await _context.Users.AddAsync(user, cancellationToken);
        return true;
    }

    /// <inheritdoc/>
    public Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        _context.Users.Update(user);
        return Task.FromResult(true);
    }

    /// <inheritdoc/>
    public Task<bool> DeleteUserAsync(User user, CancellationToken cancellationToken)
    {
        _context.Users.Remove(user);
        return Task.FromResult(true);
    }
}