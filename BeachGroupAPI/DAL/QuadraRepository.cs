using BeachGroupAPI.DTO;
using BeachGroupAPI.Model;
using Dapper;
using MySqlConnector;
using System.Security.Cryptography;

namespace BeachGroupAPI.DAL
{
    public class QuadraRepository
    {
        private readonly MySqlDataSource _dataSource;
        public QuadraRepository(MySqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<QuadraDto?> BuscarPorOidAsync(long oid)
        {
            const string sql = """
            SELECT
                oid_quadra as Oid,
                nom_quadra as Nome,
                hor_funcionamento_inicio as HorFuncionamentoInicio,
                hor_funcionamento_fim as HorFuncionamentoFim,
                vlr_quadra as VlrQuadra,
                dat_criacao DatCriacao,
                dat_alteracao DatAlteracao
            FROM quadras
            WHERE oid_quadra = @p_oid
            LIMIT 1;
            """;
            await using var connection = await _dataSource.OpenConnectionAsync();

            return await connection.QueryFirstOrDefaultAsync<QuadraDto>(
                sql,
                new
                {
                    p_oid = oid
                }
            );
        }

        public async Task<QuadraDto?> BuscarPorNomeAsync(string nome)
        {
            const string sql = """
            SELECT
                oid_quadra as Oid,
                nom_quadra as Nome,
                hor_funcionamento_inicio as HorFuncionamentoInicio,
                hor_funcionamento_fim as HorFuncionamentoFim,
                vlr_quadra as VlrQuadra,
                dat_criacao DatCriacao,
                dat_alteracao DatAlteracao
            FROM quadras
            WHERE nom_quadra = @p_nome
            LIMIT 1;
            """;
            await using var connection = await _dataSource.OpenConnectionAsync();

            return await connection.QueryFirstOrDefaultAsync<QuadraDto>(
                sql,
                new {
                    p_nome = nome
                }
            );
        }

        public async Task<IEnumerable<QuadraDto>> ListarTodosAsync()
        {
            const string sql = """
        SELECT
            oid_quadra Oid,
            nom_quadra Nome,
            hor_funcionamento_inicio HorFuncionamentoInicio,
            hor_funcionamento_fim HorFuncionamentoFim,
            vlr_quadra VlrQuadra,
            dat_criacao DatCriacao,
            dat_alteracao DatAlteracao
        FROM quadras;
        """;
            await using var connection = await _dataSource.OpenConnectionAsync();
            return await connection.QueryAsync<QuadraDto>(sql);
        }

        public async Task<QuadraDto?> CriarAsync(QuadraDto quadra)
        {
            const string sql = """
        INSERT INTO quadras (
            nom_quadra,
            hor_funcionamento_inicio,
            hor_funcionamento_fim,
            vlr_quadra,
            dat_criacao
        )
        VALUES(
            @Nome,
            @HorFuncionamentoInicio,
            @HorFuncionamentoFim,
            @VlrQuadra,
            current_timestamp()
        );

        SELECT LAST_INSERT_ID();
        """;

            await using var connection = await _dataSource.OpenConnectionAsync();

            var id = await connection.ExecuteScalarAsync<long>(
                sql,
                new
                {
                    quadra.Nome,
                    quadra.HorFuncionamentoInicio,
                    quadra.HorFuncionamentoFim,
                    quadra.VlrQuadra
                }
            );            

            return await this.BuscarPorOidAsync(id);
        }

        public async Task<bool> AtualizarAsync(QuadraDto quadraDto)
        {
            throw new Exception("Não implementado");
        }

        public async Task<bool> DeletarAsync(long id)
        {
            const string sql = """
        DELETE FROM quadras
        WHERE oid_quadra = @Id;
        """;
            await using var connection = await _dataSource.OpenConnectionAsync();
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
    }
}
