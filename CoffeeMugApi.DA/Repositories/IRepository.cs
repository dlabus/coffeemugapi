using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace CoffeeMugApi.DA.Repositories
{

    public interface IRepository<T> where T : class
    {
        Task<T> Get(Guid _);

        Task<IEnumerable<T>> GetAll();

        Task<Guid> Insert(T _);

        Task Update(Guid _, T __);

        Task Remove(Guid _);

    }
}