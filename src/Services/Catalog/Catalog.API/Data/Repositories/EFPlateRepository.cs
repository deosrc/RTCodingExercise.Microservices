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

    public async Task<OperationResult<Plate>> AddPlateAsync(Plate plate, CancellationToken cancellationToken = default)
    {
        try
        {
            // Ensure a new ID is always generated
            plate.Id = Guid.NewGuid();

            _dbContext.Plates.Add(plate);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new()
            {
                IsSuccess = true,
                Message = "Success",
                Result = plate
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

    public async Task<IEnumerable<Plate>> GetPlatesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Plates.ToListAsync(cancellationToken);
    }
}
