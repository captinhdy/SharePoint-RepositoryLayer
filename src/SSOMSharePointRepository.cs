using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrestServiceRepositoryLayer.Interfaces;
using Microsoft.SharePoint;

namespace CrestServiceRepositoryLayer
{
    public class SSOMSharePointRepository<TEntity> : ISharePointRepository<TEntity> where TEntity : class, new()
    {
        SPSite site;
         public SSOMSharePointRepository(string Url)
        {
            site = new SPSite(Url);
        }

        public IEnumerable<TEntity> GetAll(string ListName)
        {
            throw new NotImplementedException();
        }

        public TEntity GetItemById(string ListName, int Id)
        {
            throw new NotImplementedException();
        }

        public int AddItem(string ListName, TEntity Item)
        {
            try
            {
                using (SPWeb web = site.OpenWeb())
                {
                    web.AllowUnsafeUpdates = true;
                    SPList list = web.Lists.TryGetList(ListName);

                    if (list != null)
                    {
                        SPListItem item = list.AddItem();
                        var properties = typeof(TEntity).GetProperties();

                        foreach (var property in properties)
                        {
                            if (property.PropertyType.ToString() == "Boolean")
                            {
                                item[property.Name] = (bool)property.GetValue(Item) == true ? 1 : 0;
                            }
                            else if (property.Name != "ID")
                            {
                                item[property.Name] = property.GetValue(Item);
                            }
                        }

                        item.Update();

                        return item.ID;
                    }
                    else
                    {
                        throw new Exception("List now found");
                    }

                    web.AllowUnsafeUpdates = false;
                }
            }
            catch (InvalidCastException ex)
            {
                Dispose();
                throw new Exception(ex.Message + ". Data model types are invalid, please verify that models are strongly typed and match the SharePoint list column types.", new InvalidCastException());
            }
            catch (Exception ex)
            {
                Dispose();
                throw new Exception("Error getting SharePoint list data: " + ex.Message);
            }
           
        }

        public void UpdateItem(string ListName, TEntity Item)
        {
            throw new NotImplementedException();
        }

        public void UpdateItemByQuery(string ListName, TEntity Item, ICamlQuery Query)
        {
            throw new NotImplementedException();
        }

        public void DeleteItem(string ListName, int Id)
        {
            throw new NotImplementedException();
        }

        public void DeleteItemByQuery(string ListName, ICamlQuery Query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> GetItemsByQuery(string ListName, ICamlQuery Query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> GetItemsByQuery(string ListName, ICamlQuery Query, string Include)
        {
            throw new NotImplementedException();
        }

        public void AddDocument(string ListName, TEntity Document)
        {
            throw new NotImplementedException();
        }

        public TEntity GetDocumentById(string ListName, int Id)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<TEntity> GetDocumentsByQuery(string ListName, ICamlQuery Query)
        {
            throw new NotImplementedException();
        }

        public void UpdateDocument(string ListName, TEntity Document)
        {
            throw new NotImplementedException();
        }
        public void Dispose()
        {
            site.Dispose();
        }
    }
}
