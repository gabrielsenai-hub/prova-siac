using ProvaSiac.Data;
using ProvaSiac.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ProvaSiac.Repository;

public class ProdutoRepository : IProdutoRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Usuario> _userManager;

    public ProdutoRepository(
        UserManager<Usuario> userManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<List<Produto>> GetAllProdutos()
    {
        return await _context.Produtos
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Produto?> GetProdutoById(int id)
    {
        return await _context.Produtos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    private async Task GerarLog(TipoTransacao tipoTransacao, Produto produto)
    {
        var user = await _userManager.FindByIdAsync(produto.IdUsuario);

        var transacao = new Transacao()
        {
            Usuario = user?.Nome ?? "Usuário não encontrado",
            IdProduto = produto.Id,
            NomePoduto = produto.Nome,
            Marca = produto.Marca,
            Quantidade = produto.Quantidade,
            Tipo = tipoTransacao
        };

        _context.Transacoes.Add(transacao);
    }

    public async Task DeleteProduto(Produto produto)
    {
        var produtoToDelete = await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == produto.Id);

        if (produtoToDelete == null)
            return;

        _context.Produtos.Remove(produtoToDelete);

        await GerarLog(TipoTransacao.REMOCAO, produtoToDelete);

        await _context.SaveChangesAsync();
    }

    public async Task CreateProduto(Produto produto)
    {
        _context.Produtos.Add(produto);

        await GerarLog(TipoTransacao.ADICAO, produto);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateProduto(Produto produto)
    {
        _context.Produtos.Update(produto);

        await GerarLog(TipoTransacao.ATUALIZACAO, produto);

        await _context.SaveChangesAsync();
    }
}