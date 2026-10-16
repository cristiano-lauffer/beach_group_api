using Dapper;
using MySqlConnector;
using BeachGroupAPI.Model;

namespace BeachGroupAPI.DAL
{
    //Cuida exclusivamente das consultas e persistência no MySQL
    public class UsuarioRepository
    {
        private readonly MySqlDataSource _dataSource;
        public UsuarioRepository(MySqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        //buscar por email
        public async Task<Usuario?> BuscarPorEmailAsync(string email)
        {
            const string sql = """
            SELECT
                oid_usuario AS OidUsuario,
                nom_usuario AS NomUsuario,
                nom_email AS NomEmail,
                senha AS Senha,
                dat_criacao AS DatCriacao,
                dat_alteracao AS DatAlteracao
            FROM usuarios
            WHERE nom_email = @Email
            LIMIT 1;
            """;
            await using var connection = await _dataSource.OpenConnectionAsync();

            return await connection.QueryFirstOrDefaultAsync<Usuario>(
                sql,
                new { Email = email }
            );
        }

        //buscar por id
        public async Task<Usuario?> BuscarPorIdAsync(long id)
        {
            const string sql = """
            SELECT
                oid_usuario AS OidUsuario,
                nom_usuario AS NomUsuario,
                nom_email AS NomEmail,
                senha AS Senha,
                dat_criacao AS DatCriacao,
                dat_alteracao AS DatAlteracao
            FROM usuarios
            WHERE oid_usuario = @Id
            LIMIT 1;
            """;
            await using var connection = await _dataSource.OpenConnectionAsync();

            return await connection.QueryFirstOrDefaultAsync<Usuario>(
                sql,
                new { Id = id }
            );
        }

        //cadastrar usuario
        public async Task<long> CriarAsync(Usuario usuario)
        {
            const string sql = """
        INSERT INTO usuarios
        (
            nom_usuario,
            nom_email,
            senha
        )
        VALUES
        (
            @NomUsuario,
            @NomEmail,
            @Senha
        );

        SELECT LAST_INSERT_ID();
        """;

            await using var connection = await _dataSource.OpenConnectionAsync();

            var id = await connection.ExecuteScalarAsync<long>(
                sql,
                new
                {
                    usuario.NomUsuario,
                    usuario.NomEmail,
                    usuario.Senha
                }
            );

            return id;
        }

        //listar usuarios
        public async Task<IEnumerable<Usuario>> ListarTodosAsync()
        {
            const string sql = """
        SELECT
            oid_usuario AS OidUsuario,
            nom_usuario AS NomUsuario,
            nom_email AS NomEmail,
            dat_criacao AS DatCriacao,
            dat_alteracao AS DatAlteracao
        FROM usuarios;
        """;
            await using var connection = await _dataSource.OpenConnectionAsync();
            return await connection.QueryAsync<Usuario>(sql);
        }

        //atualizar usuario
        public async Task<bool> AtualizarAsync(Usuario usuario)
        {
            const string sql = """
        UPDATE usuarios
        SET
            nom_usuario = @NomUsuario,
            nom_email = @NomEmail,
            senha = @Senha,
            dat_alteracao = @DatAlteracao
        WHERE oid_usuario = @OidUsuario;
        """;
            await using var connection = await _dataSource.OpenConnectionAsync();
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                usuario.OidUsuario,
                usuario.NomUsuario,
                usuario.NomEmail,
                usuario.Senha,
                usuario.DatAlteracao
            });
            return rowsAffected > 0;
        }

        //deletar usuario
        public async Task<bool> DeletarAsync(long id)
        {
            const string sql = """
        DELETE FROM usuarios
        WHERE oid_usuario = @Id;
        """;
            await using var connection = await _dataSource.OpenConnectionAsync();
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
    }
}