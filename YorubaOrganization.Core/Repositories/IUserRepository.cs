using YorubaOrganization.Core.Dto.Request;
using YorubaOrganization.Core.Dto.Response;
using YorubaOrganization.Core.Entities;

namespace YorubaOrganization.Core.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmail(string email);
        Task Create(User newUser);
        Task<bool> DeleteBy(string email);
        Task<bool> Update(string email, UpdateUserDto update);
        Task<IEnumerable<UserDto>> List();
        Task<int> CountUsers();
    }
}
