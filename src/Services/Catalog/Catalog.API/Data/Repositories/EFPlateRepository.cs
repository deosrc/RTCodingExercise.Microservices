namespace Catalog.API.Data.Repositories;

public class EFPlateRepository : IPlateRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EFPlateRepository(ApplicationDbContext context)
    {
        _dbContext = context;
    }

    public async Task<IEnumerable<Plate>> GetPlatesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Plates.ToListAsync(cancellationToken);
    }
}
