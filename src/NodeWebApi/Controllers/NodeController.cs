using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace NodeWebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class NodeController : ControllerBase
	{
		private readonly HashSet<IPEndPoint> nodes;
		public NodeController(HashSet<IPEndPoint> nodes) => this.nodes = nodes;

		public IEnumerable<String> Get() => nodes.Select(x => x.ToString());

		public void Post([FromBody] String value) {}
	}
}
