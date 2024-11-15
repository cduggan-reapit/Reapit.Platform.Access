﻿using Reapit.Platform.Access.Domain.Entities.Abstract;

namespace Reapit.Platform.Access.Data.Repositories;

public interface IBaseRepository<T> 
    where T: EntityBase
{
    /// <summary>Add an instance of <typeparamref name="T"/> to the context change tracker.</summary>
    /// <param name="entity">The entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task<T> CreateAsync(T entity, CancellationToken cancellationToken);
    
    /// <summary>Mark an instance of <typeparamref name="T"/> as modified in the context change tracker.</summary>
    /// <param name="entity">The entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task<T> UpdateAsync(T entity, CancellationToken cancellationToken);
    
    /// <summary>Mark an instance of <typeparamref name="T"/> as deleted in the context change tracker.</summary>
    /// <param name="entity">The entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task<T> DeleteAsync(T entity, CancellationToken cancellationToken);
}