namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Provides short-lived credentials for Google services.</summary>
public interface IGoogleCredentialProvider
{
    /// <summary>Gets a valid Google credential.</summary>
    /// <param name="cancellationToken">A token that cancels the operation.</param>
    /// <returns>A valid credential.</returns>
    Task<GoogleCredential> GetCredentialAsync(CancellationToken cancellationToken);
}
