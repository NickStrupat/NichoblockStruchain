using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodeWebApi.DataModel;

namespace NodeWebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class NodeController : ControllerBase
	{
		private readonly HashSet<IPEndPoint> nodes;
		private readonly Context context;

		public NodeController(HashSet<IPEndPoint> nodes, Context context)
		{
			this.nodes = nodes;
			this.context = context;
		}

		public IEnumerable<String> Get() => nodes.Select(x => x.ToString());

		public async Task Post([FromBody] Node node)
		{
			var existing = await context.Nodes.SingleOrDefaultAsync(x => x.Id == node.Id, HttpContext.RequestAborted);
			if (existing != null)
				return;
			context.Add(node);
			await context.SaveChangesAsync(HttpContext.RequestAborted);
		}
	}
}
