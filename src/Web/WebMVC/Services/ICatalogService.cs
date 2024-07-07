namespace RTCodingExercise.Microservices.Services;

public interface ICatalogService
{
    Task<IEnumerable<Plate>> GetPlatesAsync(CancellationToken cancellationToken = default);
}
