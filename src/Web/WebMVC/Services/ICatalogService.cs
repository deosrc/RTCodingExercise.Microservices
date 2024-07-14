namespace RTCodingExercise.Microservices.Services;

public interface ICatalogService
{
    Task<PagedResult<Plate>> GetPlatesAsync(CancellationToken cancellationToken = default);
    Task<OperationResult<Plate>> AddPlateAsync(NewPlate plate, CancellationToken cancellationToken = default);
}
