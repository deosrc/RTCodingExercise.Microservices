using System.Runtime.CompilerServices;

namespace Catalog.API.Data.Repositories;

public interface IPlateRepository
{
    Task<IEnumerable<Plate>> GetPlatesAsync(CancellationToken cancellationToken);
}
