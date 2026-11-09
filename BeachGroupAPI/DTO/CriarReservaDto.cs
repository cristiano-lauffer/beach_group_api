namespace BeachGroupAPI.DTO;

public class CriarReservaDto
{
    public long OidUsuario { get; set; }

    public long OidQuadra { get; set; }

    public DateTime DatReserva { get; set; }

    public int HorInicio { get; set; }
}