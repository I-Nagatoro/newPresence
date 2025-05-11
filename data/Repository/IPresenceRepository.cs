using data.RemoteData.RemoteDatabase.DAO;
namespace data.Repository
{
    public interface IPresenceRepository
    {
        List<PresenceDAO> GetPresenceByDateAndGroup(DateOnly startDate, DateOnly endTime, int groupId);
        void SavePresence(List<PresenceDAO> presences);
        List<PresenceDAO> GetPresenceByGroup(int groupId);
        DateOnly? GetLastDateByGroupId(int groupId);
        List<PresenceDAO> GetPresenceForAbsent(DateOnly date, int GroupId);
        GroupAttendanceStatistics GetGeneralPresenceForGroup(int groupId);
        void UpdateAtt(int userId, int groupId, int firstLesson, int lastLesson, DateOnly date, bool isAttendance);
        List<PresenceDAO> GetAttendanceByGroup(int groupId);
        public List<AllPresence> AllPresence(int GroupId, DateOnly? dateStart, DateOnly? dateEnd, int? UserId);
        public void UpdateAttendance(List<AttendanceInputModel> attendanceList);
        public void DeletePresenceByUser(int groupId, int userId);
        public void DeletePresenceByDateRange(int groupId, DateOnly startDate, DateOnly endDate);
        public void DeletePresenceByGroup(int groupId);
        void DeletePresences(List<PresenceDAO> presences);
        List<PresenceDAO> GetPresenceByUserAndGroup(int userId, int groupId);
        List<PresenceDAO> GetPresenceByDateRange(int groupId, DateOnly startDate, DateOnly endDate);
        void UpdateAttendance(int userId, int groupId, DateOnly date, int lessonNumber, bool isAttendance);
        public IEnumerable<PresenceDAO> GetPresence(int groupId, DateOnly startDate, DateOnly endDate);
        public IEnumerable<PresenceDAO> GetPresenceByGroupAndDate(int groupId, DateOnly startDate, DateOnly endDate);
        public void ClearAllPresence();
        public List<PresenceDAO> GetAllPresenceForDay(int groupId, DateOnly? date);
        public void AddPresence(List<PresenceDAO> presenceList);
        public void UpdatePresence(List<PresenceDAO> updatedList);
        public List<PresenceDAO> GetPresenceByUserId(int groupId, DateOnly? date, int? userId);
    }
}

