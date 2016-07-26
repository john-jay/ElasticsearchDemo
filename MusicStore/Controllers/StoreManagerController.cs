using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MusicStoreES.Models;
using Nest;

namespace MusicStoreES.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class StoreManagerController : Controller
    {
        private MusicStoreEntities db = new MusicStoreEntities();

        //
        // GET: /StoreManager/

        public ViewResult Index()
        {
            var albums = db.Albums.Include(a => a.Genre).Include(a => a.Artist);
            return View(albums.ToList());
        }

        // ELASTICSEARCH
        public ActionResult ReIndex()
        {
            var client = ElasticClient;
            CreateIndex(client);
            InsertAlbums(client);

            TempData["Message"] = "Success";
            return RedirectToAction("Index");
        }
        private static ElasticClient ElasticClient
        {
            get
            {
                var node = new Uri("http://localhost:9200");
                var settings = new ConnectionSettings(node).DefaultIndex("musicstore");
                return new ElasticClient(settings);
            }
        }
        private void CreateIndex(ElasticClient client)
        {
            var indexSettings = new IndexSettings()
            {
                NumberOfReplicas = 0,
                NumberOfShards = 1
            };
            var state = new IndexState { Settings = indexSettings };

            client.CreateIndex("musicstore", c => c
                .InitializeUsing(state)
                .Mappings(ms => ms
                    .Map<Album>(m => m.AutoMap()))
                );
        }
        private void InsertAlbums(ElasticClient client)
        {
            foreach (var album in db.Albums)
            {
                client.Index(album, i => i
                    .Id(album.AlbumId));
            }
        }
        //--------------

        //
        // GET: /StoreManager/Details/5

        public ViewResult Details(int id)
        {
            Album album = db.Albums.Find(id);
            return View(album);
        }

        //
        // GET: /StoreManager/Create

        public ActionResult Create()
        {
            ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "Name");
            ViewBag.ArtistId = new SelectList(db.Artists, "ArtistId", "Name");
            return View();
        }

        //
        // POST: /StoreManager/Create

        [HttpPost]
        public ActionResult Create(Album album)
        {
            if (ModelState.IsValid)
            {
                db.Albums.Add(album);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "Name", album.GenreId);
            ViewBag.ArtistId = new SelectList(db.Artists, "ArtistId", "Name", album.ArtistId);
            return View(album);
        }

        //
        // GET: /StoreManager/Edit/5

        public ActionResult Edit(int id)
        {
            Album album = db.Albums.Find(id);
            ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "Name", album.GenreId);
            ViewBag.ArtistId = new SelectList(db.Artists, "ArtistId", "Name", album.ArtistId);
            return View(album);
        }

        //
        // POST: /StoreManager/Edit/5

        [HttpPost]
        public ActionResult Edit(Album album)
        {
            if (ModelState.IsValid)
            {
                db.Entry(album).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "Name", album.GenreId);
            ViewBag.ArtistId = new SelectList(db.Artists, "ArtistId", "Name", album.ArtistId);
            return View(album);
        }

        //
        // GET: /StoreManager/Delete/5

        public ActionResult Delete(int id)
        {
            Album album = db.Albums.Find(id);
            return View(album);
        }

        //
        // POST: /StoreManager/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Album album = db.Albums.Find(id);
            db.Albums.Remove(album);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}