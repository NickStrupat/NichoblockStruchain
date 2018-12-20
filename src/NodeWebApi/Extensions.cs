using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodeWebApi.DataModel;

namespace NodeWebApi
{
	public static class Extensions
	{
		public static IApplicationBuilder UseNewDatabase<TDbContext>(this IApplicationBuilder app) where TDbContext : DbContext
		{
			using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
			using (var context = serviceScope.ServiceProvider.GetRequiredService<Context>())
			{
				context.Database.EnsureDeleted();
				context.Database.EnsureCreated();
				return app;
			}
		}
	}
}