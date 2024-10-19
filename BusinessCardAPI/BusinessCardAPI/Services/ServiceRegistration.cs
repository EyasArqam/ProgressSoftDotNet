using BusinessCardAPI.Interfaces;

namespace BusinessCardAPI.Services
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IBusinessCardService, BusinessCardService>();
        }
    }
}
