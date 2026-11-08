namespace BeachGroupAPI.DTO;

public class ReservaDto
{
    public long OidReserva { get; set; }

    public long OidUsuario { get; set; }

    public long OidQuadra { get; set; }

    public DateTime DatReserva { get; set; }

    public int HorInicio { get; set; }

    public decimal VlrReserva { get; set; }

    public bool IndConfirmada { get; set; }

    public bool IndPago { get; set; }

    public decimal VlrMultaCancelamento { get; set; }
}