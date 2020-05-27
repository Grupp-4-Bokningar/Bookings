namespace SubscriptionService.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class SubscriptionModel : DbContext
    {
        public SubscriptionModel()
            : base("name=SubscriptionModel")
        {
        }

        public virtual DbSet<Subscriptions> Subscriptions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
