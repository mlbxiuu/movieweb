using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieWebsite.Data;
using MovieWebsite.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MovieWebsite.Areas.Admin.Controllers
{
    // [Area("Admin")]
    public class CountriesController : Controller
    {
        private readonly AppDbContext _context;

        public CountriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Countries
        public async Task<IActionResult> Index()
        {
            var countries = await _context.Countries.OrderBy(c => c.Name).ToListAsync();
            return View(countries);
        }

        // GET: Admin/Countries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Countries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Code,FlagPath")] Country country)
        {
            if (ModelState.IsValid)
            {
                _context.Add(country);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã thêm quốc gia thành công!";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Thêm quốc gia thất bại, vui lòng kiểm tra lại.";
            return View(country);
        }

        // GET: Admin/Countries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var country = await _context.Countries.FindAsync(id);
            if (country == null) return NotFound();
            return View(country);
        }

        // POST: Admin/Countries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Code,FlagPath")] Country country)
        {
            if (id != country.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Đã cập nhật quốc gia thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Countries.Any(e => e.Id == country.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Cập nhật thất bại, vui lòng kiểm tra lại.";
            return View(country);
        }

        // GET: Admin/Countries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var country = await _context.Countries.FirstOrDefaultAsync(m => m.Id == id);
            if (country == null) return NotFound();
            return View(country);
        }

        // POST: Admin/Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country != null)
            {
                _context.Countries.Remove(country);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa quốc gia thành công!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}