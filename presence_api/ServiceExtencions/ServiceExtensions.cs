using data.Domain.UseCase;
using data.Repository;

public static class ServiceExtensions
{
    public static void ConfigurateRepositories(this IServiceCollection services)
    {
        services.AddScoped<IGroupRepository, SQLGroupRepositoryImpl>();
        services.AddScoped<IUserRepository, SQLUserRepositoryImpl>();
        services.AddScoped<IPresenceRepository, SQLPresenceRepositoryImpl>();
    }

    public static void ConfigurateGroupUseCase(this IServiceCollection services)
    {
        services.AddScoped<GroupUseCase>();
    }

    public static void ConfigurateUserUseCase(this IServiceCollection services)
    {
        services.AddScoped<UserUseCase>();
    }

    public static void ConfiguratePresenceUseCase(this IServiceCollection services)
    {
        services.AddScoped<PresenceUseCase>();
    }

    public static void ConfigurateAPIUseCase(this IServiceCollection services)
    {
        services.AddScoped<APIUseCase>();
    }

    public static void ConfigurateAdminPanel(this IServiceCollection services)
    {
        services.ConfigurateRepositories();
        services.AddScoped<GroupUseCase>();
        services.AddScoped<UserUseCase>();
        services.AddScoped<PresenceUseCase>();
        services.AddScoped<APIUseCase>();
    }
}