namespace Catalog.API.Data.Repositories;

public class EFPlateRepository : IPlateRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<EFPlateRepository> _logger;

    public EFPlateRepository(ApplicationDbContext context, ILogger<EFPlateRepository> logger)
    {
        _dbContext = context;
        _logger = logger;
    }

    public async Task<OperationResult<Plate>> AddPlateAsync(NewPlate plate, CancellationToken cancellationToken = default)
    {
        try
        {
            // Consider adding Mapperly or similar if this becomes more complex
            var entity = new Plate()
            {
                Id = Guid.NewGuid(),
                Registration = plate.Registration,
                PurchasePrice = plate.PurchasePrice,
                SalePrice = plate.SalePrice,
                Letters = plate.Letters,
                Numbers = plate.Numbers
            };

            _dbContext.Plates.Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new()
            {
                IsSuccess = true,
                Message = "Success",
                Result = entity
            };
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to add plate to database: {InnerMessage}", ex.Message);
            return new()
            {
                IsSuccess = false,
                Message = $"Failed to add plate to database: {ex.Message}"
            };
        }
    }

    public async Task<PagedResult<Plate>> GetPlatesAsync(PagingOptions? paging = null, CancellationToken cancellationToken = default)
    {
        paging ??= new PagingOptions();
        var plates = await _dbContext.Plates.ToListAsync(cancellationToken);
        var pageInfo = new PageInfo(paging, false);
        return new PagedResult<Plate>(plates, pageInfo);
    }
}
