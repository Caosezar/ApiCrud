using ApiCrud.Models;
using ApiCrud.Services;

namespace ApiCrud.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int id);
    }
}