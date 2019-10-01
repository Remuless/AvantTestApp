using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using testWebApplication.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace testWebApplication.Controllers
{
    [Route("api/[controller]")]
    public class ProjectController : Controller
    {
        public ProjectController(IProjectRepository projectItems)
        {
            ProjectItems = projectItems;
        }
        public IProjectRepository ProjectItems { get; set; }

		[HttpGet]
		public IEnumerable<Project> GetAll()
        {
            return ProjectItems.GetAll();
        }
        [HttpGet("{id}", Name = "GetProject")]
        public IActionResult GetById(Guid id)
        {
            var item = ProjectItems.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }
        [HttpPost]
        public IActionResult Create([FromBody] Project item)
        {
            if (item == null)
            {
                return BadRequest();
            }
			if (item.Name == null || item.startdate == null)
			{
				return BadRequest();
			}
            ProjectItems.Add(item);
            return CreatedAtRoute("GetProject", new { id = item.ID }, item);
        }
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] Project item)
        {
            if (item == null || item.ID != id)
            {
                return BadRequest();
            }

            var project = ProjectItems.Find(id);
            if (project == null)
            {
                return NotFound();
            }
			if (item.status == State.CLOSED && item.enddate == null)
			{
				return BadRequest();
			}
			if (project.status == State.CLOSED)
			{
				return BadRequest();
			}

            ProjectItems.Update(item);
            return new NoContentResult();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid[] id)
        {
			foreach (var identifier in id)
			{
				var project = ProjectItems.Find(identifier);
				if (project == null)
				{
					return NotFound();
				}
				if (project.status == State.CLOSED || project.status == State.DELETED)
				{
					return BadRequest();
				}

				ProjectItems.Remove(identifier);
			}
            return new NoContentResult();
        }

    }
}
