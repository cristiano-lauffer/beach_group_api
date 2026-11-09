namespace BeachGroupAPI.Model;

public class Reserva
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

    public DateTime DatCriacao { get; set; }

    public DateTime DatAlteracao { get; set; }
}