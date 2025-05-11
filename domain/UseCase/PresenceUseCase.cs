using ClosedXML.Excel;
using data.RemoteData.RemoteDataBase.DAO;
using data.Repository;
using data.domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data.RemoteData.RemoteDatabase.DAO;
using domain.Models;

namespace data.Domain.UseCase
{
    public class PresenceUseCase
    {
        public readonly IUserRepository _userRepository;
        public readonly IPresenceRepository _presenceRepository;
        public readonly IGroupRepository _groupRepository;

        public PresenceUseCase(IUserRepository userRepository, IPresenceRepository presenceRepository, IGroupRepository groupRepository)
        {
            _userRepository = userRepository;
            _presenceRepository = presenceRepository;
            _groupRepository = groupRepository;
        }

        public Dictionary<string, List<AttendanceRecord>> GetAllAttendanceByGroups()
        {
            var attendanceByGroup = new Dictionary<string, List<AttendanceRecord>>();
            var allGroups = _groupRepository.GetAllGroups();

            foreach (var group in allGroups)
            {
                var groupAttendance = _presenceRepository.GetAttendanceByGroup(group.Id);
                var attendanceRecords = new List<AttendanceRecord>();

                foreach (var record in groupAttendance)
                {
                    var names = _userRepository.GetUserNames().Where(u => u.UserId == record.UserId);
                    foreach (var name in names)
                    {
                        attendanceRecords.Add(new AttendanceRecord
                        {
                            UserName = name.FIO,
                            UserId = name.UserId,
                            Date = record.Date,
                            IsAttedance = record.IsAttendance,
                            LessonNumber = record.LessonNumber,
                            GroupName = group.Name
                        });
                    }
                }

                attendanceByGroup.Add(group.Name, attendanceRecords);
            }

            return attendanceByGroup;
        }

        public class PresenceViewItem
        {
            public DateOnly Date { get; set; }
            public int LessonNumber { get; set; }
            public string Name { get; set; }
            public string Attendance { get; set; }
        }

        public List<PresenceViewItem> GetFilteredPresence(int groupId, DateOnly startDate, DateOnly endDate)
        {
            var presences = _presenceRepository.GetPresence(groupId, startDate, endDate);
            return presences.Select(p => new PresenceViewItem
            {
                Date = p.Date,
                LessonNumber = p.LessonNumber,
                Attendance = p.IsAttendance ? "Present" : "Absent"
            }).ToList();
        }


        public void ExportAttendanceToExcel()
        {
            var attendanceByGroup = GetAllAttendanceByGroups();
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string reportsFolderPath = Path.Combine(projectDirectory, "Reports");
            string filePath = Path.Combine(reportsFolderPath, "AttendanceReport.xlsx");

            if (!Directory.Exists(reportsFolderPath))
            {
                Directory.CreateDirectory(reportsFolderPath);
            }
            using (var workbook = new XLWorkbook())
            {
                foreach (var group in attendanceByGroup)
                {
                    var worksheet = workbook.Worksheets.Add($"{group.Key}");
                    worksheet.Cell(1, 1).Value = "ФИО";
                    worksheet.Cell(1, 2).Value = "Группа";
                    worksheet.Cell(1, 3).Value = "Дата";
                    worksheet.Cell(1, 4).Value = "Занятие";
                    worksheet.Cell(1, 5).Value = "Статус";

                    int row = 2;
                    int lesNum = 1;
                    foreach (var record in group.Value.OrderBy(r => r.Date).ThenBy(r => r.LessonNumber).ThenBy(r => r.UserId))
                    {
                        if (lesNum != record.LessonNumber)
                        {
                            row++;
                        }
                        worksheet.Cell(row, 1).Value = record.UserName;
                        worksheet.Cell(row, 2).Value = record.GroupName;
                        worksheet.Cell(row, 3).Value = record.Date.ToString("dd.MM.yyyy");
                        worksheet.Cell(row, 4).Value = record.LessonNumber;
                        worksheet.Cell(row, 5).Value = record.IsAttedance ? "Присутствует" : "Отсутствует";
                        row++;



                        lesNum = record.LessonNumber;
                    }

                    worksheet.Columns().AdjustToContents();
                }

                workbook.SaveAs(filePath);
            }
        }




        public List<PresenceDAO> GetPresenceByDateAndGroup(DateOnly startDate, DateOnly endDate, int groupId)
        {
            return _presenceRepository.GetPresenceByDateAndGroup(startDate, endDate, groupId);
        }

        public void GeneratePresenceDaily(int firstLesson, int lastLesson, int groupId)
        {
            var users = _userRepository.GetUsersByGroupId(groupId)
                .ToList();

            DateOnly startDate = _presenceRepository.GetLastDateByGroupId(groupId)?.AddDays(1)
                                 ?? DateOnly.FromDateTime(DateTime.Today);
            List<PresenceDAO> presences = new List<PresenceDAO>();

            for (int lessonNumber = firstLesson; lessonNumber <= lastLesson; lessonNumber++)
            {
                foreach (var user in users)
                {
                    presences.Add(new PresenceDAO
                    {
                        UserId = user.UserId,
                        GroupId = groupId,
                        Date = startDate,
                        LessonNumber = lessonNumber,
                        IsAttendance = true
                    });
                }
            }
            _presenceRepository.SavePresence(presences);
        }
        


        public void GenerateWeeklyPresence(int firstLesson, int lastLesson, int groupId, DateOnly startTime)
        {
            for (int i = 0; i < 7; i++)
            {
                DateOnly currentTime = startTime.AddDays(i);
                GeneratePresenceDaily(firstLesson, lastLesson, groupId);
            }
        }


        public bool MarkUserAbsentForLessons(int userId, int groupId, int firstLesson, int lastLesson, DateOnly date)
        {
            List<PresenceDAO> presences = _presenceRepository.GetPresenceForAbsent(date, groupId);
            if (presences.Where(p => p.UserId == userId).Count() > 0)
            {
                foreach (var presence in presences.Where(p => p.UserId == userId && p.LessonNumber >= firstLesson && p.LessonNumber <= lastLesson))
                {
                    presence.IsAttendance = false; 
                }
                _presenceRepository.UpdateAtt(userId, groupId, firstLesson, lastLesson, date, false);
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<PresenceDAO> GetAllPresenceByGroup(int groupId)
        {
            return _presenceRepository.GetPresenceByGroup(groupId);
        }

        public GroupAttendanceStatistics GetGeneralPresence(int groupId)
        {
            return _presenceRepository.GetGeneralPresenceForGroup(groupId);
        }

        public List<AllPresence> GetPresenceAll(DateOnly dateStart, DateOnly dateEnd, int GroupId, int UserId)
        {
            return _presenceRepository.AllPresence(GroupId, dateEnd, dateStart, UserId); ;
        }

        public void UpdateAttendance(List<AttendanceInputModel> attList)
        {
            _presenceRepository.UpdateAttendance(attList);
        }

        public void DeletePresenceByGroup(int groupId)
        {
            var presences = _presenceRepository.GetPresenceByGroup(groupId);
            _presenceRepository.DeletePresences(presences);
        }

        public void DeletePresenceByUser(int groupId, int userId)
        {
            var presences = _presenceRepository.GetPresenceByUserAndGroup(userId, groupId);
            _presenceRepository.DeletePresences(presences);
        }

        public void DeletePresenceByDateRange(int groupId, DateOnly startDate, DateOnly endDate)
        {
            var presences = _presenceRepository.GetPresenceByDateRange(groupId, startDate, endDate);
            _presenceRepository.DeletePresences(presences);
        }

        public void MarkUserAbsent(int userId, int groupId, DateOnly date, int lessonNumber)
        {
            _presenceRepository.UpdateAttendance(userId, groupId, date, lessonNumber, false); 
        }
        
        public List<PresenceDAO> GetPresenceByGroupAndDate(int groupId, DateOnly startDate, DateOnly endDate)
        {
            var presenceRecords = _presenceRepository.GetPresenceByGroupAndDate(
                groupId, 
                startDate, 
                endDate);
            Debug.WriteLine($"UseCase: Получено {presenceRecords.Count()} записей.");
            return presenceRecords.Select(p => new PresenceDAO
            {
                PresenceId = p.PresenceId,
                UserId = p.UserId,
                IsAttendance = p.IsAttendance,
                Date = p.Date,
                LessonNumber = p.LessonNumber,
                GroupId = p.GroupId
            }).ToList();
        }
        
        public string GetStudentName(int userId)
        {
            return _userRepository.GetUserById(userId)?.FIO ?? "Неизвестный";
        }
        
        public void ClearAllPresence()
        {
            _presenceRepository.ClearAllPresence();
        }
        
        public List<PresenceDAO> GetPresenceByUserId(int groupId, DateOnly? date, int? student)
        {
            return _presenceRepository.GetPresenceByUserId(groupId, date, student);
        }

        public void ClearPresenceByGroup(int groupId)
        {
            _presenceRepository.DeletePresenceByGroup(groupId);
        }

        public void AddPresence(List<PresenceInputDTO> dtoList)
        {
            var converted = dtoList.Select(p => new PresenceDAO
            {
                Date = p.Date,
                LessonNumber = p.LessonNumber,
                UserId = p.Student,
                IsAttendance = p.Status,
                GroupId = _userRepository.GetUserById(p.Student)?.GroupId ?? throw new Exception("User not found")
            }).ToList();

            _presenceRepository.AddPresence(converted);
        }

        public void UpdatePresence(List<UpdateAttendanceDTO> dtoList)
        {
            var converted = dtoList.Select(p => new PresenceDAO
            {
                Date = p.Date,
                LessonNumber = p.LessonNumber,
                UserId = p.Student,
                IsAttendance = p.Status
            }).ToList();

            _presenceRepository.UpdatePresence(converted);
        }
    }
}
