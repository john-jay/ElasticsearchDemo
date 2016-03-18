using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MusicStoreES.Models;
using Nest;

namespace MusicStoreES.Controllers
{
    public class StoreController : Controller
    {
        MusicStoreEntities storeDB = new MusicStoreEntities();

        public ActionResult Index()
        {
            var genres = storeDB.Genres.ToList();

            return View(genres);
        }

        // ELASTICSEARCH
        public ActionResult Search(string q)
        {
            var result = ElasticClient.Search<Album>(body =>
                    body.Query(query =>
                    query.QueryString(qs => 
                    qs.Query(q))));

            var genre = new Genre()
            {
                Name = "Search results for \"" + q + "\"",
                Albums = result.Documents.ToList()
            };

            return View("Browse", genre);
        }
        private static ElasticClient ElasticClient
        {
            get
            {
                var node = new Uri("http://localhost:9200");
                var settings = new ConnectionSettings(node, defaultIndex: "musicstore");
                return new ElasticClient(settings);
            }
        }
        //------------------

        //
        // GET: /Store/Browse?genre=Disco

        public ActionResult Browse(string genre)
        {
            // Retrieve Genre and its Associated Albums from database
            var genreModel = storeDB.Genres.Include("Albums")
                .Single(g => g.Name == genre);

            return View(genreModel);
        }

        //
        // GET: /Store/Details/5

        public ActionResult Details(int id)
        {
            var album = storeDB.Albums.Find(id);

            return View(album);
        }

        //
        // GET: /Store/GenreMenu

        [ChildActionOnly]
        public ActionResult GenreMenu()
        {
            var genres = storeDB.Genres.ToList();

            return PartialView(genres);
        }

    }
}