using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaStoreLibrary.library
{
    public interface IRepository<T>
    {
        T GetById(params int[] Id);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
