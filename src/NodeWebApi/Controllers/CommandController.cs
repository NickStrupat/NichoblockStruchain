using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NodeWebApi.DataModel;
using TheBlockchainTM;

namespace NodeWebApi.Controllers
{
	[ServiceFilter(typeof(LocalRequestFilter))]
	[Route("[controller]")]
	//[Route("api/[controller]")]
	//[ApiController]
	public class CommandController : ControllerBase
	{
		private readonly Context context;
		public CommandController(Context context) => this.context = context;

		[HttpGet("status")]
		public String Status() => "up";

		[HttpGet("node/{nodeId}")]
		public async Task<IActionResult> GetNode([FromRoute] Guid nodeId)
		{
			var node = await context.Nodes.SingleAsync(x => x.Id == nodeId, HttpContext.RequestAborted);
			//node.PrivateKey = null;
			return Ok(node);
		}

		[HttpPost("createNode")]
		public async Task<IActionResult> Create([FromBody] String name)
		{
			var node = new Node(name);
			context.Add(node);
			await context.SaveChangesAsync(HttpContext.RequestAborted);
			return Created($"node/{node.Id:N}", node);
		}

		[HttpPost("addData/{nodeId}")]
		public async Task<IActionResult> AddData([FromRoute] Guid nodeId, [FromBody] String data)
		{
			var node = await context.Nodes.SingleOrDefaultAsync(x => x.Id == nodeId, HttpContext.RequestAborted);
			if (node == null)
				return BadRequest("Node ID doesn't exist.");
			var block = new Block(data) {Node = node};
			block.Sign();
			node.Blocks.Add(block);
			await context.SaveChangesAsync(HttpContext.RequestAborted);
			return Created("url", null);
		}
	}
}