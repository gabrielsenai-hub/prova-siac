using System.Security.Claims;
using ProvaSiac.Controllers;
using ProvaSiac.Models;
using ProvaSiac.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

namespace ProvaSiac.tests.ProvaSiac.Unit.tests;

public class ProdutoControllerTest
{
    private readonly Mock<IProdutoRepository> _mockProdutoRepository;
    private readonly Mock<ITransacaoRepository> _mockTransacaoRepository;
    private readonly Mock<ITempDataDictionary> _mockTempData;
    private readonly ProdutoController _controller;

    public ProdutoControllerTest()
    {
        _mockProdutoRepository = new Mock<IProdutoRepository>();
        _mockTransacaoRepository = new Mock<ITransacaoRepository>();
        _mockTempData = new Mock<ITempDataDictionary>();

        var usuarioClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "usuario-diretorio-123")
        }, "mock"));

        _controller = new ProdutoController(_mockProdutoRepository.Object, _mockTransacaoRepository.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = usuarioClaims }
            },
            TempData = _mockTempData.Object
        };
    }

    [Fact]
    public async Task Criar_DeveDefinirIdUsuarioESalvarProdutoComSucesso()
    {
        var novoProduto = new Produto { Id = 1, Nome = "Sabonete", Quantidade = 10, Marca="Ypê", Modelo = "Liquido"};

        _mockProdutoRepository.Setup(r => r.CreateProduto(It.IsAny<Produto>()))
            .Returns(Task.CompletedTask);

        var resultado = await _controller.Criar(novoProduto);

        var redirectResult = Assert.IsType<RedirectToActionResult>(resultado);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);

        Assert.Equal("usuario-diretorio-123", novoProduto.IdUsuario);

        _mockProdutoRepository.Verify(r => r.CreateProduto(novoProduto), Times.Once);
        _mockTempData.VerifySet(t => t["Sucesso"] = "Produto criado com sucesso.", Times.Once);
    }

    [Fact]
    public async Task Editar_DeveAtualizarProdutoERedirecionar()
    {
        var produtoEditado = new Produto { Id = 1, Nome = "Sabonete", Quantidade = 5, Marca="Ypê", Modelo = "Liquido"};

        _mockProdutoRepository.Setup(r => r.UpdateProduto(It.IsAny<Produto>()))
            .Returns(Task.CompletedTask);

        var resultado = await _controller.Editar(produtoEditado);

        var redirectResult = Assert.IsType<RedirectToActionResult>(resultado);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);

        _mockProdutoRepository.Verify(r => r.UpdateProduto(produtoEditado), Times.Once);
    }

    [Fact]
    public async Task Excluir_DeveRetornarErro_QuandoProdutoNaoExistir()
    {
        int idInexistere = 99;
        _mockProdutoRepository.Setup(r => r.GetProdutoById(idInexistere))
            .ReturnsAsync((Produto)null!);

        var resultado = await _controller.Excluir(idInexistere);

        var redirectResult = Assert.IsType<RedirectToActionResult>(resultado);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);

        _mockTempData.VerifySet(t => t["Erro"] = "Produto não encontrado.", Times.Once);
        _mockProdutoRepository.Verify(r => r.DeleteProduto(It.IsAny<Produto>()), Times.Never);
    }

    [Fact]
    public async Task Excluir_DeveDeletarProdutoComSucesso_QuandoProdutoExistir()
    {
        int idExistente = 1;
        var produtoExistente = new Produto { Id = idExistente, Nome = "Sabonete", Quantidade = 5, Marca="Ypê", Modelo = "Liquido"};

        _mockProdutoRepository.Setup(r => r.GetProdutoById(idExistente))
            .ReturnsAsync(produtoExistente);
        _mockProdutoRepository.Setup(r => r.DeleteProduto(produtoExistente))
            .Returns(Task.CompletedTask);

        var resultado = await _controller.Excluir(idExistente);

        var redirectResult = Assert.IsType<RedirectToActionResult>(resultado);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);

        _mockProdutoRepository.Verify(r => r.DeleteProduto(produtoExistente), Times.Once);
        _mockTempData.VerifySet(t => t["Sucesso"] = "Produto excluído com sucesso.", Times.Once);
    }

    [Fact]
    public async Task Log_DeveRetornarViewComListaDeTransacoes()
    {
        var listaTransacoesEsperada = new List<Transacao>
        {
            new() { Id = 1, NomePoduto = "Produto A", Quantidade = 5, Tipo = TipoTransacao.ADICAO, Usuario = "User1", Marca = "A", Modelo ="C"},
            new() { Id = 2, NomePoduto = "Produto B", Quantidade = 2, Tipo = TipoTransacao.REMOCAO, Usuario = "User2", Marca = "B", Modelo ="D" }
        };

        _mockTransacaoRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(listaTransacoesEsperada);

        var resultado = await _controller.Log();

        var viewResult = Assert.IsType<ViewResult>(resultado);
        var modelRetornado = Assert.IsAssignableFrom<IEnumerable<Transacao>>(viewResult.Model);
        
        Assert.Equal(listaTransacoesEsperada.Count, modelRetornado.Count());
        _mockTransacaoRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }
}