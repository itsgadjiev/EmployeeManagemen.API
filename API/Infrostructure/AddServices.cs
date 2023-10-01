using API.Database;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace API.Infrostructure
{
    public static class AddServices
    {
        public static void AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddDbContext<AppDbContext>(opt =>
             {
                 string connectionStr = configuration.GetConnectionString("sql");
                 opt.UseNpgsql(connectionStr);
             });
        }
    }
}
