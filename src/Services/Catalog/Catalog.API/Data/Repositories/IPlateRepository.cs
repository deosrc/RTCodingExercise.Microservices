using System.Runtime.CompilerServices;

namespace Catalog.API.Data.Repositories;

public interface IPlateRepository
{
    Task<IEnumerable<Plate>> GetPlatesAsync(CancellationToken cancellationToken = default);

    Task<OperationResult<Plate>> AddPlateAsync(Plate plate, CancellationToken cancellationToken = default);
}
