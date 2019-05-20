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
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(@"Data Source=C:\Users\mooaz\.nuget\packages\microsoft.entityframeworkcore.tools\2.2.4\tools\netcoreapp2.0\any\ef.dllDatabase.sqlite");           
        }
    }
}
