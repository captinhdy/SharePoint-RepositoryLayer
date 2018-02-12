using System;
using System.Collections.Generic;
using System.Linq;
using CrestServiceRepositoryLayer.Interfaces;
using System.Net;
using SP = Microsoft.SharePoint.Client;
using System.Web.Configuration;
using System.Diagnostics;

namespace CrestServiceRepositoryLayer
{
    public class CSOMSharePointRepository<TEntity>: ISharePointRepository<TEntity> where TEntity: class, new()
    {
        SP.ClientContext context;

        public CSOMSharePointRepository(string Url)
        {
            string userName = WebConfigurationManager.AppSettings["ServiceAccount"].ToString();
            string password = WebConfigurationManager.AppSettings["ServicePassword"].ToString();
            context = new SP.ClientContext(Url);
            context.Credentials = new NetworkCredential(userName, password);
            context.ExecutingWebRequest += context_ExecutingWebRequest;
        }

        public IEnumerable<TEntity> GetAll(string ListName)
        {
            try
            {
                List<TEntity> entries = new List<TEntity>();
                
                SP.Web web = context.Web;
                SP.List list = web.Lists.GetByTitle(ListName);
                SP.ListItemCollection items = list.GetItems(SP.CamlQuery.CreateAllItemsQuery());
                context.Load(items);
                context.ExecuteQuery();


                var properties = typeof(TEntity).GetProperties();
                foreach (SP.ListItem item in items)
                {
                    TEntity entry = new TEntity();

                    foreach (var property in properties)
                    {

                        if (item[property.Name] != null)
                        {
                            property.SetValue(entry, item[property.Name]);
                        }
                    }

                    entries.Add(entry);
                }

                return entries;
            }
            catch (InvalidCastException ex)
            {
                throw new Exception(ex.Message + ". Data model types are invalid, please verify that models are strongly typed and match the SharePoint list column types.", new InvalidCastException());
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting SharePoint list data: " + ex.Message);
            }

        }

        public TEntity GetItemById(string ListName, int Id)
        {
            try
            {
                TEntity entry = new TEntity();
                SP.Web web = context.Web;
                SP.List list = web.Lists.GetByTitle(ListName);
                SP.CamlQuery query = new SP.CamlQuery();
                query.ViewXml = @"<View>  
                                        <Query> 
                                           <Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>" + Id + @"</Value></Eq></Where> 
                                        </Query> 
                                  </View>";

                SP.ListItemCollection items = list.GetItems(query);
                context.Load(items);
                context.ExecuteQuery();

                if (items.Count > 0)
                {
                    SP.ListItem item = items[0];
                    var properties = typeof(TEntity).GetProperties();
                    foreach (var property in properties)
                    {
                        if (item[property.Name] != null)
                        {
                            property.SetValue(entry, item[property.Name]);
                        }
                    }

                }

                return entry;
            }
            catch (InvalidCastException ex)
            {
                throw new Exception(ex.Message + ". Data model types are invalid, please verify that models are strongly typed and match the SharePoint list column types.", new InvalidCastException());
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating SharePoint list data: " + ex.Message);
            }
        }

        public int AddItem(string ListName, TEntity Item)
        {
            try
            {
                SP.Web web = context.Web;
                SP.List list = web.Lists.GetByTitle(ListName);
                SP.ListItemCreationInformation createInfo = new SP.ListItemCreationInformation();
                SP.ListItem item = list.AddItem(createInfo);
                
                var properties = typeof(TEntity).GetProperties();

                foreach (var property in properties)
                {
                    if (property.PropertyType == typeof(bool?))
                    {
                        item[property.Name] = property.GetValue(Item) != null ? property.GetValue(Item) : -1;
                    }
                    else if (property.Name != "ID" && property.Name != "Created" && property.Name != "Author")
                    {
                        item[property.Name] = property.GetValue(Item);
                    }
                }

                item.Update();
                context.Load(item);
                context.ExecuteQuery();

                return item.Id;
            }
            catch (InvalidCastException ex)
            {
                throw new Exception(ex.Message + ". Data model types are invalid, please verify that models are strongly typed and match the SharePoint list column types.", new InvalidCastException());
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                throw new Exception("Error getting SharePoint list data: " + ex.Message);
            }
        }

        public void UpdateItem(string ListName, TEntity Item)
        {
            try
            {
                SP.Web web = context.Web;
                SP.List list = web.Lists.GetByTitle(ListName);

                var properties = typeof(TEntity).GetProperties();
                var id = properties.FirstOrDefault(f => f.Name == "ID");
                SP.ListItem item = list.GetItemById(Convert.ToInt32(id.GetValue(Item)));
                if(item != null)
                {
                        foreach (var property in properties)
                        {
                            if (property.Name != "ID" && property.Name != "Created" && property.Name != "Author")
                            {
                                item[property.Name] = property.GetValue(Item);
                            }
                        }

                        item.Update();
                        context.Load(item);

                    context.ExecuteQuery();
                }
                else
                {
                    throw new Exception("Item does not exist");

                }
            }
            catch (InvalidCastException ex)
            {
                throw new Exception(ex.Message + ". Data model types are invalid, please verify that models are strongly typed and match the SharePoint list column types.", new InvalidCastException());
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating SharePoint list data: " + ex.Message);
            }
        }

        public void UpdateItemByQuery(string ListName, TEntity Item, ICamlQuery Query)
        {
            try
            {
                SP.Web web = context.Web;
                SP.List list = web.Lists.GetByTitle(ListName);
                SP.CamlQuery query = (SP.CamlQuery)Query.ExecuteQuery();

                SP.ListItemCollection items = list.GetItems(query);
                context.Load(items);
                context.ExecuteQuery();

                if (items != null)
                {
                    foreach (SP.ListItem item in items)
                    {
                        var properties = typeof(TEntity).GetProperties();

                        foreach (var property in properties)
                        {
                            if (property.Name != "ID" && property.Name != "Created" && property.Name != "Author")
                            {
                                item[property.Name] = property.GetValue(Item);
                            }
                        }

                        item.Update();
                        context.Load(item);
                    }
                    context.ExecuteQuery();
                }
                else
                {
                    throw new Exception("Item does not exist");

                }
            }
            catch (InvalidCastException ex)
            {
                throw new Exception(ex.Message + ". Data model types are invalid, please verify that models are strongly typed and match the SharePoint list column types.", new InvalidCastException());
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating SharePoint list data: " + ex.Message);
            }
        }

        public void DeleteItem(string ListName, int Id)
        {
            try
            {
                SP.Web web = context.Web;
                SP.List list = web.Lists.GetByTitle(ListName);

                SP.ListItem item = list.GetItemById(Id);
                if (item != null)
                {
                    item.DeleteObject();
                    context.ExecuteQuery();
                }
                else
                {
                    throw new Exception("Item does not exist");

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating SharePoint list data: " + ex.Message);
            }
        }

        public void DeleteItemByQuery(string ListName, ICamlQuery Query)
        {
            try
            {
                SP.Web web = context.Web;
                SP.List list = web.Lists.GetByTitle(ListName);
                SP.CamlQuery query = (SP.CamlQuery)Query.ExecuteQuery();

                SP.ListItemCollection items = list.GetItems(query);
                context.Load(items);
                context.ExecuteQuery();

                if (items != null)
                {
                    foreach (SP.ListItem item in items)
                    {
                        item.DeleteObject();
                        context.Load(item);
                    }
                    context.ExecuteQuery();
                }
                else
                {
                    throw new Exception("Item does not exist");

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating SharePoint list data: " + ex.Message);
            }
        }

        public IEnumerable<TEntity> GetItemsByQuery(string ListName, ICamlQuery Query)
        {
            try
            {
                List<TEntity> entityItems = new List<TEntity>();
                SP.Web web = context.Web;
                SP.List list = web.Lists.GetByTitle(ListName);

                SP.CamlQuery query = (SP.CamlQuery)Query.ExecuteQuery();

                SP.ListItemCollection items = list.GetItems(query);
                context.Load(items);
                context.ExecuteQuery();

                foreach (SP.ListItem item in items)
                {
                    var properties = typeof(TEntity).GetProperties();

                    TEntity entry = new TEntity();

                    foreach (var property in properties)
                    {

                        if (item[property.Name] != null)
                        {
                            var i = item[property.Name];
                            property.SetValue(entry, item[property.Name]);
                        }
                    }

                    entityItems.Add(entry);
                }

                return entityItems;
            }
            catch (InvalidCastException ex)
            {
                throw new Exception(ex.Message + ". Data model types are invalid, please verify that models are strongly typed and match the SharePoint list column types.", new InvalidCastException());
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating SharePoint list data: " + ex.Message);
            }
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
            try
            {
                List<TEntity> entityItems = new List<TEntity>();
                SP.Site site = context.Site;
                context.Load(site);
                SP.Web web = context.Web;
                SP.List list = web.Lists.GetByTitle(ListName);
                SP.CamlQuery query = (SP.CamlQuery)Query.ExecuteQuery();

                SP.ListItemCollection items = list.GetItems(query);
                context.Load(items);
                context.ExecuteQuery();

                foreach (SP.ListItem item in items)
                {                  
                    var properties = typeof(TEntity).GetProperties();
                    TEntity entry = new TEntity();

                    foreach (var property in properties)
                    {

                            if (item[property.Name] != null)
                            {
                                property.SetValue(entry, item[property.Name]);
                            }
                    }

                    entityItems.Add(entry);
                }

                return entityItems;
            }
            catch (InvalidCastException ex)
            {
                throw new Exception(ex.Message + ". Data model types are invalid, please verify that models are strongly typed and match the SharePoint list column types.", new InvalidCastException());
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating SharePoint list data: " + ex.Message);
            }
        }

        public void UpdateDocument(string ListName, TEntity Document)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Client Object Model Login Helper
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void context_ExecutingWebRequest(object sender, SP.WebRequestEventArgs e)
        {
            e.WebRequestExecutor.WebRequest.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
        }
    }
}
