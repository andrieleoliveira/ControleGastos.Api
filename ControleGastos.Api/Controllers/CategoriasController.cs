using ControleGastos.Api.Domain.Entities;
using ControleGastos.Api.Domain.Enums;
using ControleGastos.Api.Infrastructure.Data;
using ControleGastos.Domain.Entities;
using ControleGastos.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Api.Controllers;

/// <summary>
/// Controller responsável pelo gerenciamento de categorias.
///
/// Optei por usar o EF Core diretamente via AppDbContext
/// para manter o fluxo simples e objetivo, já que o escopo
/// não exige uma camada de serviço separada.
///
/// Regras aplicadas:
/// - A listagem utiliza AsNoTracking() por ser apenas leitura,
///   evitando tracking desnecessário e melhorando a performance.
/// - No POST, são validados apenas os campos essenciais para
///   garantir consistência dos dados.
/// - O identificador não é recebido do cliente, sendo gerado
///   automaticamente pelo banco de dados.
/// - O retorno Created() segue o padrão REST, informando a
///   localização do recurso criado.
/// </summary>

[ApiController]
[Route("api/categorias")]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _db;

    public CategoriasController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<Categoria>>> Listar()
    {
        var categorias = await _db.Categorias.AsNoTracking().ToListAsync();
        return Ok(categorias);
    }

    [HttpPost]
    public async Task<ActionResult<Categoria>> Criar([FromBody] Categoria categoria)
    {
        if (string.IsNullOrWhiteSpace(categoria.Descricao))
            return BadRequest("Descrição é obrigatória.");

        if (!Enum.IsDefined(typeof(FinalidadeCategoria), categoria.Finalidade))
            return BadRequest("Finalidade inválida. Use 1=Despesa, 2=Receita, 3=Ambas.");

        categoria.Id = 0;

        _db.Categorias.Add(categoria);
        await _db.SaveChangesAsync();

        return Created($"/api/categorias/{categoria.Id}", categoria);
    }
}
