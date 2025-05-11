using Avalonia.Controls;
using Avalonia.Interactivity;
using data.Domain.UseCase;
using Presence.Desktop.ViewModels;
using data.domain.Models;

namespace Presence.Desktop.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(GroupUseCase groupUseCase, UserUseCase userUseCase)
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(groupUseCase, userUseCase);
        }

        private void OnDeleteUserClick(object sender, RoutedEventArgs e)
        {
            var viewModel = (MainWindowViewModel)DataContext;
            viewModel.OnDeleteUserClick();
        }

        private void OnEditUserClick(object sender, RoutedEventArgs e)
        {
            var viewModel = (MainWindowViewModel)DataContext;
            viewModel.OnEditUserClick();
        }
    }

}