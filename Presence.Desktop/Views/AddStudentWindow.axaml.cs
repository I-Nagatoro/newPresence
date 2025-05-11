using Avalonia.Controls;
using data.Domain.UseCase;
using data.RemoteData.RemoteDatabase.DAO;
using Presence.Desktop.ViewModels;

namespace Presence.Desktop.Views
{
    public partial class AddStudentWindow : Window
    {
        public AddStudentWindow()
        {
            InitializeComponent();
        }

        public AddStudentWindow(UserUseCase userUseCase, GroupUseCase groupUseCase, GroupDAO preselectedGroup)
        {
            InitializeComponent();

            var viewModel = new AddStudentViewModel(userUseCase, groupUseCase, preselectedGroup);
            viewModel.CloseAction = () => this.Close();
            DataContext = viewModel;
        }
    }
}