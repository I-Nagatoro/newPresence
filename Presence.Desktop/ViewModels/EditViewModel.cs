using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using data.RemoteData.RemoteDatabase.DAO;
using httpClient.Group;
using httpClient.User;
using ReactiveUI;

namespace Presence.Desktop.ViewModels
{
    public class EditViewModel : ReactiveObject
    {
        private readonly GroupAPIClient _groupApiClient;
        private readonly UserAPIClient _userApiClient;

        private ObservableCollection<GroupDAO> _groups;
        public ObservableCollection<GroupDAO> Groups => _groups;

        private GroupDAO? _selectedGroupItem;
        public ReactiveCommand<Unit, Unit> UpdateUserCommand { get; }

        public EditViewModel(GroupAPIClient groupApiClient, UserAPIClient userApiClient)
        {
            _groupApiClient = groupApiClient;
            _userApiClient = userApiClient;

            _groups = new ObservableCollection<GroupDAO>();

            LoadGroups();

            UpdateUserCommand = ReactiveCommand.CreateFromTask(
                UpdateUserAsync,
                this.WhenAnyValue(
                    x => x.UserId,
                    x => x.FIO,
                    x => x.GroupId,
                    (userId, fio, groupId) =>
                        userId > 0 && !string.IsNullOrWhiteSpace(fio) && groupId > 0));
        }

        public GroupDAO? SelectedGroupItem
        {
            get => _selectedGroupItem;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedGroupItem, value);
                GroupId = _selectedGroupItem?.Id ?? 0;
            }
        }

        private int _userId;
        public int UserId
        {
            get => _userId;
            set => this.RaiseAndSetIfChanged(ref _userId, value);
        }

        private int _userGroupId;
        public int GroupId
        {
            get => _userGroupId;
            set => this.RaiseAndSetIfChanged(ref _userGroupId, value);
        }

        private string _fio;
        public string FIO
        {
            get => _fio;
            set => this.RaiseAndSetIfChanged(ref _fio, value);
        }

        public System.Action? CloseAction { get; set; }

        private async Task LoadGroups()
        {
            var groups = await _groupApiClient.GetGroupsAsync();
            _groups.Clear();
            foreach (var group in groups)
                _groups.Add(group);
        }

        private async Task UpdateUserAsync()
        {
            await _userApiClient.UpdateUser(UserId, FIO, GroupId);
            CloseAction?.Invoke();
        }
    }
}
