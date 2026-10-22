namespace BeachGroupAPI.DTO
{//DTO - Agrupa os contratos de entrada e saída da API
    public class QuadraDto
    {
        public long Oid { get; set; }

        public string Nome { get; set; } = string.Empty;
        
        public int HorFuncionamentoInicio { get; set; }
        
        public int HorFuncionamentoFim { get; set; }
        
        public Decimal VlrQuadra { get; set; }

        public DateTime DatCriacao { get; set; }

        public DateTime? DatAlteracao { get; set; }
    }
}
