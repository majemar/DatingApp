using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        //konstruktor
        public DataContext(DbContextOptions<DataContext> options): base (options) { }

        //tworzy tabele dla modelu Value
        public DbSet<Value> Values { get; set; }
    }
}