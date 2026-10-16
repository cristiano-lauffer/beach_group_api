namespace BeachGroupAPI.DTO
{
    //DTO - Agrupa os contratos de entrada e saída da API
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
