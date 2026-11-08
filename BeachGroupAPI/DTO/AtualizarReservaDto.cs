namespace BeachGroupAPI.DTO;

public class AtualizarReservaDto
{
    public DateTime DatReserva { get; set; }

    public int HorInicio { get; set; }

    public bool IndConfirmada { get; set; }

    public bool IndPago { get; set; }
}