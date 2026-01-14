using ControleGastos.Api.Domain.Entities;
using ControleGastos.Api.Domain.Enums;
using ControleGastos.Api.Dtos;
using ControleGastos.Api.Infrastructure.Data;
using ControleGastos.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Api.Controllers;

[ApiController]
[Route("api/transacoes")]
public class TransacoesController : ControllerBase
{
    private readonly AppDbContext _db;

    public TransacoesController(AppDbContext db) => _db = db;

    /// <summary>
    /// Lista todas as transações cadastradas.
    /// 
    /// Importante: o retorno é "achatado" (projection) para evitar ciclos de referência
    /// ao serializar entidades com navegações (Pessoa -> Transacoes -> Pessoa...).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var transacoes = await _db.Transacoes
            .AsNoTracking()
            .Include(t => t.Categoria)
            .Include(t => t.Pessoa)
            .Select(t => new
            {
                t.Id,
                t.Descricao,
                t.Valor,
                t.Tipo,

                t.CategoriaId,
                CategoriaDescricao = t.Categoria.Descricao,
                CategoriaFinalidade = t.Categoria.Finalidade,

                t.PessoaId,
                PessoaNome = t.Pessoa.Nome,
                PessoaIdade = t.Pessoa.Idade
            })
            .ToListAsync();

        return Ok(transacoes);
    }

    /// <summary>
    /// Cria uma transação aplicando as regras do teste:
    /// - Valor deve ser positivo
    /// - Pessoa menor de idade (<18) só pode lançar DESPESA
    /// - A categoria deve ser compatível com o tipo (finalidade despesa/receita/ambas)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CreateTransacaoDto dto)
    {
        if (dto is null)
            return BadRequest("Body inválido.");

        if (string.IsNullOrWhiteSpace(dto.Descricao))
            return BadRequest("Descrição é obrigatória.");

        if (dto.Valor <= 0)
            return BadRequest("Valor deve ser um número decimal positivo.");

        var pessoa = await _db.Pessoas.FirstOrDefaultAsync(p => p.Id == dto.PessoaId);
        if (pessoa is null)
            return BadRequest("PessoaId inválido. Pessoa não encontrada.");

        var categoria = await _db.Categorias.FirstOrDefaultAsync(c => c.Id == dto.CategoriaId);
        if (categoria is null)
            return BadRequest("CategoriaId inválido. Categoria não encontrada.");

        // Regra: menor de idade só pode despesa
        if (pessoa.Idade < 18 && dto.Tipo != TipoTransacao.Despesa)
            return BadRequest("Pessoa menor de 18 anos só pode registrar despesas.");

        // Regra: categoria precisa aceitar o tipo escolhido
        var categoriaAceitaTipo =
            categoria.Finalidade == FinalidadeCategoria.Ambos ||
            (categoria.Finalidade == FinalidadeCategoria.Despesa && dto.Tipo == TipoTransacao.Despesa) ||
            (categoria.Finalidade == FinalidadeCategoria.Receita && dto.Tipo == TipoTransacao.Receita);

        if (!categoriaAceitaTipo)
            return BadRequest("A categoria informada não é compatível com o tipo da transação (despesa/receita).");

        var transacao = new Transacao
        {
            Descricao = dto.Descricao.Trim(),
            Valor = dto.Valor,
            Tipo = dto.Tipo,
            PessoaId = dto.PessoaId,
            CategoriaId = dto.CategoriaId
        };

        _db.Transacoes.Add(transacao);
        await _db.SaveChangesAsync();

        // Retorno enxuto evita ciclos e é suficiente para o front.
        var response = new
        {
            transacao.Id,
            transacao.Descricao,
            transacao.Valor,
            transacao.Tipo,
            transacao.CategoriaId,
            transacao.PessoaId
        };

        return Created($"/api/transacoes/{transacao.Id}", response);
    }
}

