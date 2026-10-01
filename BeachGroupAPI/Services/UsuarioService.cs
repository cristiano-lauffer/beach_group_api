using BeachGroupAPI.DAL;
using BeachGroupAPI.DTO;
using BeachGroupAPI.Model;

namespace BeachGroupAPI.Services;

public class UsuarioService
{
    //Coordenam a lógica de aplicação
    private readonly UsuarioRepository _usuarioRepository;
    private readonly TokenService _tokenService;

    public UsuarioService(UsuarioRepository usuarioRepository, TokenService tokenService)
    {
        _usuarioRepository = usuarioRepository;
        _tokenService = tokenService;
    }

    //LOGIN, lógica devalidação de senha e geração de token
    public async Task<LoginResponseDto?> LoginAsync(LoginDto login)
    {
        var usuario = await _usuarioRepository.BuscarPorEmailAsync(login.Email);

        if (usuario == null)
        {
            return null;
        }

        bool senhaValida = BCrypt.Net.BCrypt.Verify(login.Senha, usuario.Senha);

        if (!senhaValida)
        {
            return null;
        }

        // Gerar o JWT Token
        var token = _tokenService.GerarToken(usuario);

        return new LoginResponseDto
        {
            Id = usuario.OidUsuario,
            Nome = usuario.NomUsuario,
            Email = usuario.NomEmail,
            Token = token
        };
    }


    //CADASTRAR - lógica de criação de usuario, validação de email e hash da senha
    public async Task<Usuario?> CriarAsync(CriarUsuarioDto dto)
    {
        var usuarioExistente = await _usuarioRepository.BuscarPorEmailAsync(dto.Email);

        if (usuarioExistente != null)
        {
            return null;
        }

        var senhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);

        var usuario = new Usuario
        {
            NomUsuario = dto.Nome,
            NomEmail = dto.Email,
            Senha = senhaHash,
            DatCriacao = DateTime.Now
        };

        var id = await _usuarioRepository.CriarAsync(usuario);
        usuario.OidUsuario = id;

        return usuario;
    }

    //LISTAR - lógica de listagem de usuarios

    public async Task<IEnumerable<UsuarioDto>> ListarTodosAsync()
    {
        var usuarios = await _usuarioRepository.ListarTodosAsync();
        return usuarios.Select(u => new UsuarioDto
        {
            Id = u.OidUsuario,
            Nome = u.NomUsuario,
            Email = u.NomEmail,
            DatCriacao = u.DatCriacao
        });
    }

    //BUSCAR POR ID - lógica de busca de usuario por id
    public async Task<UsuarioDto?> BuscarPorIdAsync(long id)
    {
        var usuario = await _usuarioRepository.BuscarPorIdAsync(id);
        if (usuario == null)
        {
            return null;
        }
        return new UsuarioDto
        {
            Id = usuario.OidUsuario,
            Nome = usuario.NomUsuario,
            Email = usuario.NomEmail,
            DatCriacao = usuario.DatCriacao
        };
    }

    //ATUALIZAR - lógica de atualização de usuario
    public async Task<UsuarioDto?> AtualizarAsync(long id, AtualizarUsuarioDto dto)
    {
        var usuario = await _usuarioRepository.BuscarPorIdAsync(id);
        if (usuario == null) return null;

        if (!string.IsNullOrWhiteSpace(dto.Nome))
            usuario.NomUsuario = dto.Nome;

        if (!string.IsNullOrWhiteSpace(dto.Email))
            usuario.NomEmail = dto.Email;

        if (!string.IsNullOrWhiteSpace(dto.Senha))
        {
            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
        }

        usuario.DatAlteracao = DateTime.Now;

        await _usuarioRepository.AtualizarAsync(usuario);

        return new UsuarioDto
        {
            Id = usuario.OidUsuario,
            Nome = usuario.NomUsuario,
            Email = usuario.NomEmail,
            DatCriacao = usuario.DatCriacao
        };
    }

    //DELETAR - lógica de deleção de usuario
    public async Task<bool> DeletarAsync(long id)
    {
        var usuario = await _usuarioRepository.BuscarPorIdAsync(id);
        if (usuario == null)
        {
            return false;
        }
        await _usuarioRepository.DeletarAsync(id);
        return true;
    }

}