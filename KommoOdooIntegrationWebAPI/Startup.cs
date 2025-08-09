using KommoOdooIntegrationWebAPI.Configurations;
using KommoOdooIntegrationWebAPI.Services.Implementations;
using KommoOdooIntegrationWebAPI.Services.Interfaces;

namespace KommoOdooIntegrationWebAPI
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddOpenApi();
            services.AddSwaggerGen();
            services.Configure<OdooConfiguration>(_configuration.GetSection("OdooSettings"));
            services.AddHttpClient<IOdooService, OdooService>();


            services.Scan(scan
                => scan.FromAssemblyOf<Startup>()
                       .AddClasses()
                       .AsMatchingInterface()
                       .WithScopedLifetime());
        }

        public void Configure(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
