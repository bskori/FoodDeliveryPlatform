using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDelivery.Data.Repositories.Interfaces
{
    public interface IGenricRepository<T>
        where T : class
    {
        Task<List<T>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
