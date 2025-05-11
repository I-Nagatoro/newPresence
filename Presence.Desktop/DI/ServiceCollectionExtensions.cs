using data.Domain.UseCase;
using data.RemoteData.RemoteDataBase;
using data.Repository;
using Microsoft.Extensions.DependencyInjection;
using Presence.Desktop.ViewModels;
using Presence.Desktop.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Presence.Desktop.DI
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCommonService(this IServiceCollection collection)
        {
            collection
                .AddDbContext<RemoteDatabaseContext>()
                .AddSingleton<IGroupRepository, SQLGroupRepositoryImpl>()
                .AddSingleton<IUserRepository, SQLUserRepositoryImpl>()
                .AddSingleton<IPresenceRepository, SQLPresenceRepositoryImpl>()
                .AddSingleton<PresenceUseCase>()
                .AddSingleton<UserUseCase>()
                .AddTransient<GroupUseCase>()
                .AddTransient<AttendanceViewModel>()
                .AddTransient<MainWindowViewModel>()
                .AddTransient<EditViewModel>()
                .AddTransient<AddStudentViewModel>();
        }
    }
}
