using BeachGroupAPI.Model;
using Dapper;
using MySqlConnector;

namespace BeachGroupAPI.DAL;

public class ReservaRepository
{
    private readonly MySqlDataSource _dataSource;

    public ReservaRepository(MySqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IEnumerable<Reserva>> ListarTodosAsync()
    {
        const string sql = """
        SELECT
            oid_reserva AS OidReserva,
            oid_usuario AS OidUsuario,
            oid_quadra AS OidQuadra,
            dat_reserva AS DatReserva,
            hor_inicio AS HorInicio,
            vlr_reserva AS VlrReserva,
            ind_confirmada AS IndConfirmada,
            ind_pago AS IndPago,
            vlr_multa_cancelamento AS VlrMultaCancelamento,
            dat_criacao AS DatCriacao,
            dat_alteracao AS DatAlteracao
        FROM reservas
        ORDER BY dat_reserva, hor_inicio
        """;

        await using var connection =
            await _dataSource.OpenConnectionAsync();

        return await connection.QueryAsync<Reserva>(sql);
    }

    public async Task<Reserva?> BuscarPorIdAsync(long id)
    {
        const string sql = """
        SELECT
            oid_reserva AS OidReserva,
            oid_usuario AS OidUsuario,
            oid_quadra AS OidQuadra,
            dat_reserva AS DatReserva,
            hor_inicio AS HorInicio,
            vlr_reserva AS VlrReserva,
            ind_confirmada AS IndConfirmada,
            ind_pago AS IndPago,
            vlr_multa_cancelamento AS VlrMultaCancelamento,
            dat_criacao AS DatCriacao,
            dat_alteracao AS DatAlteracao
        FROM reservas
        WHERE oid_reserva = @Id
        """;

        await using var connection =
            await _dataSource.OpenConnectionAsync();

        return await connection.QueryFirstOrDefaultAsync<Reserva>(
            sql,
            new { Id = id }
        );
    }

    public async Task<Reserva> CriarAsync(Reserva reserva)
    {
        const string sql = """
        INSERT INTO reservas
        (
            oid_usuario,
            oid_quadra,
            dat_reserva,
            hor_inicio,
            vlr_reserva,
            ind_confirmada,
            ind_pago,
            vlr_multa_cancelamento,
            dat_criacao,
            dat_alteracao
        )
        VALUES
        (
            @OidUsuario,
            @OidQuadra,
            @DatReserva,
            @HorInicio,
            @VlrReserva,
            @IndConfirmada,
            @IndPago,
            @VlrMultaCancelamento,
            @DatCriacao,
            @DatAlteracao
        );

        SELECT LAST_INSERT_ID();
        """;

        await using var connection =
            await _dataSource.OpenConnectionAsync();

        reserva.OidReserva =
            await connection.ExecuteScalarAsync<long>(
                sql,
                reserva
            );

        return reserva;
    }
    public async Task<bool> ExisteReservaAsync(
    long oidQuadra,
    DateTime datReserva,
    int horInicio)
    {
        const string sql = """
    SELECT COUNT(*)
    FROM reservas
    WHERE oid_quadra = @OidQuadra
      AND dat_reserva = @DatReserva
      AND hor_inicio = @HorInicio
    """;

        await using var connection =
            await _dataSource.OpenConnectionAsync();

        var quantidade =
            await connection.ExecuteScalarAsync<int>(
                sql,
                new
                {
                    OidQuadra = oidQuadra,
                    DatReserva = datReserva.Date,
                    HorInicio = horInicio
                });

        return quantidade > 0;
    }
}