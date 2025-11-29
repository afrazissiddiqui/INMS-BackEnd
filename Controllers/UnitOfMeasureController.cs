using Inventory.Api.Data;
using Inventory.Api.Domain;
using Inventory.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitOfMeasureController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UnitOfMeasureController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/UnitOfMeasure
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnitOfMeasureDto>>> GetAll()
        {
            var units = await _db.UnitsOfMeasure
                .Select(u => new UnitOfMeasureDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Abbreviation = u.Abbreviation
                }).ToListAsync();

            return Ok(units);
        }

        // GET: api/UnitOfMeasure/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UnitOfMeasureDto>> GetById(int id)
        {
            var unit = await _db.UnitsOfMeasure.FindAsync(id);

            if (unit == null) return NotFound();

            return Ok(new UnitOfMeasureDto
            {
                Id = unit.Id,
                Name = unit.Name,
                Abbreviation = unit.Abbreviation
            });
        }

        // POST: api/UnitOfMeasure
        [HttpPost]
        public async Task<ActionResult<UnitOfMeasureDto>> Create(CreateUnitOfMeasureDto dto)
        {
            var unit = new UnitOfMeasure
            {
                Name = dto.Name,
                Abbreviation = dto.Abbreviation
            };

            _db.UnitsOfMeasure.Add(unit);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = unit.Id }, new UnitOfMeasureDto
            {
                Id = unit.Id,
                Name = unit.Name,
                Abbreviation = unit.Abbreviation
            });
        }

        // PUT: api/UnitOfMeasure/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateUnitOfMeasureDto dto)
        {
            var unit = await _db.UnitsOfMeasure.FindAsync(id);
            if (unit == null) return NotFound();

            unit.Name = dto.Name;
            unit.Abbreviation = dto.Abbreviation;

            _db.UnitsOfMeasure.Update(unit);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/UnitOfMeasure/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var unit = await _db.UnitsOfMeasure.FindAsync(id);
            if (unit == null) return NotFound();

            unit.IsDeleted = true;
            _db.UnitsOfMeasure.Update(unit);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
