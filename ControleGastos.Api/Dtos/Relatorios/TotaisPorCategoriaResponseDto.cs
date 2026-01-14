namespace ControleGastos.Api.Dtos.Relatorios;

public class TotaisPorCategoriaResponseDto
{
    public List<TotaisPorCategoriaItemDto> Itens { get; set; } = new();
    public TotaisGeraisDto TotalGeral { get; set; } = new();
}
