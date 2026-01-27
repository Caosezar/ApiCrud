using ApiCrud.Data.Repositories;
using ApiCrud.Models;

namespace ApiCrud.Services
{    
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _repository.GetAllUsersAsync();
        }
       public async Task GetAllAsync() { }
    }
}