using ControleGastos.Api.Domain.Entities;
using ControleGastos.Api.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Api.Controllers;
/// <summary>
/// Controller responsável pelo gerenciamento de pessoas.
///
/// Optei por utilizar o EF Core diretamente via AppDbContext
/// para manter o código simples e direto, adequado ao escopo.
///
/// Regras e decisões aplicadas:
/// - A listagem utiliza AsNoTracking(), pois é uma operação somente de leitura,
///   evitando overhead de tracking desnecessário.
/// - No cadastro (POST), são feitas validações básicas de domínio
///   (nome obrigatório e idade positiva).
/// - O identificador da pessoa é gerado automaticamente pelo banco de dados.
/// - No DELETE, a exclusão da pessoa remove automaticamente todas as
///   transações associadas, utilizando deleção em cascata configurada no EF Core.
/// - Os retornos seguem o padrão REST (Created, NoContent, NotFound).
/// </summary>

[ApiController]
[Route("api/pessoas")]
public class PessoasController : ControllerBase
{
    private readonly AppDbContext _db;

    public PessoasController(AppDbContext db) => _db = db;

    /// <summary>
    /// Lista todas as pessoas cadastradas.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<Pessoa>>> Listar()
    {
        var pessoas = await _db.Pessoas
            .AsNoTracking()
            .ToListAsync();

        return Ok(pessoas);
    }

    /// <summary>
    /// Cria uma pessoa.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Pessoa>> Criar([FromBody] Pessoa pessoa)
    {
        if (string.IsNullOrWhiteSpace(pessoa.Nome))
            return BadRequest("Nome é obrigatório.");

        if (pessoa.Idade <= 0)
            return BadRequest("Idade deve ser um número inteiro positivo.");

        _db.Pessoas.Add(pessoa);
        await _db.SaveChangesAsync();

        return Created($"/api/pessoas/{pessoa.Id}", pessoa);
    }

    /// <summary>
    /// Remove uma pessoa pelo id. As transações associadas são removidas em cascata.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deletar(int id)
    {
        var pessoa = await _db.Pessoas
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pessoa is null)
            return NotFound();

        _db.Pessoas.Remove(pessoa);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
