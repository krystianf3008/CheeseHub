using CheeseHub.Data;
using CheeseHub.Interfaces.Models;
using CheeseHub.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
namespace CheeseHub.Services
{
    public class BaseService<T> : IBaseService<T> where T : class, IModelWithNameAndId
    {
        private readonly ApplicationDbContext _dBContext;


        public BaseService(ApplicationDbContext dBContext)
        {
            _dBContext = dBContext;
        }
        public async Task<Guid> Add(T model)
        {
            await _dBContext.Set<T>().AddAsync(model);
            await _dBContext.SaveChangesAsync();
            return model.Id;
        }

        public async Task Delete(Guid id)
        {

            _dBContext.Set<T>().Remove(await _dBContext.Set<T>().Where(x => x.Id == id).FirstOrDefaultAsync());
            await _dBContext.SaveChangesAsync();
        }
        public async Task<List<T>> GetAll()
        {
            return await _dBContext.Set<T>().ToListAsync();
        }

        public async Task<T?> GetById(Guid id)
        {
            return await _dBContext.Set<T>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }
        public async Task<T?> GetByName(string name)
        {
            return await _dBContext.Set<T>().Where(x => x.Name == name).FirstOrDefaultAsync();
        }
        public async Task<Guid> Update(T model)
        {
            _dBContext.Set<T>().Update(model);
            await _dBContext.SaveChangesAsync();
            return model.Id;
        }
        public bool IsNameUnique(string name)
        {
            return _dBContext.Set<T>().Where(x => x.Name == name).Count() == 0;
        }
    }
}
