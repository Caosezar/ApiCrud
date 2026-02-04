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
        

        public async Task<User> UpdateUserAsync(int id, User user)
        {
            var existingUser = await _repository.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                throw new KeyNotFoundException($"Produto com ID {id} não encontrado");
            }

            if (string.IsNullOrWhiteSpace(user.FirstName))
            {
                throw new ArgumentException("Nome usuário é obrigatório");
            }

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.Phone = user.Phone;
            existingUser.BirthDate = user.BirthDate;
            existingUser.IsActive = user.IsActive;
            existingUser.CreatedAt = user.CreatedAt;

            await _repository.UpdateUserAsync(existingUser);
            return existingUser;
        }
    } 
}