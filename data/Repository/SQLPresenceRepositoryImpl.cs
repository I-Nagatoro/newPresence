using data.domain.Models;
using data.RemoteData.RemoteDatabase.DAO;
using data.RemoteData.RemoteDataBase;
using Npgsql;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace data.Repository
{
    public class SQLPresenceRepositoryImpl : IPresenceRepository
    {
        private readonly RemoteDatabaseContext _remoteDatabaseContext;

        public SQLPresenceRepositoryImpl(RemoteDatabaseContext remoteDatabaseContext)
        {
            _remoteDatabaseContext = remoteDatabaseContext;
        }


        public List<PresenceDAO> GetAttendanceByGroup(int groupId)
        {
            return _remoteDatabaseContext.Presences
                .Where(p => p.GroupId == groupId)
                .Select(p => new PresenceDAO
                {
                    UserId = p.UserId,
                    GroupId = p.GroupId,
                    Date = p.Date,
                    LessonNumber = p.LessonNumber,
                    IsAttendance = p.IsAttendance
                })
                .ToList();
        }


        public List<PresenceDAO> GetPresenceForAbsent(DateOnly date, int GroupId)
        {
            return _remoteDatabaseContext.Presences.Where(p => p.GroupId == GroupId && p.Date == date).ToList();
        }

        public List<PresenceDAO> GetPresenceByDateAndGroup(DateOnly startDate, DateOnly endDate, int groupId)
        {
            return _remoteDatabaseContext.Presences.Where(p => p.Date >= startDate && p.Date <= endDate &&
                                                               _remoteDatabaseContext.Users.Any(u =>
                                                                   u.GroupId == groupId && u.UserId == p.UserId))
                .ToList();
        }

        public List<PresenceDAO> GetPresenceByGroup(int groupId)
        {
            return _remoteDatabaseContext.Presences.Where(p => p.GroupId == groupId)
                .OrderBy(p => p.Date)
                .ThenBy(p => p.UserId).ToList();
        }

        public void SavePresence(List<PresenceDAO> presences)
        {
            foreach (var presence in presences)
            {
                _remoteDatabaseContext.Presences.Add(presence);
            }

            _remoteDatabaseContext.SaveChanges();
        }

        public void UpdateAtt(int userId, int groupId, int firstLesson, int lastLesson, DateOnly date,
            bool isAttendance)
        {
            var presences = _remoteDatabaseContext.Presences
                .Where(p => p.UserId == userId
                            && p.GroupId == groupId
                            && p.LessonNumber >= firstLesson
                            && p.LessonNumber <= lastLesson
                            && p.Date == date)
                .ToList();

            foreach (var presence in presences)
            {
                presence.IsAttendance = isAttendance;
            }

            _remoteDatabaseContext.SaveChanges(); 
        }

        public DateOnly? GetLastDateByGroupId(int groupId)
        {
            var lastDate = _remoteDatabaseContext.Presences
                .Where(p => p.GroupId == groupId)
                .OrderByDescending(p => p.Date)
                .Select(p => p.Date)
                .FirstOrDefault();

            return lastDate == default ? (DateOnly?)null : lastDate;
        }

        public List<PresenceDAO> PresenceSort(List<PresenceDAO> presences)
        {
            presences = _remoteDatabaseContext.Presences.OrderBy(p => p.Date).ToList();
            return presences;
        }


        public GroupAttendanceStatistics GetGeneralPresenceForGroup(int groupId)
        {
            var presences = _remoteDatabaseContext.Presences.Where(p => p.GroupId == groupId)
                .OrderBy(p => p.LessonNumber).ToList();
            var dates = _remoteDatabaseContext.Presences;
            var distDates = dates.Select(p => p.Date).Distinct().ToList();
            int lesId = 0;
            int lesNum = 1;
            double att = 0;
            int days = -1;
            int countAllLes = 0;
            DateOnly date = DateOnly.MinValue;
            List<int> usersId = new List<int>();

            foreach (var presence in presences)
            {
                if (!usersId.Contains(presence.UserId))
                {
                    usersId.Add(presence.UserId);
                }

                if (presence.Date != date)
                {
                    date = presence.Date;
                    lesId++;
                    lesNum = presence.LessonNumber;
                    days++;
                }

                if (presence.LessonNumber != lesNum && date == presence.Date)
                {
                    lesNum = presence.LessonNumber;
                    countAllLes++;
                    lesId++;
                }


                if (presence.IsAttendance)
                {
                    att++;
                }

            }

            List<UserAttendance> a = new List<UserAttendance>();
            List<int> ids = new List<int>();
            double ok = 0;
            double skip = 0;
            int userId = 0;
            foreach (var user in usersId)
            {
                var users = _remoteDatabaseContext.Presences.Where(p => p.UserId == user);
                foreach (var usera in users)
                {
                    userId = usera.UserId;
                    if (!ids.Contains(usera.UserId))
                    {
                        skip = 0;
                        ok = 0;
                        ids.Add(userId);
                        a.Add(new UserAttendance { UserId = userId, Attended = ok, Missed = skip });
                        userId = usera.UserId;
                        if (usera.IsAttendance)
                        {
                            a.First(a => a.UserId == usera.UserId).Attended = ok += 1;
                        }
                        else
                        {
                            a.First(a => a.UserId == usera.UserId).Missed = skip += 1;
                        }
                    }
                    else
                    {
                        if (usera.IsAttendance)
                        {
                            a.First(a => a.UserId == usera.UserId).Attended = ok += 1;
                        }
                        else
                        {
                            a.First(a => a.UserId == usera.UserId).Missed = skip += 1;
                        }
                    }
                }
            }

            var statistics = new GroupAttendanceStatistics
            {
                UserCount = usersId.Count,
                TotalLessons = lesId,
                AttendancePercentage = att / usersId.Count / lesNum / distDates.Count() * 100
            };

            foreach (var user in a)
            {
                statistics.UserAttendanceDetails.Add(new UserAttendance
                {
                    UserId = user.UserId,
                    Attended = user.Attended,
                    Missed = user.Missed,
                    AttendanceRate = user.Attended / (user.Missed + user.Attended) * 100
                });
            }

            return statistics;
        }

        public List<AllPresence> AllPresence(int GroupId, DateOnly? dateStart, DateOnly? dateEnd, int? UserId)
        {
            List<AllPresence> result = new List<AllPresence>(); 

            if (dateStart != DateOnly.MinValue && dateEnd != DateOnly.MinValue && UserId != 0)
            {
                var presenceData = _remoteDatabaseContext.Presences
                    .Where(p => p.Date >= dateStart && p.Date <= dateEnd && p.UserId == UserId && p.GroupId == GroupId)
                    .Select(p => new
                    {
                        GroupName = _remoteDatabaseContext.Groups
                            .Where(g => g.Id == p.GroupId)
                            .Select(g => g.Name)
                            .FirstOrDefault(),
                        UserName = _remoteDatabaseContext.Users
                            .Where(u => u.UserId == p.UserId)
                            .Select(u => u.FIO)
                            .FirstOrDefault(),
                        p.LessonNumber,
                        p.Date,
                        p.IsAttendance
                    })
                    .ToList();

                result = presenceData.Select(d => new AllPresence
                {
                    GroupName = d.GroupName,
                    Users = new UsersForPresence
                    {
                        FIO = d.UserName,
                        LessonNumber = d.LessonNumber,
                        Date = d.Date,
                        isAttendance = d.IsAttendance
                    }
                }).ToList();
            }
            else if (dateStart != DateOnly.MinValue && dateEnd != DateOnly.MinValue)
            {
                var presenceData = _remoteDatabaseContext.Presences
                    .Where(p => p.Date >= dateStart && p.Date <= dateEnd && p.GroupId == GroupId)
                    .Select(p => new
                    {
                        GroupName = _remoteDatabaseContext.Groups
                            .Where(g => g.Id == p.GroupId)
                            .Select(g => g.Name)
                            .FirstOrDefault(),
                        UserName = _remoteDatabaseContext.Users
                            .Where(u => u.UserId == p.UserId)
                            .Select(u => u.FIO)
                            .FirstOrDefault(),
                        p.LessonNumber,
                        p.Date,
                        p.IsAttendance
                    })
                    .ToList();

                result = presenceData.Select(d => new AllPresence
                {
                    GroupName = d.GroupName,
                    Users = new UsersForPresence
                    {
                        FIO = d.UserName,
                        LessonNumber = d.LessonNumber,
                        Date = d.Date,
                        isAttendance = d.IsAttendance
                    }
                }).ToList();
            }
            else if (UserId != 0)
            {
                var presenceData = _remoteDatabaseContext.Presences
                    .Where(p => p.UserId == UserId && p.GroupId == GroupId)
                    .Select(p => new
                    {
                        GroupName = _remoteDatabaseContext.Groups
                            .Where(g => g.Id == p.GroupId)
                            .Select(g => g.Name)
                            .FirstOrDefault(),
                        UserName = _remoteDatabaseContext.Users
                            .Where(u => u.UserId == p.UserId)
                            .Select(u => u.FIO)
                            .FirstOrDefault(),
                        p.LessonNumber,
                        p.Date,
                        p.IsAttendance
                    })
                    .ToList();

                result = presenceData.Select(d => new AllPresence
                {
                    GroupName = d.GroupName,
                    Users = new UsersForPresence
                    {
                        FIO = d.UserName,
                        LessonNumber = d.LessonNumber,
                        Date = d.Date,
                        isAttendance = d.IsAttendance
                    }
                }).ToList();
            }
            else
            {
                var presenceData = _remoteDatabaseContext.Presences
                    .Where(p => p.GroupId == GroupId)
                    .Select(p => new
                    {
                        GroupName = _remoteDatabaseContext.Groups
                            .Where(g => g.Id == p.GroupId)
                            .Select(g => g.Name)
                            .FirstOrDefault(),
                        UserName = _remoteDatabaseContext.Users
                            .Where(u => u.UserId == p.UserId)
                            .Select(u => u.FIO)
                            .FirstOrDefault(),
                        p.LessonNumber,
                        p.Date,
                        p.IsAttendance
                    })
                    .ToList();

                result = presenceData.Select(d => new AllPresence
                {
                    GroupName = d.GroupName,
                    Users = new UsersForPresence
                    {
                        FIO = d.UserName,
                        LessonNumber = d.LessonNumber,
                        Date = d.Date,
                        isAttendance = d.IsAttendance
                    }
                }).ToList();
            }

            return result;
        }

        public void UpdateAttendance(List<AttendanceInputModel> attendanceList)
        {
            foreach (var attendance in attendanceList)
            {
                var existingRecord = _remoteDatabaseContext.Presences
                    .FirstOrDefault(p =>
                        p.GroupId == attendance.GroupId && p.UserId == attendance.UserId && p.Date == attendance.Date &&
                        p.LessonNumber == attendance.LessonNumber);

                if (existingRecord != null)
                {
                    existingRecord.IsAttendance = attendance.IsAttendance;
                    _remoteDatabaseContext.SaveChanges();
                }
                else
                {
                    var newRecord = new PresenceDAO
                    {
                        GroupId = attendance.GroupId,
                        UserId = attendance.UserId,
                        Date = attendance.Date,
                        LessonNumber = attendance.LessonNumber,
                        IsAttendance = attendance.IsAttendance
                    };
                    _remoteDatabaseContext.Presences.Add(newRecord);
                    _remoteDatabaseContext.SaveChanges();
                }
            }
        }



        public void DeletePresenceByUser(int groupId, int userId)
        {
            var attendanceToDelete = _remoteDatabaseContext.Presences
                .Where(p => p.GroupId == groupId && p.UserId == userId)
                .ToList();
            _remoteDatabaseContext.Presences.RemoveRange(attendanceToDelete);
            _remoteDatabaseContext.SaveChanges();
        }

        public void DeletePresenceByDateRange(int groupId, DateOnly startDate, DateOnly endDate)
        {
            var attendanceToDelete = _remoteDatabaseContext.Presences
                .Where(p => p.GroupId == groupId && p.Date >= startDate && p.Date <= endDate)
                .ToList();
            _remoteDatabaseContext.Presences.RemoveRange(attendanceToDelete);
            _remoteDatabaseContext.SaveChanges();
        }

        public void DeletePresenceByGroup(int groupId)
        {
            var attendanceToDelete = _remoteDatabaseContext.Presences
                .Where(p => p.GroupId == groupId)
                .ToList();
            _remoteDatabaseContext.Presences.RemoveRange(attendanceToDelete);
            _remoteDatabaseContext.SaveChanges();
        }

        public void DeletePresences(List<PresenceDAO> presences)
        {
            _remoteDatabaseContext.Presences.RemoveRange(presences);
            _remoteDatabaseContext.SaveChanges();
        }

        public List<PresenceDAO> GetPresenceByUserAndGroup(int userId, int groupId)
        {
            return _remoteDatabaseContext.Presences
                .Where(p => p.UserId == userId && p.GroupId == groupId)
                .ToList();
        }

        public List<PresenceDAO> GetPresenceByDateRange(int groupId, DateOnly startDate, DateOnly endDate)
        {
            return _remoteDatabaseContext.Presences
                .Where(p => p.GroupId == groupId && p.Date >= startDate && p.Date <= endDate)
                .ToList();
        }

        public void UpdateAttendance(int userId, int groupId, DateOnly date, int lessonNumber, bool isAttendance)
        {
            var presences = _remoteDatabaseContext.Presences
                .Where(p => p.UserId == userId
                            && p.GroupId == groupId
                            && p.Date == date
                            && p.LessonNumber == lessonNumber)
                .ToList();

            foreach (var presence in presences)
            {
                presence.IsAttendance = isAttendance;
            }

            _remoteDatabaseContext.SaveChanges();
        }

        public IEnumerable<PresenceDAO> GetPresence(int groupId, DateOnly startDate, DateOnly endDate)
        {
            return _remoteDatabaseContext.Presences
                .Where(p => p.GroupId == groupId
                            && p.Date >= startDate
                            && p.Date <= endDate)
                .Join(_remoteDatabaseContext.Users,
                    presence => presence.UserId,
                    user => user.UserId,
                    (presence, user) => new PresenceDAO
                    {
                        PresenceId = presence.PresenceId,
                        UserId = presence.UserId,
                        GroupId = presence.GroupId,
                        Date = presence.Date,
                        LessonNumber = presence.LessonNumber
                    })
                .ToList();
        }


        public IEnumerable<PresenceDAO> GetPresenceByGroupAndDate(int groupId, DateOnly startDate, DateOnly endDate)
        {
            return _remoteDatabaseContext.Presences
                .Where(p => p.GroupId == groupId &&
                            p.Date >= startDate &&
                            p.Date <= endDate)
                .Join(
                    _remoteDatabaseContext.Users,
                    presence => presence.UserId,
                    user => user.UserId,
                    (presence, user) => new PresenceDAO
                    {
                        PresenceId = presence.PresenceId,
                        UserId = presence.UserId,
                        Date = presence.Date,
                        LessonNumber = presence.LessonNumber,
                        IsAttendance = presence.IsAttendance,
                        GroupId = presence.GroupId
                    })
                .ToList();
        }
        
        public void ClearAllPresence()
        {
            var allPresenceRecords = _remoteDatabaseContext.Presences.ToList();
            _remoteDatabaseContext.Presences.RemoveRange(allPresenceRecords);
            _remoteDatabaseContext.SaveChanges();
        }

        public void AddPresence(List<PresenceDAO> presenceList)
        {
            _remoteDatabaseContext.Presences.AddRange(presenceList);
            _remoteDatabaseContext.SaveChanges();
        }

        public void UpdatePresence(List<PresenceDAO> updatedList)
        {
            foreach (var item in updatedList)
            {
                var existing = _remoteDatabaseContext.Presences.FirstOrDefault(p =>
                    p.Date == item.Date &&
                    p.UserId == item.UserId &&
                    p.LessonNumber == item.LessonNumber);

                if (existing != null)
                {
                    existing.IsAttendance = item.IsAttendance;
                }
            }

            _remoteDatabaseContext.SaveChanges();
        }
        
        public List<PresenceDAO> GetPresenceByUserId(int groupId, DateOnly? date, int? userId)
        {
            var query = _remoteDatabaseContext.Presences.Where(p => p.GroupId == groupId);

            if (date.HasValue)
                query = query.Where(p => p.Date == date.Value);

            if (userId.HasValue)
                query = query.Where(p => p.UserId == userId.Value);

            return query.ToList();
        }

        public List<PresenceDAO> GetAllPresenceForDay(int groupId, DateOnly? date)
        {
            var query = _remoteDatabaseContext.Presences.Where(p => p.GroupId == groupId && p.Date == date);
            return query.ToList();
        }
    }
}
