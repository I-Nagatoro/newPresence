using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using Avalonia.Interactivity;

namespace Presence.Desktop.Views
{
    public partial class AttendanceView : Window
    {
        public AttendanceView()
        {
            InitializeComponent();
            DataContext = App.Services.GetRequiredService<AttendanceViewModel>();
        }

        private void OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not AttendanceViewModel vm) return;

            var calendar = (Calendar)sender;
            var dates = calendar.SelectedDates;

            if (dates.Count == 0) return;

            vm.StartDate = DateOnly.FromDateTime(dates[0]);
            vm.EndDate = DateOnly.FromDateTime(dates[^1]);

            vm.LoadDataCommand.Execute().Subscribe();
        }
        
        private void OnCheckData(object sender, RoutedEventArgs e)
        {
            if (DataContext is AttendanceViewModel vm)
            {
                foreach (var item in vm.Presences)
                {
                    Debug.WriteLine($"{item.DateString} | {item.LessonNumber} | {item.Status}");
                }
            }
        }
    }
}
