using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrestServiceRepositoryLayer;
using CrestDataLayer.Models;

namespace CrestServiceRepository_UnitTests
{
    [TestClass]
    public class SharePointRepositoryUnitTests
    {
        string testUrl = "http://testing.crest-management.com/communities/BayColonyParksideCA";
        [TestMethod]
        public void TestGetAllItems()
        {
            CSOMSharePointRepository<CommunityEventModel> events = new CSOMSharePointRepository<CommunityEventModel>(testUrl);
            string listName = "CommunityEvents";
            List<CommunityEventModel> e = events.GetAll(listName) as List<CommunityEventModel>;

            Assert.IsNotNull(e);
        }

        [TestMethod]
        public void TestGetById()
        {
            CSOMSharePointRepository<CommunityEventModel> events = new CSOMSharePointRepository<CommunityEventModel>(testUrl);
            string listName = "CommunityEvent";
            int id = 1;
            CommunityEventModel e = events.GetItemById(listName, id);

            Assert.IsNotNull(e);
        }

        [TestMethod]
        public void TestAddItem()
        {
            CSOMSharePointRepository<CommunityEventModel> events = new CSOMSharePointRepository<CommunityEventModel>(testUrl);
            string listName = "CommunityEvents";
            CommunityEventModel e = new CommunityEventModel()
            {
                Title = "Created from test",
                EventDate = DateTime.Now,
                EndDate = DateTime.Now.AddHours(2),
                Description = "Hello from my unit test",
                ArticleByLine = "This is a unit test created item.",
                Location = "Crest",
                ShowOnCalendar = "Yes",
                Category = "",
                EventImage = ""
            };

            events.AddItem(listName, e);
        }

        [TestMethod]
        public void UpdateItem()
        {
            CSOMSharePointRepository<CommunityEventModel> events = new CSOMSharePointRepository<CommunityEventModel>(testUrl);
            string listName = "CommunityEvents";
            CommunityEventModel e = new CommunityEventModel()
            {
                ID=13,
                Title = "Created from test",
                EventDate = DateTime.Now,
                EndDate = DateTime.Now.AddHours(2),
                Description = "Hello this item was updated from my unit test - UpdateItem",
                ArticleByLine = "This is a unit test created item.",
                Location = "Crest",
                ShowOnCalendar = "Yes",
                Category = "",
                EventImage = ""
            };

            events.UpdateItem(listName, e);
        }

        [TestMethod]
        public void UpdateItemByQuery()
        {
            CSOMSharePointRepository<CommunityEventModel> events = new CSOMSharePointRepository<CommunityEventModel>(testUrl);
            string listName = "CommunityEvents";
            CommunityEventModel e = new CommunityEventModel()
            {
                ID = 14,
                Title = "Created from test",
                EventDate = DateTime.Now,
                EndDate = DateTime.Now.AddHours(2),
                Description = "Hello this item was updated from my unit test - UpdateItemByQuery",
                ArticleByLine = "This is a unit test created item.",
                Location = "Crest",
                ShowOnCalendar = "Yes",
                Category = "",
                EventImage = ""
            };

            CSOMCamlQuery query = new CSOMCamlQuery();
            query.Caml = "<Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>" + e.ID + "</Value></Eq></Where>";
            events.UpdateItemByQuery(listName, e, query);
        }

        [TestMethod]
        public void DeleteItem()
        {
            CSOMSharePointRepository<CommunityEventModel> events = new CSOMSharePointRepository<CommunityEventModel>(testUrl);
            string listName = "CommunityEvents";
            int id = 2;

            events.DeleteItem(listName, id);
        }

        [TestMethod]
        public void GetItemsByQuery()
        {
            IEnumerable<CommunityEventModel> events;
            CSOMSharePointRepository<CommunityEventModel> repository = new CSOMSharePointRepository<CommunityEventModel>(testUrl);
            string listName = "CommunityEvents";
            CSOMCamlQuery query = new CSOMCamlQuery();
            query.Caml = "<Where><Eq><FieldRef Name='ShowOnCalendar' /><Value Type='Choice'>Yes</Value></Eq></Where>";

            events = repository.GetItemsByQuery(listName, query);
            Assert.IsNotNull(events);

            if (events.Count() <= 0)
            {
                Assert.Fail("No items were returned");
            }
        }

        [TestMethod]
        public void GetItemsByQueryWithOrder()
        {
            IEnumerable<CommunityEventModel> events;
            CSOMSharePointRepository<CommunityEventModel> repository = new CSOMSharePointRepository<CommunityEventModel>(testUrl);
            string listName = "CommunityEvents";
            CSOMCamlQuery query = new CSOMCamlQuery();
            query.Caml = "<Where><Eq><FieldRef Name='ShowOnCalendar' /><Value Type='Choice'>Yes</Value></Eq></Where>";
            query.OrderByFields = "<FieldRef Name='Title' />";

            events = repository.GetItemsByQuery(listName, query);
            Assert.IsNotNull(events);

            if (events.Count() <= 0)
            {
                Assert.Fail("No items were returned");

            }
            else
            {
                CommunityEventModel e = events.First();
                Assert.AreEqual("A", e.Title.Substring(0, 1).ToUpper());
            }


        }

        [TestMethod]
        public void GetItemsByQueryWithViewFields()
        {
            IEnumerable<CommunityEventModel> events;
            CSOMSharePointRepository<CommunityEventModel> repository = new CSOMSharePointRepository<CommunityEventModel>(testUrl);
            string listName = "CommunityEvents";
            CSOMCamlQuery query = new CSOMCamlQuery();
            query.Caml = "<Where><Eq><FieldRef Name='ShowOnCalendar' /><Value Type='Choice'>Yes</Value></Eq></Where>";
            query.ViewFields = "<FieldRef Name='Title' />";

            events = repository.GetItemsByQuery(listName, query);
            Assert.IsNotNull(events);

            if (events.Count() <= 0)
            {
                Assert.Fail("No items were returned");
            }
        }

        [TestMethod]
        public void GetItemsByQueryWithRowLimit()
        {
            IEnumerable<CommunityEventModel> events;
            CSOMSharePointRepository<CommunityEventModel> repository = new CSOMSharePointRepository<CommunityEventModel>(testUrl);
            string listName = "CommunityEvents";
            int rowLimit = 1;
            CSOMCamlQuery query = new CSOMCamlQuery();
            query.Caml = "<Where><Eq><FieldRef Name='ShowOnCalendar' /><Value Type='Choice'>Yes</Value></Eq></Where>";
            query.RowLimit = rowLimit;

            events = repository.GetItemsByQuery(listName, query);
            Assert.IsNotNull(events);
            Assert.AreEqual(rowLimit, events.Count());
        }

        [TestMethod]
        public void GetItemsByQueryAndInclude()
        {
            CSOMSharePointRepository<CommunityEventModel> events = new CSOMSharePointRepository<CommunityEventModel>(testUrl);
            string listName = "CommunityEvents";
            CSOMCamlQuery query = new CSOMCamlQuery();
            string include = "";
            IEnumerable<CommunityEventModel> e = events.GetItemsByQuery(listName, query, include);

            Assert.IsNotNull(e);
        }
    }
}
