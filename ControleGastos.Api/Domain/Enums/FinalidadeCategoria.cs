namespace ControleGastos.Domain.Enums
{
    /// <summary>
    /// Essa enumeração foi criada para representar, de forma clara e tipada,
    /// os tipos de transações que uma categoria pode aceitar (despesa, receita ou ambas),
    /// conforme especificado na regra de negócio.
    ///
    /// O uso de enum evita valores inválidos, facilita a validação das regras de negócio
    /// no cadastro de transações e mantém o domínio mais organizado e consistente.
    /// </summary>
    public enum FinalidadeCategoria
    {
        Despesa = 1,
        Receita = 2,
        Ambos = 3
    }
}