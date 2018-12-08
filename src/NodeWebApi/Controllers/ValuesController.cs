using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace NodeWebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ValuesController : ControllerBase
	{
		// GET api/values
		[HttpGet]
		public ActionResult<IEnumerable<String>> Get() => new String[] { "value1", "value2" };

		// GET api/values/5
		[HttpGet("{id}")]
		public ActionResult<String> Get(Int32 id) => "value";

		// POST api/values
		[HttpPost]
		public void Post([FromBody] String value)
		{
		}

		// PUT api/values/5
		[HttpPut("{id}")]
		public void Put(Int32 id, [FromBody] String value)
		{
		}

		// DELETE api/values/5
		[HttpDelete("{id}")]
		public void Delete(Int32 id)
		{
		}
	}
}
