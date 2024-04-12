using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
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
    public class TbProfissionaisController : Controller
    {
        private readonly db_IFContext _context;
        private readonly ApplicationDbContext _identity;
        private readonly UserManager<IdentityUser> _userManager;

        public TbProfissionaisController(
            db_IFContext context, 
            UserManager<IdentityUser> userManager,
            ApplicationDbContext identity
        )
        {
            _context = context;
            _userManager = userManager;
            _identity = identity;
        }

        public enum Plano
        {
            MedicoTotal = 1,
            MedicoParcial = 2,
            Nutricionista = 3,
        }

        [Authorize(Roles = "Medico, Nutricionista, GerenteGeral, GerenteMedico, GerenteNutricionista")]
        public async Task<IActionResult> Index()
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return View();
            }

            IQueryable<TbProfissional> db_IFContext = _context.TbProfissional.AsQueryable()
                .AsNoTracking();

            db_IFContext = await AplicaFiltroProfissional(db_IFContext, usuario);

            var profissionais = await db_IFContext
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
                })
                .ToListAsync();

            return View(profissionais);
        }

        [Authorize(Roles = "Medico, Nutricionista, GerenteGeral, GerenteMedico, GerenteNutricionista")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return NotFound();
            }

            IQueryable<TbProfissional> tbProfissionalQuery = _context.TbProfissional.AsQueryable()
                .Include(t => t.IdCidadeNavigation)
                .Include(t => t.IdContratoNavigation)
                .Include(t => t.IdTipoAcessoNavigation)
                .AsNoTracking();

            tbProfissionalQuery = await AplicaFiltroProfissional(tbProfissionalQuery, usuario);

            var tbProfissional = await tbProfissionalQuery.FirstOrDefaultAsync(m => m.IdProfissional == id);

            if (tbProfissional == null)
            {
                return NotFound();
            }

            return View(tbProfissional);
        }

        //public IActionResult Create()
        //{
        //    ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome");
        //    ViewData["IdPlano"] = new SelectList(_context.TbPlano, "IdPlano", "Nome");
        //    ViewData["IdTipoAcesso"] = new SelectList(_context.TbTipoAcesso, "IdTipoAcesso", "Nome");
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("IdTipoProfissional,IdTipoAcesso,IdCidade,IdUser,Nome,Cpf,CrmCrn,Especialidade,Logradouro,Numero,Bairro,Cep,Ddd1,Ddd2,Telefone1,Telefone2,Salario")] TbProfissional tbProfissional, [Bind("IdPlano")] TbContrato IdContratoNavigation)
        //{
        //    try
        //    {
        //        ModelState.Remove("IdUser");
        //        ModelState.Remove("IdContrato");
        //        if (ModelState.IsValid)
        //        {
        //            IdContratoNavigation.DataInicio = DateTime.UtcNow;
        //            IdContratoNavigation.DataFim = IdContratoNavigation.DataInicio.Value.AddMonths(1);
        //            _context.Add(IdContratoNavigation);
        //            await _context.SaveChangesAsync();

        //            var userManager = HttpContext.RequestServices.GetService<UserManager<IdentityUser>>();
        //            if (userManager != null)
        //            {
        //                var email = User.Identity?.Name;
        //                if (email != null)
        //                {
        //                    var user = await userManager.FindByEmailAsync(email);
        //                    if (user != null)
        //                    {
        //                        tbProfissional.IdUser = user.Id;
        //                    }
        //                    else
        //                    {
        //                        return NotFound();
        //                    }
        //                }
        //                else
        //                {
        //                    return NotFound();
        //                }
        //            }
        //            else
        //            {
        //                return NotFound();
        //            }

        //            tbProfissional.IdContrato = IdContratoNavigation.IdContrato;
        //            _context.Add(tbProfissional);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //    } catch (DbUpdateException dex)
        //    {
        //        ModelState.AddModelError("", "Incapaz de salvar. " + dex.ToString());
        //    } catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", "Erro geral. " + ex.ToString());
        //    }
            
        //    ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome", tbProfissional.IdCidade);
        //    ViewData["IdPlano"] = new SelectList(_context.TbPlano, "IdPlano", "Nome", IdContratoNavigation.IdPlano);
        //    ViewData["IdTipoAcesso"] = new SelectList(_context.TbTipoAcesso, "IdTipoAcesso", "Nome", tbProfissional.IdTipoAcesso);
        //    return View(tbProfissional);
        //}

        [Authorize(Roles = "Medico, Nutricionista, GerenteGeral, GerenteMedico, GerenteNutricionista")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Erro", "Home");
            }

            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return NotFound();
            }

            IQueryable<TbProfissional> tbProfissionalQuery = _context.TbProfissional.AsQueryable()
                .Include(t => t.IdCidadeNavigation)
                .Include(t => t.IdContratoNavigation)
                .Include(t => t.IdTipoAcessoNavigation);

            tbProfissionalQuery = await AplicaFiltroProfissional(tbProfissionalQuery, usuario);

            var tbProfissional = await tbProfissionalQuery
                .Include(t => t.IdContratoNavigation)
                .FirstOrDefaultAsync(m => m.IdProfissional == id);

            if (tbProfissional == null)
            {
                return NotFound();
            }

            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome", tbProfissional.IdCidade);
            ViewData["IdContrato"] = new SelectList(_context.TbPlano, "IdPlano", "Nome", tbProfissional.IdContratoNavigation.IdPlano);
            ViewData["IdTipoAcesso"] = new SelectList(_context.TbTipoAcesso, "IdTipoAcesso", "Nome", tbProfissional.IdTipoAcesso);
            return View(tbProfissional);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Medico, Nutricionista, GerenteGeral, GerenteMedico, GerenteNutricionista")]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return NotFound();
            }

            IQueryable<TbProfissional> tbProfissionalQuery = _context.TbProfissional.AsQueryable()
                .Include(t => t.IdCidadeNavigation)
                .Include(t => t.IdContratoNavigation)
                .Include(t => t.IdTipoAcessoNavigation);

            tbProfissionalQuery = await AplicaFiltroProfissional(tbProfissionalQuery, usuario);

            var tbProfissional = await tbProfissionalQuery
                .Include(t => t.IdContratoNavigation)
                .FirstOrDefaultAsync(m => m.IdProfissional == id);

            if (tbProfissional == null)
            {
                return NotFound();
            }

            var cpf = tbProfissional.Cpf;

            var isGerente = User.IsInRole("GerenteGeral") || User.IsInRole("GerenteMedico") || User.IsInRole("GerenteNutricionista");

            if (await TryUpdateModelAsync<TbProfissional>(
                tbProfissional,
                "",
                s => s.IdProfissional,
                s => s.IdTipoAcesso,
                s => s.IdCidade,
                s => s.Nome,
                s => s.Cpf,
                s => s.CrmCrn,
                s => s.Especialidade,
                s => s.Logradouro,
                s => s.Numero,
                s => s.Bairro,
                s => s.Cep,
                s => s.Ddd1,
                s => s.Ddd2,
                s => s.Telefone1,
                s => s.Telefone2,
                s => s.Salario))
            {
                try
                {
                    if (!isGerente)
                    {
                        tbProfissional.Cpf = cpf;
                    }

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
        [Authorize(Roles = "GerenteGeral, GerenteMedico, GerenteNutricionista")]
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return NotFound();
            }

            IQueryable<TbProfissional> tbProfissionalQuery = _context.TbProfissional.AsQueryable();

            tbProfissionalQuery = await AplicaFiltroProfissional(tbProfissionalQuery, usuario);

            var tbProfissional = await tbProfissionalQuery
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "GerenteGeral, GerenteMedico, GerenteNutricionista")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return NotFound();
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    IQueryable<TbProfissional> tbProfissionalQuery = _context.TbProfissional
                        .AsQueryable();
                    tbProfissionalQuery = await AplicaFiltroProfissional(tbProfissionalQuery, usuario);

                    var tbProfissional = await tbProfissionalQuery
                        .FirstOrDefaultAsync(m => m.IdProfissional == id);

                    if (tbProfissional == null)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    var pacientesDoProfissional = await _context.TbMedicoPaciente
                        .Where(mp => mp.IdProfissional == id)
                        .Select(mp => mp.IdPaciente)
                        .ToListAsync();

                    foreach (var pacienteId in pacientesDoProfissional)
                    {
                        var tbMedicoPaciente = await _context.TbMedicoPaciente
                                .FirstOrDefaultAsync(mp => mp.IdPaciente == pacienteId && mp.IdProfissional == id);

                        var numProfissionaisVinculados = await _context.TbMedicoPaciente
                            .CountAsync(mp => mp.IdPaciente == pacienteId);

                        if (tbMedicoPaciente != null)
                        {
                            _context.TbMedicoPaciente.Remove(tbMedicoPaciente);
                        }

                        if (numProfissionaisVinculados == 1)
                        {
                            var paciente = await _context.TbPaciente.FindAsync(pacienteId);

                            if (paciente != null)
                            {
                                _context.TbPaciente.Remove(paciente);
                            }
                        }
                    }

                    _context.TbProfissional.Remove(tbProfissional);
                    var userId = tbProfissional.IdUser;

                    var user = await _identity.Users
                        .FirstOrDefaultAsync(x => x.Id == userId);

                    if (user != null)
                    {
                        _identity.Users.Remove(user);
                    }

                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
                }
            }
        }

        private bool TbProfissionalExists(int id)
        {
            return _context.TbProfissional.Any(e => e.IdProfissional == id);
        }

        private async Task<IQueryable<TbProfissional>> AplicaFiltroProfissional(IQueryable<TbProfissional> query, IdentityUser usuario)
        {
            var roles = await _userManager.GetRolesAsync(usuario);

            if (roles.Contains("GerenteGeral"))
            {
            }
            else if (roles.Contains("GerenteMedico"))
            {
                var medicoIds = await GetUsersIdsByRoleNameAsync("Medico");

                if (medicoIds != null)
                {
                    query = query
                        .Where(s => medicoIds.Contains(s.IdUser) || s.IdUser == usuario.Id);
                }
                else
                {
                    throw new InvalidOperationException("Erro ao obter IDs de médicos.");
                }
            }
            else if (roles.Contains("GerenteNutricionista"))
            {
                var nutricionistaIds = await GetUsersIdsByRoleNameAsync("Nutricionista");

                if (nutricionistaIds != null)
                {
                    query = query
                        .Where(s => nutricionistaIds.Contains(s.IdUser) || s.IdUser == usuario.Id);
                }
                else
                {
                    throw new InvalidOperationException("Erro ao obter IDs de nutricionistas.");
                }
            }
            else if (roles.Contains("Medico") || roles.Contains("Nutricionista"))
            {
                query = query
                    .Where(s => s.IdUser == usuario.Id);
            }
            else
            {
                throw new InvalidOperationException("Usuário sem permissão para visualizar profissionais.");
            }

            return query;
        }

        private async Task<List<string>?> GetUsersIdsByRoleNameAsync(string roleName)
        {
            var role = await _identity.Roles.SingleOrDefaultAsync(r => r.Name == roleName);

            if (role != null)
            {
                return await _identity.UserRoles
                    .Where(ur => ur.RoleId == role.Id)
                    .Select(ur => ur.UserId)
                    .ToListAsync();
            }
 
            return null;
        }
    }
}
