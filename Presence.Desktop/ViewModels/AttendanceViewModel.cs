using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Threading;
using data.RemoteData.RemoteDatabase.DAO;
using httpClient.Group;
using httpClient.Presence;
using ReactiveUI;

public class AttendanceViewModel : ReactiveObject
{
    private readonly GroupAPIClient _groupApiClient;
    private readonly PresenceAPIClient _presenceApiClient;

    public ObservableCollection<GroupDAO> Groups { get; } = new ObservableCollection<GroupDAO>();
    public ObservableCollection<PresenceDAO> Presences { get; } = new ObservableCollection<PresenceDAO>();

    private GroupDAO? _selectedGroup;
    public GroupDAO? SelectedGroup
    {
        get => _selectedGroup;
        set => this.RaiseAndSetIfChanged(ref _selectedGroup, value);
    }

    private DateOnly _startDate;
    public DateOnly StartDate
    {
        get => _startDate;
        set => this.RaiseAndSetIfChanged(ref _startDate, value);
    }

    private DateOnly _endDate;
    public DateOnly EndDate
    {
        get => _endDate;
        set => this.RaiseAndSetIfChanged(ref _endDate, value);
    }

    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }

    public AttendanceViewModel(
        GroupAPIClient groupApiClient,
        PresenceAPIClient presenceApiClient)
    {
        _groupApiClient = groupApiClient;
        _presenceApiClient = presenceApiClient;

        LoadGroups();

        LoadDataCommand = ReactiveCommand.CreateFromTask(async () => await LoadData());

        this.WhenAnyValue(
            x => x.SelectedGroup,
            x => x.StartDate,
            x => x.EndDate
        ).Subscribe(_ => LoadDataCommand.Execute().Subscribe());
    }

    private async void LoadGroups()
    {
        var groups = await _groupApiClient.GetGroupsAsync();
        Groups.Clear();
        foreach (var group in groups)
            Groups.Add(group);
    }

    private async Task LoadData()
    {
        Presences.Clear();

        if (SelectedGroup == null)
            return;

        try
        {
            var response = await _presenceApiClient.GetPresenceAsync(
                SelectedGroup.Id,
                StartDate,
                EndDate);

            if (response?.Users == null)
                return;

            foreach (var u in response.Users)
            {
                var presence = new PresenceDAO
                {
                    UserId = u.UserId,
                    LessonNumber = u.LessonNumber,
                    Date = u.Date,
                    IsAttendance = u.IsAttendance
                };

                Dispatcher.UIThread.Post(() => Presences.Add(presence));
            }

            this.RaisePropertyChanged(nameof(Presences));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке данных посещаемости: {ex.Message}");
            Presences.Clear();
        }
    }

}
