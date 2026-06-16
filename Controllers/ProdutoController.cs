using System.Security.Claims;
using ProvaSiac.Models;
using ProvaSiac.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ProvaSiac.Controllers;

public class ProdutoController : Controller
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly ITransacaoRepository _transacaoRepository;

    public ProdutoController(IProdutoRepository produtoRepository, ITransacaoRepository transacaoRepository)
    {
        _produtoRepository = produtoRepository;
        _transacaoRepository = transacaoRepository;
    }

    [HttpPost("Criar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(Produto produto)
    {

        produto.IdUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        await _produtoRepository.CreateProduto(produto);

        TempData["Sucesso"] = "Produto criado com sucesso.";
        return RedirectToAction("Index", "Home");
    }
[HttpPost("RemoverEstoque")]
public async Task<IActionResult> RemoverEstoque(int id, int quantidadeRemover)
{
    if (quantidadeRemover <= 0)
    {
        TempData["Erro"] = "A quantidade a ser removida deve ser maior que zero.";
        return RedirectToAction("Index", "Home");
    }

    bool sucesso = await _produtoRepository.RemoverEstoqueProduto(id, quantidadeRemover);
    
    if (!sucesso)
    {
        TempData["Erro"] = "Estoque insuficiente para concluir a operação.";
    }
    
    return RedirectToAction("Index", "Home");
}
    [HttpPost("Editar")]
    public async Task<IActionResult> Editar(Produto produto)
    {

        await _produtoRepository.UpdateProduto(produto);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost("Excluir")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Master")]
    public async Task<IActionResult> Excluir(int id)
    {
        var produto = await _produtoRepository.GetProdutoById(id);

        if (produto == null)
        {
            TempData["Erro"] = "Produto não encontrado.";
            return RedirectToAction("Index", "Home");
        }

        await _produtoRepository.DeleteProduto(produto);

        TempData["Sucesso"] = "Produto excluído com sucesso.";
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Log()
    {
        var lista = await _transacaoRepository.GetAllAsync();
        return View(lista);
    }
}