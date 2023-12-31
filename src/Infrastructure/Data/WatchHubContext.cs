﻿using ApplicationCore.Entities;
using Infrastructure.Data.Config;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class WatchHubContext : DbContext
    {
        public WatchHubContext(DbContextOptions<WatchHubContext> options) : base(options)
        {
            
        }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Basket> Baskets => Set<Basket>();
        public DbSet<BasketItem> BasketItems => Set<BasketItem>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());  // Assembly => Bu projedeki dosyaları tara ve uygula

            //modelBuilder.ApplyConfiguration(new BrandConfiguration());
            //modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            //modelBuilder.ApplyConfiguration(new ProductConfiguration());            // Bu üçü yerine yukarıdaki tek kod aynı işi yapıyor
        }

    }
}
