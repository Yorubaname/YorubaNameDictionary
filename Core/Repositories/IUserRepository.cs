using Core.Dto.Request;
using Core.Dto.Response;
using Core.Entities;

namespace Core.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmail(string email);
        Task Create(User newName);
        Task<bool> DeleteBy(string email);
        Task<bool> Update(string email, UpdateUserDto update);
        Task<IEnumerable<UserDto>> List();
    }
}
