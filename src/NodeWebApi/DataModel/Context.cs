using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
			InvokeEntityTypeBuilderMethods(modelBuilder);
		}

		private void InvokeEntityTypeBuilderMethods(ModelBuilder modelBuilder)
		{
			var entityTypes =
				from propertyInfo in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
				where propertyInfo.PropertyType.IsGenericType &&
				      propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)
				select propertyInfo.PropertyType.GetGenericArguments().Single();

			var entityMethodInfo = new Func<EntityTypeBuilder<Object>>(modelBuilder.Entity<Object>).Method
				.GetGenericMethodDefinition();
			var modelCreatingTypes =
				from entityType in entityTypes
				let methodInfos = entityType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				let firstMethodInfo =
					methodInfos.FirstOrDefault(x =>
						x.ReturnType == typeof(void) &&
						x.GetParameters().Count(y =>
							y.ParameterType == typeof(EntityTypeBuilder<>).MakeGenericType(entityType)
						) == 1
					)
				where firstMethodInfo != null
				let entityTypeBuilder =
					entityMethodInfo.MakeGenericMethod(entityType).Invoke(modelBuilder, Array.Empty<Object>())
				select (EntityTypeBuilder: entityTypeBuilder, MethodInfo: firstMethodInfo);

			foreach (var (entityTypeBuilder, methodInfo) in modelCreatingTypes)
				methodInfo.Invoke(null, new[] {entityTypeBuilder});
		}
	}
}
