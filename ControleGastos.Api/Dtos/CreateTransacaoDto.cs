using ControleGastos.Api.Domain.Enums;

namespace ControleGastos.Api.Dtos;

public class CreateTransacaoDto
{
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public TipoTransacao Tipo { get; set; }
    public int CategoriaId { get; set; }
    public int PessoaId { get; set; }
}