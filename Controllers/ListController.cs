using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ListApi.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ListApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ListController : Controller
    {
        private readonly ListContext _context;

        public ListController(ListContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<ListItem> GetAll()
        {
            var owner = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
            return _context.ListItems.Where(t => t.Owner == owner).ToList();
        }

        [HttpGet("{id}", Name = "GetList")]
        public IActionResult GetById(long id)
        {
            var owner = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
            var item = _context.ListItems.FirstOrDefault(t => (t.ListItemId == id && t.Owner == owner) );

            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody] ListItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            var owner = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
            item.Owner = owner;

            _context.ListItems.Add(item);
            _context.SaveChanges();

            return CreatedAtRoute("GetList", new { id = item.ListItemId }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] ListItem item)
        {
            if (item == null || item.ListItemId != id)
            {
                return BadRequest();
            }

            var owner = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
            var listitem = _context.ListItems.FirstOrDefault(t => (t.ListItemId == id && t.Owner == owner));

            if (listitem == null)
            {
                return NotFound();
            }

            listitem.Completed = item.Completed;
            listitem.Description = item.Description;

            _context.ListItems.Update(listitem);
            _context.SaveChanges();
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var owner = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
            var listitem = _context.ListItems.FirstOrDefault(t => (t.ListItemId == id && t.Owner == owner));

            if (listitem == null)
            {
                return NotFound();
            }

            _context.ListItems.Remove(listitem);
            _context.SaveChanges();
            return new NoContentResult();
        }
    }
}
