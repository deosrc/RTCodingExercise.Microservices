using Newtonsoft.Json;

namespace Catalog.API.Data
{
    public class ApplicationDbContextSeed
    {
        public async Task SeedAsync(ApplicationDbContext context, IWebHostEnvironment env, ILogger<ApplicationDbContextSeed> logger, IOptions<AppSettings> settings, int? retry = 0)
        {
            int retryForAvaiability = retry.Value;

            try
            {
                // If there is already data in the database, skip seeding the database.
                // Adding data which already exists causes exceptions and significantly impacts performance.
                var hasData = await context.Plates.AnyAsync();
                if (hasData)
                {
                    logger.LogWarning("Data already exists in the database. Skipping data seeding.");
                    return;
                }

                await SeedCustomData(context, env, logger);
            }
            catch (Exception ex)
            {
                // used for initilisaton of docker containers
                if (retryForAvaiability < 10)
                {
                    retryForAvaiability++;

                    logger.LogError(ex, $"There is an error migrating data for ApplicationDbContext");

                    await SeedAsync(context, env, logger, settings, retryForAvaiability);
                }
            }
        }

        public async Task SeedCustomData(ApplicationDbContext context, IWebHostEnvironment env, ILogger<ApplicationDbContextSeed> logger)
        {
            try
            {
                var plates = ReadApplicationRoleFromJson(env.ContentRootPath, logger);

                await context.Plates.AddRangeAsync(plates);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public List<Plate> ReadApplicationRoleFromJson(string contentRootPath, ILogger<ApplicationDbContextSeed> logger)
        {
            string filePath = Path.Combine(contentRootPath, "Setup", "plates.json");
            string json = File.ReadAllText(filePath);

            var plates = JsonConvert.DeserializeObject<List<Plate>>(json) ?? new List<Plate>();

            return plates;
        }
    }
}
