using BeachGroupAPI.DAL;
using BeachGroupAPI.DTO;
using BeachGroupAPI.Model;

namespace BeachGroupAPI.Services;

public class ReservaService
{
    private readonly ReservaRepository _reservaRepository;
    private readonly UsuarioRepository _usuarioRepository;
    private readonly QuadraRepository _quadraRepository;

    public ReservaService(
        ReservaRepository reservaRepository,
        UsuarioRepository usuarioRepository,
        QuadraRepository quadraRepository)
    {
        _reservaRepository = reservaRepository;
        _usuarioRepository = usuarioRepository;
        _quadraRepository = quadraRepository;
    }

    public async Task<Reserva?> CriarAsync(CriarReservaDto dto)
    {
        //valida horário
        if (dto.HorInicio < 6 || dto.HorInicio > 23)
        {
            return null;
        }

        // valida usuário
        var usuario =
            await _usuarioRepository.BuscarPorIdAsync(dto.OidUsuario);

        if (usuario == null)
        {
            return null;
        }

        // valida quadra
        var quadra =
            await _quadraRepository.BuscarPorOidAsync(dto.OidQuadra);

        if (quadra == null)
        {
            return null;
        }

        // valida conflito de horário
        var existeReserva =
            await _reservaRepository.ExisteReservaAsync(
                dto.OidQuadra,
                dto.DatReserva,
                dto.HorInicio
            );

        if (existeReserva)
        {
            return null;
        }

        var reserva = new Reserva
        {
            OidUsuario = dto.OidUsuario,
            OidQuadra = dto.OidQuadra,

            DatReserva = dto.DatReserva,
            HorInicio = dto.HorInicio,

            VlrReserva = quadra.VlrQuadra,

            IndConfirmada = false,
            IndPago = false,

            VlrMultaCancelamento = 0,

            DatCriacao = DateTime.Now,
            DatAlteracao = DateTime.Now
        };

        return await _reservaRepository.CriarAsync(reserva);
    }

    public async Task<IEnumerable<Reserva>> ListarTodosAsync()
    {
        return await _reservaRepository.ListarTodosAsync();
    }

    public async Task<Reserva?> BuscarPorIdAsync(long id)
    {
        return await _reservaRepository.BuscarPorIdAsync(id);
    }

    public async Task<bool> CancelarAsync(long id)
    {
        var reserva = await _reservaRepository.BuscarPorIdAsync(id);
        if (reserva == null)
        {
            return false;
        }
        await _reservaRepository.DeletarAsync(id);
        return true;
    }
}