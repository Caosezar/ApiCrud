namespace ApiCrud.Models
{
    /// <summary>
    /// Entidade Product representa um produto no banco de dados.
    /// 
    /// ℹ️ O QUE É UMA ENTIDADE?
    /// Uma entidade é uma classe que representa uma tabela no banco de dados.
    /// Cada propriedade pública equivale a uma coluna na tabela.
    /// </summary>
    public class Product
    {
        /// <summary>Identificador único do produto (Primary Key)</summary>
        public int Id { get; set; }

        /// <summary>Nome do produto (obrigatório, máximo 200 caracteres)</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Descrição detalhada do produto (opcional, máximo 1000 caracteres)</summary>
        public string? Description { get; set; }

        /// <summary>Preço do produto em decimal (obrigatório, deve ser >= 0)</summary>
        public decimal Price { get; set; }

        /// <summary>Quantidade em estoque (opcional, padrão 0, deve ser >= 0)</summary>
        public int? StockQuantity { get; set; }

        /// <summary>ID da categoria do produto (chave estrangeira, opcional)</summary>
        public int? CategoryId { get; set; }

        /// <summary>Indica se o produto está disponível para venda (padrão: true)</summary>
        public bool? IsAvailable { get; set; }

        /// <summary>Data e hora de criação do produto (preenchida automaticamente)</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Data e hora da última atualização (preenchida automaticamente)</summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
