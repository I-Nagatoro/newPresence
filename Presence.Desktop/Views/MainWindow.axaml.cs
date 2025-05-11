using Avalonia.Controls;
using Avalonia.Interactivity;
using data.Domain.UseCase;
using Presence.Desktop.ViewModels;
using data.domain.Models;
using httpClient.Group;
using httpClient.Presence;
using httpClient.User;

namespace Presence.Desktop.Views
{
    public partial class MainWindow : Window
    {
        private GroupAPIClient _groupAPIClient;
        private PresenceAPIClient _presenceAPIClient;
        private UserAPIClient _userAPIClient;
        public MainWindow(GroupAPIClient _groupAPIClient, UserAPIClient _userAPIClient)
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(_groupAPIClient, _userAPIClient);
        }
    }

}