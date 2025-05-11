using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MarketplaceDomain.Model;
using MarketplaceInfrastructure;

namespace MarketplaceInfrastructure.Controllers
{
    public class StatusessesController : Controller
    {
        private readonly MarketplaceContext _context;

        public StatusessesController(MarketplaceContext context)
        {
            _context = context;
        }

        // GET: Statusesses
        public async Task<IActionResult> Index()
        {
            return View(await _context.Statusesses.ToListAsync());
        }

        // GET: Statusesses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statusess = await _context.Statusesses
                .FirstOrDefaultAsync(m => m.StatusId == id);
            if (statusess == null)
            {
                return NotFound();
            }

            return View(statusess);
        }

        // GET: Statusesses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Statusesses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StatusId,Name,Description")] Statusess statusess)
        {
            if (ModelState.IsValid)
            {
                _context.Add(statusess);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(statusess);
        }

        // GET: Statusesses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statusess = await _context.Statusesses.FindAsync(id);
            if (statusess == null)
            {
                return NotFound();
            }
            return View(statusess);
        }

        // POST: Statusesses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StatusId,Name,Description")] Statusess statusess)
        {
            if (id != statusess.StatusId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(statusess);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatusessExists(statusess.StatusId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(statusess);
        }

        // GET: Statusesses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statusess = await _context.Statusesses
                .FirstOrDefaultAsync(m => m.StatusId == id);
            if (statusess == null)
            {
                return NotFound();
            }

            return View(statusess);
        }

        // POST: Statusesses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var statusess = await _context.Statusesses.FindAsync(id);
            if (statusess != null)
            {
                _context.Statusesses.Remove(statusess);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StatusessExists(int id)
        {
            return _context.Statusesses.Any(e => e.StatusId == id);
        }
    }
}
