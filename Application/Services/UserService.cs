using Core.Dto.Response;
using Core.Entities;
using Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<User> CreateUser(CreateUserDto createUserDto)
        {
            var user = new User
            {
                Roles = createUserDto.Roles.ToList(),
                Email = createUserDto.Email,
                Username = createUserDto.Username,
                Password = BCrypt_.HashPassword(createUserDto.Password, BCrypt_.GenerateSalt())
            };
            await _userRepository.Create(user);

            return user;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _userRepository.GetUserByEmail(email);
        }
    }
}
