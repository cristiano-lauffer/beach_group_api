using System.Text;
using BeachGroupAPI;
using BeachGroupAPI.DAL;
using BeachGroupAPI.Services;
using BeachGroupAPI.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Configurando Autenticação via JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurando a política de CORS para que o navegador não bloqueie requisições vindas de outra porta
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000") // Portas padrão do Vite/React
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddMySqlDataSource(
    builder.Configuration.GetConnectionString("DefaultConnection")!
);

builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<QuadraRepository>();
builder.Services.AddScoped<QuadraService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

// Endpoints

//------------------------------------
// Usuários
//------------------------------------

//cadastro de usuário
app.MapPost("/api/usuarios", async (
    CriarUsuarioDto dto,
    UsuarioService usuarioService) =>
{
    var usuario = await usuarioService.CriarAsync(dto);

    if (usuario == null)
    {
        return Results.Conflict("Já existe um usuário com este e-mail.");
    }

    return Results.Created(
        $"/api/usuarios/{usuario.OidUsuario}",
        new
        {
            usuario.OidUsuario,
            usuario.NomUsuario,
            usuario.NomEmail
        }
    );
});

//listar usuários
app.MapGet("/api/usuarios", async (UsuarioService usuarioService) =>
{
    var usuarios = await usuarioService.ListarTodosAsync();
    return Results.Ok(usuarios);
});

//buscar usuário por id
app.MapGet("/api/usuarios/{id:long}", async (long id, UsuarioService usuarioService) =>
{
    var usuario = await usuarioService.BuscarPorIdAsync(id);
    if (usuario == null)
    {
        return Results.NotFound("Usuário não encontrado.");
    }
    return Results.Ok(usuario);
});

//atualizar usuário por id
app.MapPut("/api/usuarios/{id:long}", async (
    long id,
    AtualizarUsuarioDto dto,
    UsuarioService usuarioService) =>
{
    var usuarioAtualizado = await usuarioService.AtualizarAsync(id, dto);
    if (usuarioAtualizado == null)
    {
        return Results.NotFound("Usuário não encontrado.");
    }
    return Results.Ok(usuarioAtualizado);
});


//deletar usuário por id
app.MapDelete("/api/usuarios/{id:long}", async (long id, UsuarioService usuarioService) =>
{
    var usuario = await usuarioService.BuscarPorIdAsync(id);
    if (usuario == null)
    {
        return Results.NotFound("Usuário não encontrado.");
    }
    await usuarioService.DeletarAsync(id);
    return Results.Content("Usuário deletado com sucesso.");
});

//------------------------------------
// Login
//------------------------------------

//login de usuário
app.MapPost("/api/login", async (
    LoginDto login,
    UsuarioService usuarioService) =>
{
    var resultado = await usuarioService.LoginAsync(login);

    if (resultado == null)
    {
    return Results.BadRequest("E-mail ou senha inválidos.");
    }

    return Results.Ok(resultado);
});

//------------------------------------
// Quadras
//------------------------------------

//buscar quadra por id
app.MapGet("/api/quadras/{id:long}", async (long id, QuadraService quadraService) =>
{
    var quadra = await quadraService.BuscarPorOidAsync(id);
    if (quadra == null)
    {
        return Results.NotFound("Quadra não encontrado.");
    }

    return Results.Ok(quadra);
});

//listar quadras
app.MapGet("/api/quadras", async (QuadraService quadraService) =>
{
    var usuarios = await quadraService.ListarTodosAsync();
    return Results.Ok(usuarios);
});

//cadastrar quadras
app.MapPost("/api/quadras", async (
    QuadraDto dto,
    QuadraService quadraService) =>
{
    var quadra = await quadraService.CriarAsync(dto);

    if (quadra == null)
    {
        return Results.Conflict("Já existe uma quadra com este nome.");
    }

    return Results.Created(
        $"/api/quadras/{quadra.Oid}",
        new
        {
            quadra.Nome,
            quadra.HorFuncionamentoInicio,
            quadra.HorFuncionamentoFim,
            quadra.VlrQuadra,
            quadra.DatCriacao
        }
    );
});

//atualizar quadra por id
app.MapPut("/api/quadras/{id:long}", async (
    long id,
    QuadraDto dto,
    QuadraService quadraService) =>
{
    var quadra= await quadraService.AtualizarAsync(id, dto);
    if (quadra == null)
    {
        return Results.NotFound("Quadra não encontrada.");
    }
    return Results.Ok(quadra);
});


//deletar quadra por id
app.MapDelete("/api/quadras/{id:long}", async (long id, QuadraService quadraService) =>
{
    var quadra = await quadraService.BuscarPorOidAsync(id);
    if (quadra == null)
    {
        return Results.NotFound("Quadra não encontrada.");
    }
    await quadraService.DeletarAsync(id);
    return Results.Content("Quadra deletada com sucesso.");
});

app.Run();