using BeachGroupAPI.DAL;
using BeachGroupAPI.DTO;
using BeachGroupAPI.Model;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BeachGroupAPI.Services;

public class QuadraService
{
    private readonly QuadraRepository _quadraRepository;

    public QuadraService(QuadraRepository quadraRepository)
    {
        _quadraRepository = quadraRepository;
    }

    public async Task<QuadraDto?> BuscarPorOidAsync(long id)
    {
        return await _quadraRepository.BuscarPorOidAsync(id);
    }

    public async Task<IEnumerable<QuadraDto>> ListarTodosAsync()
    {
        var quadras = await _quadraRepository.ListarTodosAsync();
        return quadras.Select(o => new QuadraDto
        {
            Oid = o.Oid,
            Nome = o.Nome,
            HorFuncionamentoInicio = o.HorFuncionamentoInicio,
            HorFuncionamentoFim = o.HorFuncionamentoFim,
            VlrQuadra = o.VlrQuadra,
            DatCriacao = o.DatCriacao,
            DatAlteracao = o.DatAlteracao
        });
    }

    public async Task<QuadraDto?> CriarAsync(QuadraDto dto)
    {
        var quadraExistente = await _quadraRepository.BuscarPorNomeAsync(dto.Nome);

        if (quadraExistente != null)
        {
            return null;
        }

        return await _quadraRepository.CriarAsync(dto);
    }

    public async Task<QuadraDto?> AtualizarAsync(long id, QuadraDto dto)
    {
        throw new Exception("Não implementado");
    }

    public async Task<bool> DeletarAsync(long id)
    {
        var usuario = await _quadraRepository.BuscarPorOidAsync(id);
        if (usuario == null)
        {
            return false;
        }
        await _quadraRepository.DeletarAsync(id);
        return true;
    }
}