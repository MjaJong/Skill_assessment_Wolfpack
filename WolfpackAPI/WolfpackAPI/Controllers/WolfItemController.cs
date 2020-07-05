using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WolfAPI.Models;

namespace WolfAPI.Controllers
{
    [Route("api/WolfItems")]
    [ApiController]
    public class WolfItemsController : ControllerBase
    {
        private readonly WolfContext _context;

        public WolfItemsController(WolfContext context)
        {
            _context = context;
        }

        #region Wolf related methods.
        /// <summary>
        /// Gets a list of all wolves in the data base.
        /// </summary>
        /// <returns>A list of all wolves in the database.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WolfItem>>> GetWolfItems()
        {
            return await _context.WolfItems.ToListAsync();
        }

        /// <summary>
        /// Gets a wolf according to its database ID.
        /// </summary>
        /// <param name="id">The ID of the wolf to get.</param>
        /// <returns>A found wolf or a 404 if the ID matches no ID in teh database.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<WolfItem>> GetWolfItem(long id)
        {
            var wolfItem = await _context.WolfItems.FindAsync(id);

            if (wolfItem == null)
            {
                return NotFound();
            }

            return wolfItem;
        }

       /// <summary>
       /// Updates a wolf with the given ID with the new data.
       /// </summary>
       /// <param name="id">The ID of the wolf to update.</param>
       /// <param name="wolfItem">The new data. This has to match the given ID.</param>
       /// <returns>Http 204 if the wolf has been updated in the data base or the appropriate error code if somethign goes wrong.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWolfItem(long id, WolfItem wolfItem)
        {
            if (id != wolfItem.Id)
            {
                return BadRequest();
            }
            
            //TODO: VALIDATE & CONVERT GIVEN DATA.

            _context.Entry(wolfItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WolfItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a new wolf in the database.
        /// </summary>
        /// <param name="wolfItem">The wolf to create.</param>
        /// <returns>A code 201 and a copy of the created wolf including its ID in the database.</returns>
        [HttpPost]
        public async Task<ActionResult<WolfItem>> PostWolfItem(WolfItem wolfItem)
        {
            //TODO: VALIDATE & CONVERT GIVEN DATA.
            _context.WolfItems.Add(wolfItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWolfItem), new { id = wolfItem.Id }, wolfItem);
        }

        /// <summary>
        /// Deletes the wolf with the given ID.
        /// </summary>
        /// <param name="id">The ID to delete.</param>
        /// <returns>A copy of the deleted wolf or a error code if the ID could not be found.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<WolfItem>> DeleteWolfItem(long id)
        {
            var wolfItem = await _context.WolfItems.FindAsync(id);
            if (wolfItem == null)
            {
                return NotFound();
            }

            _context.WolfItems.Remove(wolfItem);
            await _context.SaveChangesAsync();

            return wolfItem;
        }

        /// <summary>
        /// Check to see if the database contains the given ID.
        /// </summary>
        /// <param name="id">The ID to look for.</param>
        /// <returns>True if any ID matches the given ID, otherwise false.</returns>
        private bool WolfItemExists(long id)
        {
            return _context.WolfItems.Any(wolf => wolf.Id == id);
        }

        private 
        #endregion
    }
}
