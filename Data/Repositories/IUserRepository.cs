using ApiCrud.Models;

namespace ApiCrud.Data.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(int id);
    }
}