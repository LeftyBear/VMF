using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Builds Google Docs batchUpdate request DTOs from physical plans.</summary>
public sealed class GoogleDocsBatchRequestBuilder : IGoogleDocsBatchRequestBuilder
{
    private readonly IPhysicalUpdateRequestMapper mapper;

    /// <summary>Initializes a Google Docs batch request builder.</summary>
    public GoogleDocsBatchRequestBuilder()
        : this(new GoogleDocsPhysicalUpdateRequestMapper())
    {
    }

    /// <summary>Initializes a Google Docs batch request builder.</summary>
    public GoogleDocsBatchRequestBuilder(IPhysicalUpdateRequestMapper mapper)
    {
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <inheritdoc />
    public BatchUpdateDocumentRequest Build(PhysicalUpdatePlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        var batch = mapper.Map(plan);
        return new BatchUpdateDocumentRequest(
            batch.Requests,
            new BatchUpdateWriteControl(batch.RequiredRevisionId));
    }
}
