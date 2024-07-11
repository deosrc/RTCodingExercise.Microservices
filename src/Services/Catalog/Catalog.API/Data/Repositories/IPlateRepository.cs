using System.Runtime.CompilerServices;

namespace Catalog.API.Data.Repositories;

public interface IPlateRepository
{
    Task<PagedResult<Plate>> GetPlatesAsync(PagingOptions? paging = null, CancellationToken cancellationToken = default);

    Task<OperationResult<Plate>> AddPlateAsync(NewPlate plate, CancellationToken cancellationToken = default);
}
