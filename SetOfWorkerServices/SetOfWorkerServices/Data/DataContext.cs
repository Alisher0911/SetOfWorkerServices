using System;
using Microsoft.EntityFrameworkCore;
using SetOfWorkerServices.Models;

namespace SetOfWorkerServices.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
    }
}
