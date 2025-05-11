using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using httpClient.Group;
using httpClient.User;
using data.RemoteData.RemoteDatabase.DAO;
using Presence.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Presence.Desktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
{
    private readonly GroupAPIClient _groupAPIClient;
    private readonly UserAPIClient _userAPIClient;

    private ObservableCollection<UserDAO> _users = new();
    public ObservableCollection<UserDAO> Users => _users;
    public ObservableCollection<UserDAO> SelectedUsers { get; set; } = new();

    private ObservableCollection<GroupDAO> _groups = new();
    public ObservableCollection<GroupDAO> Groups
    {
        get => _groups;
        set => this.RaiseAndSetIfChanged(ref _groups, value);
    }

    private GroupDAO? _selectedGroupItem;
    public GroupDAO? SelectedGroupItem
    {
        get => _selectedGroupItem;
        set => this.RaiseAndSetIfChanged(ref _selectedGroupItem, value);
    }

    public List<string> SortOptions { get; } = new() { "По фамилии", "По убыванию" };

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

    public MainWindowViewModel(GroupAPIClient groupClient, UserAPIClient userClient)
    {
        _groupAPIClient = groupClient;
        _userAPIClient = userClient;

        OnDeleteUserClicks = ReactiveCommand.CreateFromTask(OnDeleteUserClickAsync, this.WhenAnyValue(vm => vm.CanDelete));
        EditUserCommand = ReactiveCommand.CreateFromTask(OnEditUserClickAsync, this.WhenAnyValue(vm => vm.CanEdit));
        GoPresenceScreen = ReactiveCommand.Create(OpenPresenceScreen);

        RemoveAllStudentsCommand = ReactiveCommand.CreateFromTask(RemoveAllStudentsAsync);
        AddStudentCommand = ReactiveCommand.CreateFromTask(OpenAddStudentWindowAsync);

        this.WhenAnyValue(vm => vm.SelectedGroupItem)
            .Subscribe(_ =>
            {
                SetUsers();
                SortUsers();
            });

        this.WhenAnyValue(vm => vm.SelectedSortOption)
            .Subscribe(_ => SortUsers());

        SelectedUsers.CollectionChanged += (s, e) =>
        {
            this.RaisePropertyChanged(nameof(CanDelete));
            this.RaisePropertyChanged(nameof(CanEdit));
        };

        _ = RefreshGroupsAsync();
    }

    private async Task RefreshGroupsAsync()
    {
        var groups = await _groupAPIClient.GetGroupsAsync();
        if (groups != null)
        {
            Groups = new ObservableCollection<GroupDAO>(groups);
        }
    }

    private void SetUsers()
    {
        _users.Clear();
        if (SelectedGroupItem?.Users != null)
        {
            foreach (var user in SelectedGroupItem.Users)
            {
                _users.Add(user);
            }
        }
        this.RaisePropertyChanged(nameof(Users));
    }

    private void SortUsers()
    {
        if (SelectedGroupItem?.Users == null) return;

        var sorted = SelectedGroupItem.Users.ToList();
        sorted = SelectedSortOption switch
        {
            "По фамилии" => sorted.OrderBy(u => u.FIO).ToList(),
            "По убыванию" => sorted.OrderByDescending(u => u.FIO).ToList(),
            _ => sorted
        };

        _users.Clear();
        foreach (var user in sorted)
            _users.Add(user);
    }

    private async Task RemoveAllStudentsAsync()
    {
        if (SelectedGroupItem == null) return;

        await _groupAPIClient.RemoveAllUsersFromGroup(SelectedGroupItem.Id);
        SelectedGroupItem.Users = new List<UserDAO>();
        SetUsers();
    }

    private async Task OnDeleteUserClickAsync()
    {
        if (SelectedUsers.Count == 0 || SelectedGroupItem == null) return;

        foreach (var user in SelectedUsers.ToList())
        {
            await _userAPIClient.DeleteUserAsync(user.UserId);
        }

        await RefreshGroupsAsync();
        SelectedGroupItem = Groups.FirstOrDefault(g => g.Id == SelectedGroupItem?.Id);
        SelectedUsers.Clear();
    }

    private async Task OpenAddStudentWindowAsync()
    {
        var dialog = new AddStudentWindow(_userAPIClient, _groupAPIClient, SelectedGroupItem);
        var mainWindow = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow != null)
        {
            await dialog.ShowDialog(mainWindow);
            await RefreshGroupsAsync();
            SelectedGroupItem = Groups.FirstOrDefault(g => g.Id == SelectedGroupItem?.Id);
        }
    }

    public void OpenPresenceScreen()
    {
        var presenceScreen = new AttendanceView
        {
            DataContext = App.ServiceProvider.GetRequiredService<AttendanceViewModel>()
        };
        presenceScreen.Show();
    }

    private async Task OnEditUserClickAsync()
    {
        var user = SelectedUsers.FirstOrDefault();
        if (user == null) return;

        var previousSelectedGroupId = SelectedGroupItem?.Id;
        var groups = await _groupAPIClient.GetGroupsAsync();

        var dialog = new EditUserDialog(null, null, user.UserId, user.FIO, user.GroupId, groups);
        var mainWindow = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow == null) return;

        var result = await dialog.ShowEditDialog(mainWindow);
        if (result != (null, null))
        {
            var newName = result.Item1;
            var newGroup = result.Item2;

            await _userAPIClient.UpdateUser(user.UserId, newName, newGroup.Id);
            SelectedUsers.Clear();
        }

        await RefreshGroupsAsync();
        SelectedGroupItem = Groups.FirstOrDefault(g => g.Id == previousSelectedGroupId);
    }
}
}
