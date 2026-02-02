using ApiCrud.Data.Repositories;
using ApiCrud.Models;

namespace ApiCrud.Services
{
    //busca de usuário por id
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
        //atualização de usuário
        public async Task<User> UpdateUserAsync(int id, User user)
        {
            var existingProduct = await _repository.GetUserByIdAsync(id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Produto com ID {id} não encontrado");
            }
            
            if (string.IsNullOrWhiteSpace(user.FirstName))
            {
                throw new ArgumentException("Nome usuário é obrigatório");
            }

           existingProduct.FirstName = user.FirstName;
           existingProduct.LastName = user.LastName;
           existingProduct.Email = user.Email;
           existingProduct.Phone = user.Phone;
           existingProduct.BirthDate = user.BirthDate;
           existingProduct.IsActive = user.IsActive;
           existingProduct.CreatedAt = user.CreatedAt;

            await _repository.UpdateUserAsync(existingProduct);
            return existingProduct;
        }
    }
}