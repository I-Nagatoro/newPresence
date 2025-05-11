using data.domain.Models;
using data.Domain.UseCase;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CsvHelper;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using CsvHelper.Configuration;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using data.RemoteData.RemoteDataBase.DAO;
using Presence.Desktop.ViewModels;
using Presence.Desktop.Views;
using data.RemoteData.RemoteDatabase.DAO;
using DynamicData;
using Microsoft.Extensions.DependencyInjection;

namespace Presence.Desktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly GroupUseCase _groupUseCase;
        private readonly UserUseCase _userUseCase;

        private readonly PresenceUseCase presenceUseCase;

        private ObservableCollection<UserDAO> _users;
        public ObservableCollection<UserDAO> Users => _users;
        public ObservableCollection<UserDAO> SelectedUsers { get; set; } = new ObservableCollection<UserDAO>();

        private List<GroupDAO> GroupDAOsDataSource = new List<GroupDAO>();
        private ObservableCollection<GroupDAO> _groups;
        public ObservableCollection<GroupDAO> Groups => _groups;

        private GroupDAO? _selectedGroupItem;
        public GroupDAO? SelectedGroupItem
        {
            get => _selectedGroupItem;
            set => this.RaiseAndSetIfChanged(ref _selectedGroupItem, value);
        }

        public List<string> SortOptions { get; } = new List<string> { "По фамилии", "По убыванию" };

        private string _selectedSortOption;
        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set => this.RaiseAndSetIfChanged(ref _selectedSortOption, value);
        }

        public bool CanDelete => SelectedUsers?.Count > 0;
        public bool CanEdit => SelectedUsers?.Count == 1;

        public ReactiveCommand<Unit, Unit> OnDeleteUserClicks { get; }
        public ReactiveCommand<Unit, Unit> EditUserCommand { get; }
        public ICommand RemoveAllStudentsCommand { get; }
        public ICommand AddStudentCommand { get; }
        

        public ReactiveCommand<Unit, Unit> GoPresenceScreen { get; }

        public MainWindowViewModel(GroupUseCase groupUseCase, UserUseCase userUseCase)
        {
            OnDeleteUserClicks = ReactiveCommand.Create(OnDeleteUserClick, this.WhenAnyValue(vm => vm.CanDelete));
            EditUserCommand = ReactiveCommand.Create(OnEditUserClick, this.WhenAnyValue(vm => vm.CanEdit));
            _groupUseCase = groupUseCase;
            _userUseCase = userUseCase;
            GoPresenceScreen = ReactiveCommand.Create(OpenPresenceScreen);

            _groups = new ObservableCollection<GroupDAO>(GroupDAOsDataSource);
            _users = new ObservableCollection<UserDAO>();

            this.WhenAnyValue(vm => vm.SelectedGroupItem)
                .Subscribe(_ =>
                {
                    RefreshGroups();
                    SetUsers();
                });

            this.WhenAnyValue(vm => vm.SelectedGroupItem)
                .Subscribe(vm => SetUsers());

            this.WhenAnyValue(vm => vm.SelectedSortOption)
                .Subscribe(_ => SortUsers());

            RemoveAllStudentsCommand = ReactiveCommand.Create(RemoveAllStudents);
            AddStudentCommand = ReactiveCommand.CreateFromTask(OpenAddStudentWindowAsync);

            SelectedUsers.CollectionChanged += (s, e) =>
            {
                this.RaisePropertyChanged(nameof(CanDelete));
                this.RaisePropertyChanged(nameof(CanEdit));
            };
        }
        
        private void SetUsers()
        {
            _users.Clear();

            if (SelectedGroupItem != null && SelectedGroupItem.Users != null)
            {
                foreach (var user in SelectedGroupItem.Users)
                {
                    _users.Add(user);
                }
            }
            RefreshGroups();
            this.RaisePropertyChanged(nameof(Users));
        }

        private void SortUsers()
        {
            if (SelectedGroupItem?.Users == null) return;

            var sortedUsers = SelectedGroupItem.Users.ToList();

            switch (SelectedSortOption)
            {
                case "По фамилии":
                    sortedUsers = sortedUsers.OrderBy(u => u.FIO).ToList();
                    break;
                case "По убыванию":
                    sortedUsers = sortedUsers.OrderByDescending(u => u.FIO).ToList();
                    break;
            }

            Users.Clear();
            foreach (var item in sortedUsers)
            {
                Users.Add(item);
            }
        }

        private void RemoveAllStudents()
        {
            if (SelectedGroupItem == null) return;

            _groupUseCase.RemoveAllStudentsFromGroup(SelectedGroupItem.Id);
            SelectedGroupItem.Users = new List<UserDAO>();
            SetUsers();
        }

        private async Task OpenAddStudentWindowAsync()
        {
            var dialog = new AddStudentWindow(
                _userUseCase,
                _groupUseCase,
                SelectedGroupItem
            );
            var groupId = SelectedGroupItem?.Id;
            var mainWindow = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow != null)
            {
                await dialog.ShowDialog(mainWindow);
                RefreshGroups();
                SelectedGroupItem = Groups.FirstOrDefault(g => g.Id == groupId);
            }
        }

        public void OnDeleteUserClick()
        {
            if (SelectedUsers.Count == 0 || SelectedGroupItem?.Users == null)
                return;

            foreach (var user in SelectedUsers.ToList())
            {
                _userUseCase.RemoveUserById(user.UserId);

                var updatedUsers = SelectedGroupItem.Users.Where(u => u != user).ToList();
                SelectedGroupItem.Users = new List<UserDAO>(updatedUsers);
            }

            SetUsers();
            SelectedUsers.Clear();

            this.RaisePropertyChanged(nameof(CanDelete));
            this.RaisePropertyChanged(nameof(CanEdit));
        }

        private void RefreshGroups()
        {
            GroupDAOsDataSource.Clear();
            _groups.Clear();

            foreach (var item in _groupUseCase.GetAllGroups())
            {
                var groupPresenter = new GroupDAO
                {
                    Id = item.Id,
                    Name = item.Name,
                    Users = item.Users?.Select(user => new UserDAO
                    {
                        FIO = user.FIO,
                        UserId = user.UserId,
                        Group = new GroupDAO { Id = item.Id, Name = item.Name }
                    }).ToList()
                };

                GroupDAOsDataSource.Add(groupPresenter);
                _groups.Add(item);
            }

            this.RaisePropertyChanged(nameof(Groups));
        }

        public void OpenPresenceScreen()
        {
            var presenceScreen = new AttendanceView
            {
                DataContext = App.Services.GetRequiredService<AttendanceViewModel>()
            };
            presenceScreen.Show();
        }


        public async void OnEditUserClick()
        {
            var user = SelectedUsers.FirstOrDefault();
            if (user == null) return;

            var previousSelectedGroupId = SelectedGroupItem?.Id;

            var groups = _groupUseCase.GetAllGroups();

            var editDialog = new EditUserDialog(_groupUseCase, _userUseCase, user.UserId, user.FIO, user.GroupId, groups);

            var mainWindow = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow == null) return;

            var result = await editDialog.ShowEditDialog(mainWindow);

            if (result != (null, null))
            {
                var newName = result.Item1;
                var newGroup = result.Item2;

                user.FIO = newName;
                user.GroupId = newGroup.Id;

                _userUseCase.UpdateUser(user.UserId, user.FIO, user.GroupId);

                SelectedUsers.Clear();
            }

            RefreshGroups();
            SelectedGroupItem = Groups.FirstOrDefault(g => g.Id == previousSelectedGroupId);

            this.RaisePropertyChanged(nameof(CanEdit));
            this.RaisePropertyChanged(nameof(CanDelete));
        }

    }
}
