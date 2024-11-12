namespace Reapit.Platform.Access.Domain.Entities.Abstract;

/// <summary>Base class for entities maintained by external services (e.g. Organisations).</summary>
public abstract class RemoteEntityBase
{
    /// <summary>The date and time on which the record was last synchronised from the remote service.</summary>
    public DateTimeOffset? DateLastSynchronised { get; set; }
}