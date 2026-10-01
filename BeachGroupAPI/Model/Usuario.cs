namespace BeachGroupAPI.Model
{
    //Representa a entidade de dominio
    public class Usuario
    {
        public long OidUsuario { get; set; }
        public string NomUsuario { get; set; } = string.Empty;
        public string NomEmail { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public DateTime DatCriacao { get; set; }
        public DateTime? DatAlteracao { get; set; }

    }
}
