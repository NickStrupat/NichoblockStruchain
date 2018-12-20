using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NodeWebApi.DataModel;

namespace NodeWebApi.Controllers
{
	[Route("[controller]")]
	public class BlockController : ControllerBase
	{
		private readonly Context context;
		public BlockController(Context context) => this.context = context;

		[HttpGet]
		public String Get() => "hello";

		[HttpPut]
		public async Task Add([FromBody] Block block)
		{
			context.Add(block);
			await context.SaveChangesAsync();
		}
	}
}
