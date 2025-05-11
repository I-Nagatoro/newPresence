using data.RemoteData.RemoteDatabase.DAO;
using data.RemoteData.RemoteDataBase;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;


namespace data.Repository
{
    public class SQLGroupRepositoryImpl : IGroupRepository
    {
        private readonly RemoteDatabaseContext _remoteDatabaseContext;

        public SQLGroupRepositoryImpl(RemoteDatabaseContext remoteDatabaseContext)
        {
            _remoteDatabaseContext = remoteDatabaseContext;
        }
        public bool AddGroup(string groupName)
        {
            var GroupDAO = new GroupDAO
            {
                Id = _remoteDatabaseContext.Groups.Count() + 1,
                Name = groupName
            };
            _remoteDatabaseContext.Groups.Add(GroupDAO);
            _remoteDatabaseContext.SaveChanges();
            return true;
        }

        public void RemoveAllStudentsFromGroup(int groupId)
        {
            var group = _remoteDatabaseContext.Groups.Include(g => g.Users).FirstOrDefault(g => g.Id == groupId);
            if (group != null)
            {
                var userList = group.Users.ToList();
                foreach (var user in userList)
                {
                    _remoteDatabaseContext.Entry(user).State = EntityState.Deleted;
                }
                _remoteDatabaseContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException($"Группа с ID {groupId} не найдена.");
            }
        }

        public void AddStudentToGroup(int groupId, UserDAO student)
        {
            var group = _remoteDatabaseContext.Groups.Include(g => g.Users).FirstOrDefault(g => g.Id == groupId);
            if (group != null)
            {
                if (group.Users.Any(u => u.UserId == student.UserId))
                {
                    throw new ArgumentException($"Студент с GUID {student.UserId} уже добавлен в эту группу.");
                }

                _remoteDatabaseContext.Users.Add(student); 

                student.GroupId = group.Id;

                _remoteDatabaseContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException($"Группа с ID {groupId} не найдена.");
            }
        }

        public GroupDAO GetGroupById(int groupId)
        {
            var GroupDAO = _remoteDatabaseContext.Groups
                .Include(g => g.Users)
                .FirstOrDefault(g => g.Id == groupId);
            if (GroupDAO == null) return null;

            return new GroupDAO
            {
                Id = GroupDAO.Id,
                Name = GroupDAO.Name,
                Users = GroupDAO.Users.Select(u => new UserDAO
                {
                    UserId = u.UserId,
                    FIO = u.FIO
                }).ToList()
            };
        }
        
        public GroupDAO GetGroupByName(string groupName)
        {
            var GroupDAO = _remoteDatabaseContext.Groups
                .Include(g => g.Users)
                .FirstOrDefault(g => g.Name == groupName);
            if (GroupDAO == null) return null;

            return new GroupDAO
            {
                Id = GroupDAO.Id,
                Name = GroupDAO.Name,
                Users = GroupDAO.Users.Select(u => new UserDAO
                {
                    UserId = u.UserId,
                    FIO = u.FIO
                }).ToList()
            };
        }
                               
        public List<GroupDAO> GetAllGroups()
        {
            return _remoteDatabaseContext.Groups
            .Include(g => g.Users)
                .Select(g => new GroupDAO
                {
                    Id = g.Id,
                    Name = g.Name,
                    Users = g.Users.Select(u => new UserDAO
                    {
                        UserId = u.UserId,
                        FIO = u.FIO,
                        GroupId = g.Id
                    }).ToList()
                })
                .ToList();
        }

        public bool UpdateGroupById(int groupId, GroupDAO updatedGroup)
        {
            var GroupDAO = _remoteDatabaseContext.Groups
                .Include(g => g.Users)
                .FirstOrDefault(g => g.Id == groupId);
            if (GroupDAO == null) return false;

            GroupDAO.Name = updatedGroup.Name;
            GroupDAO.Users = updatedGroup.Users.Select(user => new UserDAO
            {
                UserId = user.UserId,
                FIO = user.FIO,
                GroupId = user.GroupId
            }).ToList();

            _remoteDatabaseContext.SaveChanges();
            return true;
        }

        public bool RemoveGroupById(int groupId)
        {
            var GroupDAO = _remoteDatabaseContext.Groups.Find(groupId);
            if (GroupDAO == null) return false;

            _remoteDatabaseContext.Groups.Remove(GroupDAO);
            _remoteDatabaseContext.SaveChanges();
            return true;
        }

        public void DeleteGroupById(int groupId)
        {
            var GroupDAO = _remoteDatabaseContext.Groups.Find(groupId);
            if (GroupDAO == null) return;
            _remoteDatabaseContext.Groups.Remove(GroupDAO);
            _remoteDatabaseContext.SaveChanges();
        }
        

    }
}
