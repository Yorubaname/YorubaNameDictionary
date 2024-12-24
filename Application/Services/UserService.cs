using YorubaOrganization.Core.Dto.Request;
using YorubaOrganization.Core.Dto.Response;
using YorubaOrganization.Core.Entities;
using YorubaOrganization.Core.Repositories;
using BCrypt_ = BCrypt.Net.BCrypt;

namespace Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> CountUsers()
        {
            return await _userRepository.CountUsers();
        }

        public async Task<User> CreateUser(CreateUserDto createUserDto)
        {
            var user = new User
            {
                Roles = createUserDto.Roles.Select(r => r.ToUpper()).ToList(),
                Email = createUserDto.Email,
                Username = createUserDto.Username,
                Password = BCrypt_.HashPassword(createUserDto.Password, BCrypt_.GenerateSalt()),
                CreatedBy = createUserDto.CreatedBy
            };
            await _userRepository.Create(user);

            return user;
        }

        public async Task<bool> DeleteBy(string username)
        {
            return await _userRepository.DeleteBy(username);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _userRepository.GetUserByEmail(email);
        }

        public async Task<IEnumerable<UserDto>> List()
        {
            return await _userRepository.List();
        }

        public async Task<bool> Update(string email, UpdateUserDto update)
        {
            var hashedPassword = update.Password == null ? null : BCrypt_.HashPassword(update.Password, BCrypt_.GenerateSalt());
            return await _userRepository.Update(email, new UpdateUserDto(update.Username, hashedPassword, update.Roles, update.UpdatedBy));
        }
    }
}
