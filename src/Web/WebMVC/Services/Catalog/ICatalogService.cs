namespace RTCodingExercise.Microservices.Services.Catalog;

public interface ICatalogService
{
    Task<IEnumerable<Plate>> GetPlatesAsync(CancellationToken cancellationToken = default);
    Task<OperationResult<Plate>> AddPlateAsync(NewPlate plate, CancellationToken cancellationToken = default);
}
