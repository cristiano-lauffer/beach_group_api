using System.Data.Common;

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