using ReactiveUI;
using data.RemoteData.RemoteDatabase.DAO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using httpClient.Group;
using httpClient.User;

namespace Presence.Desktop.ViewModels
{
    public class AddStudentViewModel : ReactiveObject
    {
        private readonly UserAPIClient _userClient;
        private readonly GroupAPIClient _groupClient;
        private readonly GroupDAO _preselectedGroup;

        public ObservableCollection<GroupDAO> Groups { get; } = new ObservableCollection<GroupDAO>();

        public AddStudentViewModel(UserAPIClient userClient,
                                   GroupAPIClient groupClient,
                                   GroupDAO preselectedGroup)
        {
            _userClient = userClient;
            _groupClient = groupClient;
            _preselectedGroup = preselectedGroup;

            AddStudentCommand = ReactiveCommand.CreateFromTask(
                AddStudentAsync,
                this.WhenAnyValue(
                    x => x.FIO,
                    x => x.SelectedGroup,
                    (fio, group) => !string.IsNullOrWhiteSpace(fio) && group != null)
            );

            // Запустить загрузку групп асинхронно
            LoadGroupsAsync();
        }

        private string _fio;
        public string FIO
        {
            get => _fio;
            set => this.RaiseAndSetIfChanged(ref _fio, value);
        }

        private GroupDAO _selectedGroup;
        public GroupDAO SelectedGroup
        {
            get => _selectedGroup;
            set => this.RaiseAndSetIfChanged(ref _selectedGroup, value);
        }

        public ReactiveCommand<Unit, Unit> AddStudentCommand { get; }

        private async Task LoadGroupsAsync()
        {
            try
            {
                var groups = await _groupClient.GetGroupsAsync();
                if (groups != null)
                {
                    Groups.Clear();
                    foreach (var group in groups)
                    {
                        Groups.Add(group);
                    }

                    if (_preselectedGroup != null)
                    {
                        SelectedGroup = Groups.FirstOrDefault(g => g.Id == _preselectedGroup.Id);
                    }
                }
            }
            catch (System.Exception ex)
            {
                // Здесь можно логировать или показывать ошибку
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки групп: {ex.Message}");
            }
        }

        private async Task AddStudentAsync()
        {
            if (SelectedGroup == null || string.IsNullOrWhiteSpace(FIO))
                return;

            await _userClient.CreateUser(FIO,SelectedGroup.Id);
            CloseAction?.Invoke();
        }

        public System.Action CloseAction { get; set; }
    }
}
