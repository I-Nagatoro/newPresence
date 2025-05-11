using data.Exceptions;
using data.RemoteData.RemoteDataBase;
using data.RemoteData.RemoteDataBase.DAO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data.RemoteData.RemoteDatabase.DAO;

namespace data.Repository
{
    public class SQLUserRepositoryImpl : IUserRepository
    {
        private readonly RemoteDatabaseContext _remoteDatabaseContext;
        

        public SQLUserRepositoryImpl(RemoteDatabaseContext remoteDatabaseContext)
        {
            _remoteDatabaseContext = remoteDatabaseContext;
        }

        public async Task<bool> RemoveUserByIdAsync(int userId)
        {
            var user = await _remoteDatabaseContext.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);
            
            if (user == null) 
                throw new UserNotFoundException(userId);

            _remoteDatabaseContext.Users.Remove(user);
            await _remoteDatabaseContext.SaveChangesAsync();
            return true;
        }

        public UserDAO AddUser(UserDAO user)
        {
            _remoteDatabaseContext.Users.Add(user);
            _remoteDatabaseContext.SaveChanges();
            return user;
        }

        public async Task<UserDAO> AddUserAsync(UserDAO user)
        {
            await _remoteDatabaseContext.Users.AddAsync(user);
            await _remoteDatabaseContext.SaveChangesAsync();
            return user;
        }

        public async Task<UserDAO> UpdateUserAsync(UserDAO user)
        {
            var groupExists = await _remoteDatabaseContext.Groups
                .AnyAsync(g => g.Id == user.GroupId);
            
            if (!groupExists)
                throw new GroupNotFoundException(user.GroupId);

            var existingUser = await _remoteDatabaseContext.Users
                .FirstOrDefaultAsync(u => u.UserId == user.UserId);
            
            if (existingUser == null) 
                throw new UserNotFoundException(user.UserId);

            existingUser.FIO = user.FIO;
            existingUser.GroupId = user.GroupId;
            
            await _remoteDatabaseContext.SaveChangesAsync();
            return existingUser;
        }

        public async Task<List<UserDAO>> GetAllUsersAsync(bool trackEntities = false)
        {
            var query = _remoteDatabaseContext.Users
                .OrderBy(u => u.UserId)
                .AsQueryable();

            if (!trackEntities)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<List<UserDAO>> GetUserNamesAsync()
        {
            return await _remoteDatabaseContext.Users
                .Select(u => new UserDAO 
                { 
                    UserId = u.UserId, 
                    FIO = u.FIO 
                })
                .AsNoTracking()
                .ToListAsync();
        }
        
        public List<UserDAO> GetAllUsers()
        {
            return _remoteDatabaseContext.Users
                .OrderBy(u => u.UserId)
                .AsNoTracking()
                .ToList();
        }

        public bool RemoveUserById(int userId)
        {
            var user = _remoteDatabaseContext.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) throw new UserNotFoundException(userId);
    
            _remoteDatabaseContext.Users.Remove(user);
            _remoteDatabaseContext.SaveChanges();
            return true;
        }



        public List<UserDAO> GetUserNames()
        {
            return _remoteDatabaseContext.Users
                .Select(u => new UserDAO { UserId = u.UserId, FIO = u.FIO })
                .AsNoTracking()
                .ToList();
        }

        public UserDAO? GetUserById(int userId)
        {
            return _remoteDatabaseContext.Users
                .Include(u => u.Group)
                .Where(u => u.UserId == userId)
                .Select(u => new UserDAO 
                { 
                    UserId = u.UserId, 
                    FIO = u.FIO, 
                    GroupId = u.GroupId,
                    Group = new GroupDAO()
                    { 
                        Id = u.Group.Id, 
                        Name = u.Group.Name 
                    }
                })
                .AsNoTracking()
                .FirstOrDefault();
        }

        public List<UserDAO> GetUsersByGroupId(int groupId)
        {
            List<UserDAO> users = new List<UserDAO>();
            users = _remoteDatabaseContext.Users.Where(u => u.GroupId == groupId)
                .Select(u => new UserDAO { UserId = u.UserId, FIO = u.FIO, GroupId = u.GroupId })
                .ToList();
            return users;
        }
    }
}