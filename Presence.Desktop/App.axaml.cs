using System;
using Presence.Desktop.ViewModels;
using Presence.Desktop.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using data.Domain.UseCase;
using Microsoft.Extensions.DependencyInjection;
using Presence.Desktop.DI;

namespace Presence.Desktop
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCommonService();
            serviceCollection.AddSingleton<GroupUseCase>();
            serviceCollection.AddSingleton<UserUseCase>();
            Services = serviceCollection.BuildServiceProvider();

            var mainViewModel = Services.GetRequiredService<MainWindowViewModel>();
            var groupUseCase  = Services.GetRequiredService<GroupUseCase>();
            var userUseCase   = Services.GetRequiredService<UserUseCase>();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow(groupUseCase, userUseCase)
                {
                    DataContext = mainViewModel
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}