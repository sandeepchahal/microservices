using UserService.Implementation;

namespace UserService.ServiceRegistrations;

public static class ConfigureServices
{
    public static void RegisterServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IUserManageService, UserManageService>();
    }
}