using ProvaSiac.Data;
using ProvaSiac.Models;

namespace ProvaSiac.Repository;

public interface IProdutoRepository
{
    Task<List<Produto>> GetAllProdutos();
    Task<Produto> GetProdutoById(int id);
    Task DeleteProduto(Produto produto);
    Task CreateProduto(Produto produto);
    Task UpdateProduto(Produto produto);
}