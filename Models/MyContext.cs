using WeddingPlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) {}

        public DbSet<RegUser> Users {get;set;}
        public DbSet<Wedding> Weddings {get;set;}
        public DbSet<RSVP> RSVPs {get;set;}
    }
}