using System;
using System.Collections.Generic;
using CrestServiceRepositoryLayer.Interfaces;

namespace CrestServiceRepositoryLayer.Interfaces
{
    public interface ISharePointRepository<TEntity> : IRepository<TEntity> where TEntity: class, new()
    {
        void UpdateItemByQuery(string ListName, TEntity Item, ICamlQuery Query);
        IEnumerable<TEntity> GetItemsByQuery(string ListName, ICamlQuery Query);
        void AddDocument(string ListName, TEntity Document);
        TEntity GetDocumentById(string ListName, int Id);
        IEnumerable<TEntity> GetDocumentsByQuery(string ListName, ICamlQuery Query);
        void UpdateDocument(string ListName, TEntity Document);
    }
}
