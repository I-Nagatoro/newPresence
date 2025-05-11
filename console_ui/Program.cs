using data.RemoteData.RemoteDataBase;
using data.Repository;
using data.Domain.UseCase;
using Microsoft.Extensions.DependencyInjection;
using ui;


IServiceCollection services = new ServiceCollection();

services
    .AddDbContext<RemoteDatabaseContext>()
    .AddSingleton<IGroupRepository, SQLGroupRepositoryImpl>()
    .AddSingleton<IUserRepository, SQLUserRepositoryImpl>()
    .AddSingleton<IPresenceRepository, SQLPresenceRepositoryImpl>()
    .AddSingleton<UserUseCase>()
    .AddSingleton<GroupUseCase>()
    .AddSingleton<PresenceUseCase>()
    .AddSingleton<MainMenuUI>();



var serviceProvider = services.BuildServiceProvider();
MainMenuUI mainMenuUI = serviceProvider.GetService<MainMenuUI>();
mainMenuUI.Start();
