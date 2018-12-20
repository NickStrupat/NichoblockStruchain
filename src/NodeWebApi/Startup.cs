using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodeWebApi.DataModel;
using System;
using System.Collections.Generic;
using System.Net;

[assembly: ApiController]
namespace NodeWebApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration) => Configuration = configuration;

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddMvc()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			services
				.AddDbContext<Context>()
				.AddScoped<LocalRequestFilter>()
				.AddSingleton<HashSet<IPEndPoint>>();
		}

		IApplicationBuilder UseDevOrProd(IApplicationBuilder app, IHostingEnvironment env) =>
			env.IsDevelopment()
				? app.UseDeveloperExceptionPage()
				: app.UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.

		public void Configure(IApplicationBuilder app, IHostingEnvironment env) =>
			UseDevOrProd(app, env)
				.UseHttpsRedirection()
				.UseMvc(rb => rb.MapRoute("default", "{controller=Command}/{action=Status}/{id?}"))
				.UseNewDatabase<Context>()
				;//.UseSignalR(builder => builder.MapHub<BlockchainHub<String>>("/nodes"));
	}
}
