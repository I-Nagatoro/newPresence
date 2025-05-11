using ReactiveUI;
using data.Domain.UseCase;
using data.RemoteData.RemoteDatabase.DAO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Presence.Desktop.ViewModels
{
    public class AddStudentViewModel : ReactiveObject
    {
        private readonly UserUseCase _userUseCase;
        private readonly GroupUseCase _groupUseCase;
        private readonly GroupDAO _preselectedGroup;

        public ObservableCollection<GroupDAO> Groups { get; } = new ObservableCollection<GroupDAO>();

        public AddStudentViewModel(UserUseCase userUseCase, 
                                 GroupUseCase groupUseCase,
                                 GroupDAO preselectedGroup)
        {
            _userUseCase = userUseCase;
            _groupUseCase = groupUseCase;
            _preselectedGroup = preselectedGroup;

            var groups = _groupUseCase.GetAllGroups();
            foreach (var group in groups)
            {
                Groups.Add(group);
            }

            SelectedGroup = Groups.FirstOrDefault(g => g.Id == _preselectedGroup.Id);

            AddStudentCommand = ReactiveCommand.CreateFromTask(
                AddStudentAsync,
                this.WhenAnyValue(
                    x => x.FIO,
                    x => x.SelectedGroup,
                    (fio, group) => !string.IsNullOrWhiteSpace(fio) && group != null)
            );
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

        private async Task AddStudentAsync()
        {
            var newStudent = new UserDAO
            {
                FIO = FIO.Trim(),
                GroupId = SelectedGroup.Id
            };

            await _userUseCase.AddUserAsync(newStudent);
            CloseAction?.Invoke();
        }

        public System.Action CloseAction { get; set; }
    }
}