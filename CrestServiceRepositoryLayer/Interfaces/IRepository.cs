using System;
using System.Collections.Generic;

namespace CrestServiceRepositoryLayer.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class, new()
    {    
        IEnumerable<TEntity> GetAll(string ListName);
        TEntity GetItemById(string ListName, int Id);
        int AddItem(string ListName, TEntity Item);
        void UpdateItem(string ListName, TEntity Item);
        void DeleteItem(string ListName, int Id);
    }
}
