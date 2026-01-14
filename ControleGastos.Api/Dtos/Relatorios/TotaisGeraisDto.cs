namespace ControleGastos.Api.Dtos.Relatorios;

public class TotaisGeraisDto
{
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal Saldo => TotalReceitas - TotalDespesas;
}
