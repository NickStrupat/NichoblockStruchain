using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NodeWebApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration) => Configuration = configuration;

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) =>
			services
				.AddMvc()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

		// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
		IApplicationBuilder UseDevOrProd(IApplicationBuilder app, IHostingEnvironment env) => env.IsDevelopment() ? app.UseDeveloperExceptionPage() : app.UseHsts();

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env) =>
			UseDevOrProd(app, env)
				.UseHttpsRedirection()
				.UseMvc();
	}
}
