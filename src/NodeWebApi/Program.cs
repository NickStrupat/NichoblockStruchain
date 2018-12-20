using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace NodeWebApi
{
	public class Program
	{
		public static void Main(String[] args) =>
			WebHost
				.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.Build()
				.Run();
	}
}
