using ControleGastos.Domain.Enums;

namespace ControleGastos.Domain.Entities
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        /// <summary>
        /// Define a finalidade da categoria (despesa, receita ou ambas),
        /// sendo utilizada posteriormente para validar o tipo de transação
        /// associada à categoria.
        /// </summary>
        public FinalidadeCategoria Finalidade { get; set; }
    }
}
