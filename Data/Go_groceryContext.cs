using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Go_grocery.Models;

namespace Go_grocery.Data
{
    public class Go_groceryContext : DbContext
    {
        public Go_groceryContext (DbContextOptions<Go_groceryContext> options)
            : base(options)
        {
        }

        public DbSet<Go_grocery.Models.Product> Product { get; set; }

        public DbSet<Go_grocery.Models.Courier> Courier { get; set; }
        public DbSet<Go_grocery.Models.ShopOwner> ShopOwner { get; set; }
        public DbSet<Go_grocery.Models.Customer> Customer { get; set; }
        public DbSet<Go_grocery.Models.Order> Order  { get; set; }


    }
}
