using System;
using System.Net.Http;
using EntryPoint;

namespace Console
{
	class Program
	{
		static void Main(String[] args) => Cli.Execute<Commands>(args);
	}

	class Commands : BaseCliCommands
	{
		[Command("start")]
		public void Start(String[] args)
		{
			var arguments = Cli.Parse<StartArguments>();
			
		}

		[Command("stop")]
		public void Stop(String[] args) => Cli.Parse<StopArguments>();

		[Command("status")]
		public void Status(String[] args)
		{
			using (var httpClient = new HttpClient())
				System.Console.WriteLine(httpClient.GetStringAsync("http://localhost:50154/api/command/status").Result);
		}
	}

	class StatusArguments : BaseCliArguments
	{
		public StatusArguments() : base(nameof(StatusArguments)) { }
	}

	class StartArguments : BaseCliArguments
	{
		public StartArguments() : base(nameof(StartArguments)) { }

		[OptionParameter("uri", 'u')]
		public String Uri { get; set; }
	}

	class StopArguments : BaseCliArguments
	{
		public StopArguments() : base(nameof(StartArguments)) { }
	}
}
