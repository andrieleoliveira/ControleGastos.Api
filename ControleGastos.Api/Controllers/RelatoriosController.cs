using ControleGastos.Api.Dtos.Relatorios;
using ControleGastos.Api.Infrastructure.Data;
using ControleGastos.Api.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Api.Controllers;

[ApiController]
[Route("api/relatorios")]
public class RelatoriosController : ControllerBase
{
    private readonly AppDbContext _db;

    public RelatoriosController(AppDbContext db) => _db = db;

    [HttpGet("totais-por-categoria")]
    public async Task<ActionResult<TotaisPorCategoriaResponseDto>> TotaisPorCategoria()
    {
        var categorias = await _db.Categorias
            .AsNoTracking()
            .Select(c => new { c.Id, c.Descricao })
            .ToListAsync();

        var transacoes = await _db.Transacoes
            .AsNoTracking()
            .Select(t => new { t.CategoriaId, t.Tipo, t.Valor })
            .ToListAsync();

        var itens = categorias
            .Select(cat =>
            {
                var transCat = transacoes.Where(t => t.CategoriaId == cat.Id);

                var totalReceitas = transCat
                    .Where(t => t.Tipo == TipoTransacao.Receita)
                    .Sum(t => t.Valor);

                var totalDespesas = transCat
                    .Where(t => t.Tipo == TipoTransacao.Despesa)
                    .Sum(t => t.Valor);

                return new TotaisPorCategoriaItemDto
                {
                    CategoriaId = cat.Id,
                    CategoriaDescricao = cat.Descricao,
                    TotalReceitas = totalReceitas,
                    TotalDespesas = totalDespesas
                };
            })
            .OrderBy(i => i.CategoriaDescricao)
            .ToList();

        var totalGeral = new TotaisGeraisDto
        {
            TotalReceitas = itens.Sum(i => i.TotalReceitas),
            TotalDespesas = itens.Sum(i => i.TotalDespesas)
        };

        return Ok(new TotaisPorCategoriaResponseDto
        {
            Itens = itens,
            TotalGeral = totalGeral
        });
    }

    [HttpGet("totais-por-pessoa")]
    public async Task<ActionResult<object>> TotaisPorPessoa()
    {
        var pessoas = await _db.Pessoas
            .AsNoTracking()
            .Select(p => new { p.Id, p.Nome })
            .ToListAsync();

        var transacoes = await _db.Transacoes
            .AsNoTracking()
            .Select(t => new { t.PessoaId, t.Tipo, t.Valor })
            .ToListAsync();

        var itens = pessoas
            .Select(p =>
            {
                var transPessoa = transacoes.Where(t => t.PessoaId == p.Id);

                var totalReceitas = transPessoa
                    .Where(t => t.Tipo == TipoTransacao.Receita)
                    .Sum(t => t.Valor);

                var totalDespesas = transPessoa
                    .Where(t => t.Tipo == TipoTransacao.Despesa)
                    .Sum(t => t.Valor);

                return new
                {
                    pessoaId = p.Id,
                    pessoaNome = p.Nome,
                    totalReceitas,
                    totalDespesas,
                    saldo = totalReceitas - totalDespesas
                };
            })
            .OrderBy(i => i.pessoaNome)
            .ToList();

        var totalGeral = new
        {
            totalReceitas = itens.Sum(i => i.totalReceitas),
            totalDespesas = itens.Sum(i => i.totalDespesas),
            saldo = itens.Sum(i => i.saldo)
        };

        return Ok(new { itens, totalGeral });
    }
}
