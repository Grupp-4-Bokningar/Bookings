namespace Adminprototype
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class TestModell : DbContext
    {
        public TestModell()
            : base("name=TestModell")
        {
        }

        public virtual DbSet<Bookings> Bookings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bookings>()
                .Property(e => e.User_Type)
                .IsUnicode(false);
        }
    }
}
