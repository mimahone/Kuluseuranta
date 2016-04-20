using Kuluseuranta.Objects;
using System.Data.Entity;

namespace Kuluseuranta.DB
{
  public class PaymentsContext : DbContext
  {
    public PaymentsContext() : base("name=Kuluseuranta.Properties.Settings.ConnectionString")
    {

    }

    public DbSet<User> Users { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Payment> Payments { get; set; }

    //protected override void OnModelCreating(DbModelBuilder modelBuilder)
    //{
    //  //Configure domain classes using Fluent API here

    //  base.OnModelCreating(modelBuilder);
    //}

  }
}
