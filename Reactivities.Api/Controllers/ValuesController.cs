using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reactivities.Persistence;

namespace DatingApp.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ValuesController : ControllerBase
	{
		private readonly DataContext context;

		public ValuesController(DataContext dataContext)
		{
			this.context = dataContext;
		}

		// GET api/values
		[HttpGet]
		public async Task<ActionResult<IEnumerable<string>>> Get()
		{
			var values = await this.context.Values.ToListAsync();
			return Ok(values);
		}

		// GET api/values/5
		[HttpGet("{id}")]
		public async Task<ActionResult<string>> Get(int id)
		{
			var value = await this.context.Values.SingleOrDefaultAsync(value => value.Id == id);
			return Ok(value);
		}

		// POST api/values
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT api/values/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/values/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
