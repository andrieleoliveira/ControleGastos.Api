namespace ControleGastos.Api.Dtos.Relatorios;

public class TotaisPorCategoriaItemDto
{
    public int CategoriaId { get; set; }
    public string CategoriaDescricao { get; set; } = string.Empty;

    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal Saldo => TotalReceitas - TotalDespesas;
}
