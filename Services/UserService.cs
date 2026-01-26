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
        public async Task<User?> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID deve ser maior que 0");
            }
            return await _repository.GetUserByIdAsync(id);
        }
    }
}