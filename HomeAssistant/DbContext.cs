﻿using Microsoft.EntityFrameworkCore;


namespace HomeAssistant
{
    public class AppDbContext:DbContext
    {
      
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Room> Rooms { get; set; }
      
    }
}
