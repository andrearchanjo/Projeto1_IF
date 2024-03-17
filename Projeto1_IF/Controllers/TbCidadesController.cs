using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projeto1_IF.Models;

namespace Projeto1_IF.Controllers
{
    public class TbCidadesController : Controller
    {
        private readonly db_IFContext _context;

        public TbCidadesController(db_IFContext context)
        {
            _context = context;
        }

        // GET: TbCidades
        public async Task<IActionResult> Index()
        {
            var db_IFContext = _context.TbCidade.Include(t => t.IdEstadoNavigation);
            return View(await db_IFContext.ToListAsync());
        }

        // GET: TbCidades/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbCidade = await _context.TbCidade
                .Include(t => t.IdEstadoNavigation)
                .FirstOrDefaultAsync(m => m.IdCidade == id);
            if (tbCidade == null)
            {
                return NotFound();
            }

            return View(tbCidade);
        }

        // GET: TbCidades/Create
        public IActionResult Create()
        {
            ViewData["IdEstado"] = new SelectList(_context.TbEstado, "IdEstado", "IdEstado");
            return View();
        }

        // POST: TbCidades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCidade,IdEstado,Nome")] TbCidade tbCidade)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbCidade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEstado"] = new SelectList(_context.TbEstado, "IdEstado", "IdEstado", tbCidade.IdEstado);
            return View(tbCidade);
        }

        // GET: TbCidades/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbCidade = await _context.TbCidade.FindAsync(id);
            if (tbCidade == null)
            {
                return NotFound();
            }
            ViewData["IdEstado"] = new SelectList(_context.TbEstado, "IdEstado", "IdEstado", tbCidade.IdEstado);
            return View(tbCidade);
        }

        // POST: TbCidades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCidade,IdEstado,Nome")] TbCidade tbCidade)
        {
            if (id != tbCidade.IdCidade)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbCidade);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbCidadeExists(tbCidade.IdCidade))
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
            ViewData["IdEstado"] = new SelectList(_context.TbEstado, "IdEstado", "IdEstado", tbCidade.IdEstado);
            return View(tbCidade);
        }

        // GET: TbCidades/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbCidade = await _context.TbCidade
                .Include(t => t.IdEstadoNavigation)
                .FirstOrDefaultAsync(m => m.IdCidade == id);
            if (tbCidade == null)
            {
                return NotFound();
            }

            return View(tbCidade);
        }

        // POST: TbCidades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbCidade = await _context.TbCidade.FindAsync(id);
            if (tbCidade != null)
            {
                _context.TbCidade.Remove(tbCidade);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbCidadeExists(int id)
        {
            return _context.TbCidade.Any(e => e.IdCidade == id);
        }
    }
}
