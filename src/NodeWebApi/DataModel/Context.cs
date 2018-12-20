using Microsoft.EntityFrameworkCore;

namespace NodeWebApi.DataModel
{
	public class Context : DbContext
	{
		public virtual DbSet<Node> Nodes { get; set; }
		public virtual DbSet<Block> Blocks { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite($"Filename={GetType().FullName}.db");
			//optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Block>()
				.HasIndex(b => b.Timestamp);

			modelBuilder.Entity<Block>()
				.HasOne(b => b.Node)
				.WithMany(n => n.Blocks)
				.HasForeignKey(b => b.NodeId)
				.HasPrincipalKey(n => n.Id);

			modelBuilder.Entity<Block>()
				.HasKey(b => new {b.Id, b.NodeId});

			modelBuilder.Entity<Node>()
				.HasIndex(n => n.Name)
				.IsUnique();
		}
	}
}
