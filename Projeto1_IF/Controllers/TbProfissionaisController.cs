using System;
using System.Collections.Generic;
using System.Data;
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
    [Authorize]
    public class TbProfissionaisController : Controller
    {
        private readonly db_IFContext _context;

        public TbProfissionaisController(db_IFContext context)
        {
            _context = context;
        }

        public enum Plano
        {
            MedicoTotal = 1,
            MedicoParcial = 2,
            Nutricionista = 3,
        }

        // GET: TbProfissionais
        [Authorize(Roles = "GerenteMedico, GerenteNutricionista")]
        //[Authorize(Roles = "Medico, Nutricionista")]
        public async Task<IActionResult> Index()
        {
            // Método 1
            if (User.IsInRole("GerenteMedico")) {
                var db_IFContext = _context.TbProfissional
                    .Where(t => (Plano)t.IdContratoNavigation.IdPlano == Plano.MedicoTotal || (Plano)t.IdContratoNavigation.IdPlano == Plano.MedicoParcial)
                    .Select(pro => new ProfissionalResumido
                    {
                        IdProfissional = pro.IdProfissional,
                        Nome = pro.Nome,
                        NomeCidade = pro.IdCidadeNavigation.Nome,
                        NomePlano = pro.IdContratoNavigation.IdPlanoNavigation.Nome,
                        Cpf = pro.Cpf,
                        Especialidade = pro.Especialidade,
                        Logradouro = pro.Logradouro,
                        Numero = pro.Numero,
                        Bairro = pro.Bairro,
                        Cep = pro.Cep,
                        Ddd1 = pro.Ddd1,
                        Ddd2 = pro.Ddd2,
                        Telefone1 = pro.Telefone1,
                        Telefone2 = pro.Telefone2,
                        Salario = pro.Salario,
                    });

                return View(db_IFContext);


                // Método 2
                //var db_IFContext = (from pro in _context.TbProfissional
                //                    where (Plano)pro.IdContratoNavigation.IdPlano == Plano.MedicoTotal
                //                    select pro)
                //                        .Include(t => t.IdTipoAcessoNavigation)
                //                        .Include(t => t.IdCidadeNavigation)
                //                        .Include(pro => pro.IdContratoNavigation)
                //                            .ThenInclude(contrato => contrato.IdPlanoNavigation);

                // Método 3
                //var db_IFContext = from pro in _context.TbProfissional
                //                   join contrato in _context.TbContrato on pro.IdContrato equals contrato.IdContrato
                //                   join plano in _context.TbPlano on contrato.IdPlano equals plano.IdPlano
                //                   where plano.IdPlano == 1
                //                   select pro;
            }
            else { 
                if (User.IsInRole("GerenteNutricionista")) {
                    var db_IFContext2 = (from pro in _context.TbProfissional
                                         where (Plano)pro.IdContratoNavigation.IdPlano == Plano.Nutricionista
                                         select new ProfissionalResumido
                                    {
                                        IdProfissional = pro.IdProfissional,
                                        Nome = pro.Nome,
                                        NomeCidade = pro.IdCidadeNavigation.Nome,
                                        NomePlano = pro.IdContratoNavigation.IdPlanoNavigation.Nome,
                                        Cpf = pro.Cpf,
                                        Especialidade = pro.Especialidade,
                                        Logradouro = pro.Logradouro,
                                        Numero = pro.Numero,
                                        Bairro = pro.Bairro,
                                        Cep = pro.Cep,
                                        Ddd1 = pro.Ddd1,
                                        Ddd2 = pro.Ddd2,
                                        Telefone1 = pro.Telefone1,
                                        Telefone2 = pro.Telefone2,
                                        Salario = pro.Salario,
                                    });
                        return View(await db_IFContext2.ToListAsync());
                }
            }
            return View();
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
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdProfissional == id);
            if (tbProfissional == null)
            {
                return NotFound();
            }

            return View(tbProfissional);
        }

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
        public async Task<IActionResult> Create([Bind("IdTipoProfissional,IdTipoAcesso,IdCidade,IdUser,Nome,Cpf,CrmCrn,Especialidade,Logradouro,Numero,Bairro,Cep,Ddd1,Ddd2,Telefone1,Telefone2,Salario")] TbProfissional tbProfissional, [Bind("IdPlano")] TbContrato IdContratoNavigation)
        {
            try
            {
                ModelState.Remove("IdUser");
                ModelState.Remove("IdContrato");
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
            } catch (DbUpdateException dex)
            {
                ModelState.AddModelError("", "Incapaz de salvar. " + dex.ToString());
            } catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro geral. " + ex.ToString());
            }
            
            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome", tbProfissional.IdCidade);
            ViewData["IdPlano"] = new SelectList(_context.TbPlano, "IdPlano", "Nome", IdContratoNavigation.IdPlano);
            ViewData["IdTipoAcesso"] = new SelectList(_context.TbTipoAcesso, "IdTipoAcesso", "Nome", tbProfissional.IdTipoAcesso);
            return View(tbProfissional);
        }

        // GET: TbProfissionais/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Erro", "Home");
            }

            var tbProfissional = await _context.TbProfissional.Include(t => t.IdContratoNavigation).FirstOrDefaultAsync(s => s.IdProfissional == id);
            if (tbProfissional == null)
            {
                return NotFound();
            }

            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome", tbProfissional.IdCidade);
            ViewData["IdContrato"] = new SelectList(_context.TbPlano, "IdPlano", "Nome", tbProfissional.IdContratoNavigation.IdPlano);
            ViewData["IdTipoAcesso"] = new SelectList(_context.TbTipoAcesso, "IdTipoAcesso", "Nome", tbProfissional.IdTipoAcesso);
            return View(tbProfissional);
        }

        // POST: TbProfissionais/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbProfissional = await _context.TbProfissional.Include(t => t.IdContratoNavigation).FirstOrDefaultAsync(s => s.IdProfissional == id);
            if ( tbProfissional == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<TbProfissional>(
                tbProfissional,
                "",
                s => s.IdProfissional, s => s.IdTipoAcesso, s => s.IdCidade, s => s.Nome, s => s.Cpf, s => s.CrmCrn,
                s => s.Especialidade, s => s.Logradouro, s => s.Numero, s => s.Bairro, s => s.Cep,
                s => s.Ddd1, s => s.Ddd2, s => s.Telefone1, s => s.Telefone2, s => s.Salario))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }

            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome", tbProfissional.IdCidade);
            ViewData["IdContrato"] = new SelectList(_context.TbPlano, "IdPlano", "Nome", tbProfissional.IdContratoNavigation.IdPlano);
            ViewData["IdTipoAcesso"] = new SelectList(_context.TbTipoAcesso, "IdTipoAcesso", "Nome", tbProfissional.IdTipoAcesso);

            return View(tbProfissional);
        }

        // GET: TbProfissionais/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbProfissional = await _context.TbProfissional
                .Include(t => t.IdCidadeNavigation)
                .ThenInclude(s => s.IdEstadoNavigation)
                .Include(t => t.IdTipoAcessoNavigation)
                .Include(t => t.IdContratoNavigation)
                .ThenInclude(s => s.IdPlanoNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdProfissional == id);

            if (tbProfissional == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(tbProfissional);
        }

        // POST: TbProfissionais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbProfissional = await _context.TbProfissional.FindAsync(id);
            if (tbProfissional == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.TbProfissional.Remove(tbProfissional);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private bool TbProfissionalExists(int id)
        {
            return _context.TbProfissional.Any(e => e.IdProfissional == id);
        }
    }
}
