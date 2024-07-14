using Paging.Domain;

namespace RTCodingExercise.Microservices.Services;

public interface ICatalogService
{
    Task<PagedResult<Plate>> GetPlatesAsync(PagingOptions? paging = null, CancellationToken cancellationToken = default);
    Task<OperationResult<Plate>> AddPlateAsync(NewPlate plate, CancellationToken cancellationToken = default);
}
