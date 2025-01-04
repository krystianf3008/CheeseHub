using CheeseHub.Interfaces.Models;

namespace CheeseHub.Interfaces.Services
{
    public interface IBaseService<T> where T : IModelWithNameAndId
    {
        public Task<T?> GetById(Guid id);
        public Task<T?> GetByName(string name);
        public Task<List<T>> GetAll();
        public Task<Guid> Add(T model);
        public Task<Guid> Update(T model);
        public Task Delete(Guid id);
        public bool IsNameUnique(string name);

    }
}
