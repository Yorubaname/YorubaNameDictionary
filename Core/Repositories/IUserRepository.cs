using Core.Dto.Request;
using Core.Entities;
using Core.Entities.NameEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmail(string email);
        Task Create(User newName);
        Task<bool> DeleteBy(string email);
        Task<bool> Update(string email, UpdateUserDto update);
    }
}
