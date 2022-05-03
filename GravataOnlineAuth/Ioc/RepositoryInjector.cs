using Microsoft.Extensions.DependencyInjection;
using GravataOnlineAuth.Repository.User;

namespace GravataOnlineAuth.Ioc
{
    public class RepositoryInjector
    {
        public static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }

}
