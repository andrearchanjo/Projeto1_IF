using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projeto1_IF.Data;
using Projeto1_IF.Models;

namespace Projeto1_IF.Controllers
{
    [Authorize]
    public class TbPacientesController : Controller
    {
        private readonly db_IFContext _context;
        private readonly ApplicationDbContext _identity;
        private readonly UserManager<IdentityUser> _userManager;

        public TbPacientesController(
            db_IFContext context,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext identity
        )
        {
            _context = context;
            _userManager = userManager;
            _identity = identity;
        }

        [Authorize(Roles = "Medico, Nutricionista")]
        public async Task<IActionResult> Index()
        {
            var idProfissional = await GetProfissionalIdByUserIdAsync();

            if (idProfissional == null)
            {
                throw new InvalidOperationException("Não foi possível obter o ID do profissional associado ao usuário atual.");
            }

            var db_IFContext = _context.TbPaciente.AsQueryable()
                .AsNoTracking()
                .Where(p => p.TbMedicoPaciente.Any(mp => mp.IdProfissional == idProfissional));

            return View(await db_IFContext.ToListAsync());
        }

        [Authorize(Roles = "Medico, Nutricionista")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var idProfissional = await GetProfissionalIdByUserIdAsync();

            if (idProfissional == null)
            {
                throw new InvalidOperationException("Não foi possível obter o ID do profissional associado ao usuário atual.");
            }

            var tbPaciente = await _context.TbPaciente
                .Include(t => t.IdCidadeNavigation)
                .ThenInclude(s => s.IdEstadoNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdPaciente == id && m.TbMedicoPaciente.Any(mp => mp.IdProfissional == idProfissional));

            if (tbPaciente == null)
            {
                return NotFound();
            }

            return View(tbPaciente);
        }

        [Authorize(Roles = "Medico, Nutricionista")]
        public IActionResult Create()
        {
            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Medico, Nutricionista")]
        public async Task<IActionResult> Create([Bind("IdPaciente,Nome,Rg,Cpf,DataNascimento,NomeResponsavel,Sexo,Etnia,Endereco,Bairro,IdCidade,TelResidencial,TelComercial,TelCelular,Profissao,FlgAtleta,FlgGestante")] TbPaciente tbPaciente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(tbPaciente);
                    await _context.SaveChangesAsync();

                    var idProfissional = await GetProfissionalIdByUserIdAsync();

                    if (idProfissional != null)
                    {
                        TbMedicoPaciente medicoPaciente = new TbMedicoPaciente
                        {
                            IdProfissional = (int) idProfissional,
                            IdPaciente = tbPaciente.IdPaciente
                        };

                        _context.Add(medicoPaciente);
                    }
                    else
                    {
                        throw new InvalidOperationException("Não foi possível obter o ID do profissional associado ao usuário atual.");
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException dex)
            {
                ModelState.AddModelError("", "Incapaz de salvar. " + dex.ToString());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro geral. " + ex.ToString());
            }

            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome", tbPaciente.IdCidade);
            return View(tbPaciente);
        }

        [Authorize(Roles = "Medico, Nutricionista")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Erro", "Home");
            }

            var idProfissional = await GetProfissionalIdByUserIdAsync();

            if (idProfissional == null)
            {
                throw new InvalidOperationException("Não foi possível obter o ID do profissional associado ao usuário atual.");
            }

            var tbPaciente = await _context.TbPaciente
                .Include(t => t.IdCidadeNavigation)
                .ThenInclude(s => s.IdEstadoNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdPaciente == id && m.TbMedicoPaciente.Any(mp => mp.IdProfissional == idProfissional));

            if (tbPaciente == null)
            {
                return NotFound();
            }

            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome", tbPaciente.IdCidade);
            return View(tbPaciente);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Medico, Nutricionista")]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Erro", "Home");
            }

            var idProfissional = await GetProfissionalIdByUserIdAsync();

            if (idProfissional == null)
            {
                throw new InvalidOperationException("Não foi possível obter o ID do profissional associado ao usuário atual.");
            }

            var tbPaciente = await _context.TbPaciente
                .Include(t => t.IdCidadeNavigation)
                .ThenInclude(s => s.IdEstadoNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdPaciente == id && m.TbMedicoPaciente.Any(mp => mp.IdProfissional == idProfissional));

            if (tbPaciente == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<TbPaciente>(
                tbPaciente,
                "",
                s => s.IdPaciente,
                s => s.Nome,
                s => s.Rg,
                s => s.Cpf,
                s => s.DataNascimento,
                s => s.NomeResponsavel,
                s => s.Sexo,
                s => s.Etnia,
                s => s.Endereco,
                s => s.Bairro,
                s => s.IdCidade,
                s => s.TelResidencial,
                s => s.TelComercial,
                s => s.TelCelular,
                s => s.Profissao,
                s => s.FlgAtleta,
                s => s.FlgGestante))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }

            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome", tbPaciente.IdCidade);

            return View(tbPaciente);
        }

        [Authorize(Roles = "Medico, Nutricionista")]
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var idProfissional = await GetProfissionalIdByUserIdAsync();

            if (idProfissional == null)
            {
                throw new InvalidOperationException("Não foi possível obter o ID do profissional associado ao usuário atual.");
            }

            var tbPaciente = await _context.TbPaciente
                .Include(t => t.IdCidadeNavigation)
                .ThenInclude(s => s.IdEstadoNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdPaciente == id && m.TbMedicoPaciente.Any(mp => mp.IdProfissional == idProfissional));

            if (tbPaciente == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(tbPaciente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Medico, Nutricionista")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var idProfissional = await GetProfissionalIdByUserIdAsync();

            if (idProfissional == null)
            {
                throw new InvalidOperationException("Não foi possível obter o ID do profissional associado ao usuário atual.");
            }

            var tbPaciente = await _context.TbPaciente
                .Include(t => t.IdCidadeNavigation)
                .ThenInclude(s => s.IdEstadoNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdPaciente == id && m.TbMedicoPaciente.Any(mp => mp.IdProfissional == idProfissional));

            if (tbPaciente == null)
            {
                return NotFound();
            }

            try
            {
                var tbMedicoPaciente = await _context.TbMedicoPaciente
                    .FirstOrDefaultAsync(m => m.IdPaciente == id && m.IdProfissional == idProfissional);

                if (tbMedicoPaciente == null) 
                { 
                    return NotFound(); 
                }

                _context.TbMedicoPaciente.Remove(tbMedicoPaciente);
                await _context.SaveChangesAsync();

                _context.TbPaciente.Remove(tbPaciente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }


        private bool TbPacienteExists(int id)
        {
            return _context.TbPaciente.Any(e => e.IdPaciente == id);
        }

        private async Task<int?> GetProfissionalIdByUserIdAsync()
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return null;
            }

            var profissionalId = await _context.TbProfissional
                .Where(p => p.IdUser == usuario.Id)
                .Select(p => p.IdProfissional)
                .FirstOrDefaultAsync();

            return profissionalId;
        }
    }
}
