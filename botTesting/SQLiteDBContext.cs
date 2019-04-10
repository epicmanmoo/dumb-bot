using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace botTesting
{
    public class SQLiteDBContext : DbContext
    {
        public DbSet<Stone> Stones { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {                    
            string DbLocation = Assembly.GetEntryAssembly().Location.Replace(@"\bin\Debug\netcoreapps2.1", @"Data");
            options.UseSqlite($"Data Source={DbLocation}Database.sqlite");
            Console.WriteLine($"Data Source={DbLocation}Database.sqlite");
        }
    }
}
