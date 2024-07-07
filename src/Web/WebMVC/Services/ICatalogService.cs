namespace RTCodingExercise.Microservices.Services;

public interface ICatalogService
{
    Task<IEnumerable<Plate>> GetPlatesAsync(CancellationToken cancellationToken = default);
    Task<OperationResult<Plate>> AddPlateAsync(Plate plate, CancellationToken cancellationToken = default);
}
