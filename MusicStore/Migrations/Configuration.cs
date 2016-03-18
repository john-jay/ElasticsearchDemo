using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MusicStoreES.Models;

namespace MusicStore.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MusicStoreES.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MusicStoreES.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //context.People.AddOrUpdate(
            //  p => p.FullName,
            //  new Person { FullName = "Andrew Peters" },
            //  new Person { FullName = "Brice Lambson" },
            //  new Person { FullName = "Rowan Miller" }
            //);

            context.Roles.AddOrUpdate(r => r.Name,
                new IdentityRole() { Name = "Administrator" }
                );

            if (!context.Users.Any(u => u.UserName == "admin@me.com"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser
                {
                    UserName = "admin@me.com"
                };
                manager.Create(user, "password123!");

                manager.AddToRole(user.Id, "Administrator");
            }

        }
    }
}
