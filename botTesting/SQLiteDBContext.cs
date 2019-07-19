using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace botTesting
{
    public class SQLiteDBContext : DbContext
    {
        public DbSet<Stone> Stones { get; set; }
        public DbSet<SpecificCMDS> Spclcmds { get; set; }
        public DbSet<Welcome> welcomes { get; set; }
        public DbSet<NamingThings> namings { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string[] lines = File.ReadAllLines(@"M:\token.txt");
            string loc = lines[1];
            options.UseSqlite(@"Data Source=" + loc);           
        }
    }
}
