using Avalonia.Controls;
using data.RemoteData.RemoteDatabase.DAO;
using httpClient.Group;
using httpClient.User;
using Presence.Desktop.ViewModels;

namespace Presence.Desktop.Views
{
    public partial class AddStudentWindow : Window
    {
        public AddStudentWindow()
        {
            InitializeComponent();
        }

        public AddStudentWindow(UserAPIClient userClient, GroupAPIClient groupClient, GroupDAO preselectedGroup)
        {
            InitializeComponent();

            var viewModel = new AddStudentViewModel(userClient, groupClient, preselectedGroup);
            viewModel.CloseAction = Close;
            DataContext = viewModel;
        }
    }
}