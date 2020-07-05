using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WolfAPI.Models;

namespace WolfAPI.Controllers
{
    [Route("api/WolfItems")]
    [ApiController]
    public class WolfItemsController : ControllerBase
    {
        //Static collection of all numbers allowed to be used to denote gender per ISO/IEC 5218, with the addition of a 3 to allow for non binary to be selected as well.
        private static readonly int[] VALID_GENDERS = new int[] { 0, 1, 2, 3, 9 };

        //Coordinates under the ISO system have pre defined bounds we can check against, set here as constant values.
        private const float UPPER_LATITUDE_BOUND = 90f;
        private const float LOWER_LATITUDE_BOUND = -90f;
        private const float UPPER_LONGITUDE_BOUND = 180f;
        private const float LOWER_LONGITUDE_BOUND = -180f;

        private const char COORD_SEPARATOR = '/';

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
            
            if(VerifyGivenData(wolfItem) == false) { return UnprocessableEntity(); }

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
            if (VerifyGivenData(wolfItem) == false) { return UnprocessableEntity(); }
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

        /// <summary>
        /// Verifies that the given <see cref="WolfItem"/> contains the correct data. The method will exit early if any of the verfication steps fail.
        /// </summary>
        /// <param name="wolfToVerify">The <see cref="WolfItem"/> to verify the data of.</param>
        /// <returns>A boolean that is true if everything is correct, and false otherwise.</returns>
        private bool VerifyGivenData(WolfItem wolfToVerify)
        {
            bool dataIsValid = true;
            //TODO:
            //Verify that the birthdate is correct.
            dataIsValid = VerifyGivenDate(wolfToVerify.BirthDate);
            if (!dataIsValid) { return dataIsValid; }

            //Verify that the gender is one of the five allowed numbers (0, 1, 2, 3, 9).
            dataIsValid = VALID_GENDERS.Any(gender => wolfToVerify.Gender == gender);
            if (!dataIsValid) { return dataIsValid; }

            //Verify that the location data is correct.
            dataIsValid = VerifyLocationData(wolfToVerify.Location);
            if (!dataIsValid) { return dataIsValid; }

            return dataIsValid;
        }

        /// <summary>
        /// Verifies the given birth date.
        /// </summary>
        /// <param name="date">The data as a string to verify.</param>
        /// <returns>True if the date is valid, false otherwise.</returns>
        private bool VerifyGivenDate(string date)
        {
            DateTime dateToVerify;
            if(DateTime.TryParse(date, out dateToVerify) == false) { return false; } //The given string could not be parsed to a DateTime object, so there is something wrong with the given date.
            if(dateToVerify > DateTime.Today) { return false; } //The given date is later than the date of today, which birthdays can't be.
            return true;
        }

        /// <summary>
        /// Verifies the location of the wolf. Should it be desired, the float array can be returned with and out parameter.
        /// </summary>
        /// <param name="locationData">The float array reprisentation of the location.</param>
        /// <returns>True if everything is correct, false otherwise.</returns>
        private bool VerifyLocationData(string locationData)
        {
            //Convert string representation to float[]
            string[] splitString = locationData.Split(COORD_SEPARATOR);
            if (splitString.Count() < 2 || splitString.Count() > 3) { return false; }

            float[] locationAsFloat = new float[] { 0, 0, 0};

            for(int i = 0; i < splitString.Length; i++)
            {
                float result;
                if (float.TryParse(splitString[i], NumberStyles.Any, CultureInfo.InvariantCulture, out result)) { locationAsFloat[i] = result; }
                else { return false; } //Unparceable float value found.
            }

            for(int x = 0; x < 2; x++) //We always check the first two elements, as these are latitude and longitude under the ISO.
            {
                if(x == 0) { if(locationAsFloat[x] < LOWER_LATITUDE_BOUND || locationAsFloat[x] > UPPER_LATITUDE_BOUND) { return false; } }
                else { if (locationAsFloat[x] < LOWER_LONGITUDE_BOUND || locationAsFloat[x] > UPPER_LONGITUDE_BOUND) { return false; } }
            }

            return true;
        }
        #endregion
    }
}
