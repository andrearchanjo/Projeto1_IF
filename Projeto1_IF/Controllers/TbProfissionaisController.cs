using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projeto1_IF.Models;

namespace Projeto1_IF.Controllers
{
    public class TbProfissionaisController : Controller
    {
        private readonly db_IFContext _context;

        public TbProfissionaisController(db_IFContext context)
        {
            _context = context;
        }

        // GET: TbProfissionais
        public async Task<IActionResult> Index()
        {
            var db_IFContext = _context.TbProfissional.Include(t => t.IdCidadeNavigation).Include(t => t.IdContratoNavigation).Include(t => t.IdTipoAcessoNavigation);
            return View(await db_IFContext.ToListAsync());
        }

        // GET: TbProfissionais/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbProfissional = await _context.TbProfissional
                .Include(t => t.IdCidadeNavigation)
                .Include(t => t.IdContratoNavigation)
                .Include(t => t.IdTipoAcessoNavigation)
                .FirstOrDefaultAsync(m => m.IdProfissional == id);
            if (tbProfissional == null)
            {
                return NotFound();
            }

            return View(tbProfissional);
        }

        [Authorize]
        // GET: TbProfissionais/Create
        public IActionResult Create()
        {
            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome");
            ViewData["IdPlano"] = new SelectList(_context.TbPlano, "IdPlano", "Nome");
            ViewData["IdTipoAcesso"] = new SelectList(_context.TbTipoAcesso, "IdTipoAcesso", "Nome");
            return View();
        }

        // POST: TbProfissionais/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoProfissional,IdTipoAcesso,IdCidade,IdUser,Nome,Cpf,CrmCrn,Especialidade,Logradouro,Numero,Bairro,Cep,Cidade,Estado,Ddd1,Ddd2,Telefone1,Telefone2,Salario")] TbProfissional tbProfissional, [Bind("IdPlano")] TbContrato IdContratoNavigation)
        {
            ModelState.Remove("IdUser");
            if (ModelState.IsValid)
            {
                IdContratoNavigation.DataInicio = DateTime.UtcNow;
                IdContratoNavigation.DataFim = IdContratoNavigation.DataInicio.Value.AddMonths(1);
                _context.Add(IdContratoNavigation);
                await _context.SaveChangesAsync();

                var userManager = HttpContext.RequestServices.GetService<UserManager<IdentityUser>>();
                if (userManager != null)
                {
                    var email = User.Identity?.Name;
                    if (email != null)
                    {
                        var user = await userManager.FindByEmailAsync(email);
                        if (user != null)
                        {
                            tbProfissional.IdUser = user.Id;
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return NotFound();
                }

                tbProfissional.IdContrato = IdContratoNavigation.IdContrato;
                _context.Add(tbProfissional);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome");
            ViewData["IdPlano"] = new SelectList(_context.TbPlano, "IdPlano", "Nome");
            ViewData["IdTipoAcesso"] = new SelectList(_context.TbTipoAcesso, "IdTipoAcesso", "Nome");
            return View(tbProfissional);
        }

        // GET: TbProfissionais/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbProfissional = await _context.TbProfissional.FindAsync(id);
            if (tbProfissional == null)
            {
                return NotFound();
            }
            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "IdCidade", tbProfissional.IdCidade);
            ViewData["IdContrato"] = new SelectList(_context.TbContrato, "IdContrato", "IdContrato", tbProfissional.IdContrato);
            ViewData["IdTipoAcesso"] = new SelectList(_context.TbTipoAcesso, "IdTipoAcesso", "Nome", tbProfissional.IdTipoAcesso);
            return View(tbProfissional);
        }

        // POST: TbProfissionais/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdProfissional,IdTipoProfissional,IdContrato,IdTipoAcesso,IdCidade,IdUser,Nome,Cpf,CrmCrn,Especialidade,Logradouro,Numero,Bairro,Cep,Cidade,Estado,Ddd1,Ddd2,Telefone1,Telefone2,Salario")] TbProfissional tbProfissional)
        {
            if (id != tbProfissional.IdProfissional)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbProfissional);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbProfissionalExists(tbProfissional.IdProfissional))
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
            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "IdCidade", tbProfissional.IdCidade);
            ViewData["IdContrato"] = new SelectList(_context.TbContrato, "IdContrato", "IdContrato", tbProfissional.IdContrato);
            ViewData["IdTipoAcesso"] = new SelectList(_context.TbTipoAcesso, "IdTipoAcesso", "Nome", tbProfissional.IdTipoAcesso);
            return View(tbProfissional);
        }

        // GET: TbProfissionais/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbProfissional = await _context.TbProfissional
                .Include(t => t.IdCidadeNavigation)
                .Include(t => t.IdContratoNavigation)
                .Include(t => t.IdTipoAcessoNavigation)
                .FirstOrDefaultAsync(m => m.IdProfissional == id);
            if (tbProfissional == null)
            {
                return NotFound();
            }

            return View(tbProfissional);
        }

        // POST: TbProfissionais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbProfissional = await _context.TbProfissional.FindAsync(id);
            if (tbProfissional != null)
            {
                _context.TbProfissional.Remove(tbProfissional);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbProfissionalExists(int id)
        {
            return _context.TbProfissional.Any(e => e.IdProfissional == id);
        }
    }
}
