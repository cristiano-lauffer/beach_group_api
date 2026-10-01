namespace BeachGroupAPI.DTO
{
    //DTO - Agrupa os contratos de entrada e saída da API
    public class UsuarioDto
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DatCriacao { get; set; }
    }
}
