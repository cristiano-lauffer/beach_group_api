using System.Text;
using BeachGroupAPI.DAL;
using BeachGroupAPI.Services;
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

// Endpoints

//cadastro de usuário
app.MapPost("/api/usuarios", async (
    BeachGroupAPI.DTO.CriarUsuarioDto dto,
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
    BeachGroupAPI.DTO.AtualizarUsuarioDto dto,
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


//login de usuário
app.MapPost("/api/login", async (
    BeachGroupAPI.DTO.LoginDto login,
    UsuarioService usuarioService) =>
{
    var resultado = await usuarioService.LoginAsync(login);

    if (resultado == null)
    {
    return Results.BadRequest("E-mail ou senha inválidos.");
    }

    return Results.Ok(resultado);
});

app.Run();