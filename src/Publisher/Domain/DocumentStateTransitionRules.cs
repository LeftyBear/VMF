namespace Vmf.Publisher.Domain;

/// <summary>Defines persisted document-state transitions independently of orchestration.</summary>
public static class DocumentStateTransitionRules
{
    /// <summary>Determines whether a persisted state may transition to the requested state.</summary>
    /// <param name="current">The current state, or <see langword="null"/> when no state exists.</param>
    /// <param name="next">The requested persisted state.</param>
    /// <returns><see langword="true"/> when the transition is allowed.</returns>
    public static bool IsAllowed(DocumentState? current, DocumentState next) => current switch
    {
        null => next == DocumentState.Active,
        DocumentState.Active => true,
        DocumentState.Missing => next is DocumentState.Missing or DocumentState.Active or DocumentState.Archived,
        DocumentState.Archived => next == DocumentState.Archived,
        _ => false,
    };
}
