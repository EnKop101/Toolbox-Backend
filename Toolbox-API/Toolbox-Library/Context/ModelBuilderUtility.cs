using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Toolbox_Library.Models;

namespace Toolbox_Library.Context
{
    internal static class ModelBuilderUtility
    {
        public static void ApplyModelBuilder(ModelBuilder modelBuilder)
        {
            EnumModelBuilder(modelBuilder);
        }

        // Automatically converts the Enums to string values when it enters the database and back into enums when it is extracted.
        private static void EnumModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Property(ul => ul.Status).HasConversion<string>();
            modelBuilder.Entity<User>().Property(u => u.Role).HasConversion<string>();
        }
    }
}
