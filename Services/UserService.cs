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

        public Task GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = await _repository.GetAllUsersAsync();
            return users;
        }
    }
}