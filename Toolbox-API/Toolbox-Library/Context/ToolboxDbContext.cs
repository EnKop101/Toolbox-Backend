using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Toolbox_Library.Models;

namespace Toolbox_Library.Context
{
    // Add-Migration InitDb -Context ToolboxDbContext -OutputDir "Migrations/Prod"
    // update-database

    public class ToolboxDbContext(DbContextOptions<ToolboxDbContext> options) : DbContext(options)
    {
        internal DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderUtility.ApplyModelBuilder(modelBuilder);
        }
    }
}
