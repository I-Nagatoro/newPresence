using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using httpClient.Group;
using httpClient.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Presence.Desktop.DI;
using Presence.Desktop.ViewModels;
using Presence.Desktop.Views;

namespace Presence.Desktop
{
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // Создаём коллекцию сервисов
            var serviceCollection = new ServiceCollection();

            // Добавляем все сервисы, в том числе GroupAPIClient и UserAPIClient
            serviceCollection.AddCommonService();

            // Добавляем логирование
            serviceCollection.AddLogging(builder => 
            {
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            // Строим контейнер зависимостей
            ServiceProvider = serviceCollection.BuildServiceProvider();

            // Получаем GroupAPIClient и UserAPIClient после построения ServiceProvider
            var groupAPIClient = ServiceProvider.GetService<GroupAPIClient>();
            var userAPIClient = ServiceProvider.GetService<UserAPIClient>();

            // Инициализируем главное окно
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow(groupAPIClient, userAPIClient)
                {
                    DataContext = new MainWindowViewModel(groupAPIClient, userAPIClient),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}