using data.Exceptions;
using data.RemoteData.RemoteDataBase;
using data.RemoteData.RemoteDataBase.DAO;
using data.Repository;
using data.RemoteData.RemoteDataBase.DAO;
using data.RemoteData.RemoteDatabase.DAO;
namespace data.Domain.UseCase
{
    public class UserUseCase
    {
        private readonly IUserRepository _repositoryUserImpl;
        private readonly IGroupRepository _repositoryGroupImpl;
        public UserUseCase(IUserRepository repositoryImpl, IGroupRepository repositoryGroupImpl, IPresenceRepository presenceRepository)
        {
            _repositoryUserImpl = repositoryImpl;
            _repositoryGroupImpl = repositoryGroupImpl;
        }

        private void ValidateUserFIO(string fio)
        {
            if (string.IsNullOrWhiteSpace(fio))
            {
                throw new ArgumentException("ФИО не может быть пустым.");
            }
        }



        private UserDAO ValidateUserExistence(int userId)
        {
            var user = _repositoryUserImpl.GetAllUsers()
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                throw new Exception("Пользователь не найден.");
            }

            return user;
        }

        private GroupDAO ValidateGroupExistence(int groupId)
        {
            var group = _repositoryGroupImpl.GetAllGroups()
                .FirstOrDefault(g => g.Id == groupId);

            if (group == null)
            {
                throw new Exception("Группа не найдена.");
            }

            return group;
        }

        public List<UserDAO> GetAllUsers() => _repositoryUserImpl.GetAllUsers()
            .Join(_repositoryGroupImpl.GetAllGroups(),
            user => user.GroupId, 
            group => group.Id, 
            (user, group) => 
            new UserDAO
            {
                UserId = user.UserId,
                FIO = user.FIO,
                GroupId = group.Id
            }).ToList();

        public bool RemoveUserById(int userId)
        {
            try
            {
                return _repositoryUserImpl.RemoveUserById(userId);
            }
            catch (UserNotFoundException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return false;
            }
            catch (RepositoryException ex)
            {
                Console.WriteLine($"Ошибка в репозитории: {ex.Message}");
                return false;
            }
        }
        
        public UserDAO AddUser(UserDAO user)
        {
            if (string.IsNullOrWhiteSpace(user.FIO))
                throw new InvalidDataException("ФИО не может быть пустым");
        
            if (user.GroupId <= 0)
                throw new InvalidDataException("Неверный идентификатор группы");

            return _repositoryUserImpl.AddUser(user);
        }

        public async Task AddUserAsync(UserDAO user)
        {
            if (string.IsNullOrWhiteSpace(user.FIO))
                throw new InvalidDataException("ФИО не может быть пустым");
        
            if (user.GroupId <= 0)
                throw new InvalidDataException("Неверный идентификатор группы");

            await _repositoryUserImpl.AddUserAsync(user);
        }

        public async Task UpdateUserAsync(int userId, string fio, int groupId)
        {
            UserDAO user = new UserDAO()
            {
                UserId = userId,
                FIO = fio,
                GroupId = groupId
            };
            await _repositoryUserImpl.UpdateUserAsync(user);
        }

        public UserDAO UpdateUser(int userId, string newFio, int groupId)
        {
            ValidateUserFIO(newFio);
            ValidateGroupExistence(groupId);

            UserDAO UserDao = new UserDAO
            {
                UserId = userId,
                FIO = newFio,
                GroupId = groupId
            };

            var result =  _repositoryUserImpl.UpdateUserAsync(UserDao);

            if (result == null)
            {
                throw new Exception("Ошибка при обновлении пользователя.");
            }

            var groupEntity = ValidateGroupExistence(result.Id);

            return new UserDAO
            {
                UserId = userId,
                FIO = newFio,
                GroupId = groupId
            };

        }

        public UserDAO? FindUserById(int userId)
        {
            return _repositoryUserImpl.GetUserById(userId);
        }
        
    }
}
