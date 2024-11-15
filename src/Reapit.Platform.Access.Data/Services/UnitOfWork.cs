using Reapit.Platform.Access.Data.Context;
using Reapit.Platform.Access.Data.Repositories.Groups;
using Reapit.Platform.Access.Data.Repositories.Organisations;
using Reapit.Platform.Access.Data.Repositories.Roles;
using Reapit.Platform.Access.Data.Repositories.Users;

namespace Reapit.Platform.Access.Data.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly AccessDbContext _context;
    
    private IUserRepository? _userRepository;
    private IOrganisationRepository? _organisationRepository;
    private IGroupRepository? _groupRepository;
    private IRoleRepository? _roleRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public UnitOfWork(AccessDbContext context)
        => _context = context;
    
    /// <inheritdoc />
    public IUserRepository Users 
        => _userRepository ??= new UserRepository(_context);

    /// <inheritdoc/>
    public IOrganisationRepository Organisations
        => _organisationRepository ??= new OrganisationRepository(_context);
    
    /// <inheritdoc/>
    public IGroupRepository Groups
        => _groupRepository ??= new GroupRepository(_context);
    
    /// <inheritdoc/>
    public IRoleRepository Roles
        => _roleRepository ??= new RoleRepository(_context);
    
    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await _context.SaveChangesAsync(cancellationToken);
}