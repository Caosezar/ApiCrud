using ApiCrud.Models;

namespace ApiCrud.Services
{
    public interface IUserService
    {
        Task GetAllAsync();
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
