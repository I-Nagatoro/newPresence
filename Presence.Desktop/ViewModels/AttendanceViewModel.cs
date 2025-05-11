using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using Avalonia.Threading;
using data.Domain.UseCase;
using data.RemoteData.RemoteDatabase.DAO;
using DynamicData;
using ReactiveUI;

public class AttendanceViewModel : ReactiveObject
{
    private readonly GroupUseCase _groupUseCase;
    private readonly PresenceUseCase _presenceUseCase;
    
    public ObservableCollection<GroupDAO> Groups { get; } = new ObservableCollection<GroupDAO>();
    public ObservableCollection<PresenceDAO> Presences { get; } = new ObservableCollection<PresenceDAO>();
    
    private GroupDAO _selectedGroup;
    public GroupDAO SelectedGroup
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
        GroupUseCase groupUseCase,
        PresenceUseCase presenceUseCase)
    {
        _groupUseCase = groupUseCase;
        _presenceUseCase = presenceUseCase;

        var groups = _groupUseCase.GetAllGroups();
        Groups.AddRange(groups);

        LoadDataCommand = ReactiveCommand.Create(LoadData);

        this.WhenAnyValue(
            x => x.SelectedGroup,
            x => x.StartDate,
            x => x.EndDate
        ).Subscribe(_ => LoadDataCommand.Execute().Subscribe());
    }

    private void LoadData()
    {
        Presences.Clear();

        if (SelectedGroup == null) return;

        var presences = _presenceUseCase.GetPresenceByGroupAndDate(
            SelectedGroup.Id, 
            StartDate, 
            EndDate);

        Debug.WriteLine($"Загружено записей: {presences.Count}");


        foreach (var p in presences)
        {
            Dispatcher.UIThread.Post(() => Presences.Add(p));
        }

        this.RaisePropertyChanged(nameof(Presences));
    }
}