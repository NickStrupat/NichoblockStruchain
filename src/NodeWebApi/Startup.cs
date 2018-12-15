using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheBlockchainTM;

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
				.AddSignalR();

			services
				.AddSingleton<Blockchain<String>>(ImplementationFactory)
				.AddSingleton<ConcurrentDictionary<HubConnection, DateTime>>();
		}

		private Blockchain<String> ImplementationFactory(IServiceProvider sp)
		{
			var hubConnections = sp.GetRequiredService<ConcurrentDictionary<HubConnection, DateTime>>();
			if (hubConnections.Count == 0)
				return new Blockchain<String>("genesis");
			var hubConnection = hubConnections.Keys.First();
			var mres = new ManualResetEventSlim(initialState:false);
			Blockchain<String> theirBlockchain = null;
			hubConnection.On("HeresMyBlockchain", (Blockchain<String> blockchain) =>
			{
				Console.WriteLine("HeresMyBlockchain");
				theirBlockchain = blockchain;
				mres.Set();
			});
			hubConnection.SendAsync("GiveMeYourBlockchain");
			mres.Wait();
			mres.Dispose();
			return theirBlockchain;
		}

		// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
		IApplicationBuilder UseDevOrProd(IApplicationBuilder app, IHostingEnvironment env) => env.IsDevelopment() ? app.UseDeveloperExceptionPage() : app.UseHsts();

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env) =>
			UseDevOrProd(app, env)
				.UseHttpsRedirection()
				.UseMvc()
				.UseSignalR(builder => builder.MapHub<BlockchainHub<String>>("/nodes"));
	}
}
