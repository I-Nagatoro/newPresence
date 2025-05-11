using Avalonia.Controls;
using Avalonia.Interactivity;
using data.RemoteData.RemoteDatabase.DAO;
using System.Collections.Generic;
using System.Threading.Tasks;
using data.Domain.UseCase;
using Microsoft.Extensions.DependencyInjection;
using Presence.Desktop.ViewModels;

namespace Presence.Desktop.Views
{
    public partial class EditUserDialog : Window
    {
        private GroupUseCase _groupUseCase;
        private UserUseCase _userUseCase;
        public EditUserDialog()
        {
            InitializeComponent();
        }

        public EditUserDialog(GroupUseCase groupUseCase, UserUseCase userUseCase, int currentUserId,
            string currentName,
            int currentGroupId,
            List<GroupDAO> groups)
            : this()
        {
            _idTextBlock = this.FindControl<TextBlock>("_idTextBlock");
            _nameTextBox = this.FindControl<TextBox>("_nameTextBox");
            _groupComboBox = this.FindControl<ComboBox>("_groupComboBox");
            
            var viewModel = new EditViewModel(groupUseCase, userUseCase);
            viewModel.CloseAction = this.Close;
            DataContext = viewModel;
            viewModel.UserId = currentUserId;
            viewModel.FIO = currentName;
            viewModel.GroupId = currentGroupId;
            
            _idTextBlock.Text = currentUserId.ToString();
            _groupComboBox.ItemsSource = groups;
            _groupComboBox.SelectedItem=groups.Find(g => g.Id == currentGroupId);
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            Close((default(string), default(GroupDAO)));
        }
        

        public Task<(string, GroupDAO)> ShowEditDialog(Window parent) =>
            this.ShowDialog<(string, GroupDAO)>(parent);
    }
}