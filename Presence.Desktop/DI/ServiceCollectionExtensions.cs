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
using httpClient.Group;
using httpClient.Presence;
using httpClient.User;
using Microsoft.Extensions.Logging;

namespace Presence.Desktop.DI
{
    public static class ServiceColletionExtensions
    {
        public static void AddCommonService(this IServiceCollection collection)
        {
            collection
                .AddHttpClient()
                .AddLogging(logging =>
                {
                    logging.ClearProviders();
                })
                .AddTransient<IUserRepository, SQLUserRepositoryImpl>()
                .AddTransient<IGroupRepository, SQLGroupRepositoryImpl>()
                .AddTransient<IPresenceRepository, SQLPresenceRepositoryImpl>()
                .AddScoped<IGroupAPIClient, GroupAPIClient>()
                .AddScoped<IUserAPIClient, UserAPIClient>()
                .AddScoped<IPresenceAPIClient, PresenceAPIClient>()
                .AddTransient<AddStudentViewModel>()
                .AddTransient<AttendanceViewModel>()
                .AddTransient<EditViewModel>()
                .AddTransient<MainWindowViewModel>()
                .AddSingleton<GroupUseCase>()
                .AddSingleton<UserUseCase>()
                .AddSingleton<PresenceUseCase>()
                .AddSingleton<GroupAPIClient>()
                .AddSingleton<UserAPIClient>()
                .AddSingleton<PresenceAPIClient>();
    
            collection.AddHttpClient("PresenceApi", client => 
            {
                client.BaseAddress = new Uri("http://localhost:5193");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });
        }
    }
}
