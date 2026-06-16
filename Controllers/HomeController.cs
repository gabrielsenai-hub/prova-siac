using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProvaSiac.Models;
using ProvaSiac.Repository;

namespace ProvaSiac.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProdutoRepository _produtoRepository;

    public HomeController(ILogger<HomeController> logger,  IProdutoRepository produtoRepository)
    {
        _logger = logger;
        _produtoRepository = produtoRepository;
    }

    public async Task<IActionResult> Index()
    {
        var lista = await _produtoRepository.GetAllProdutos();
        return View(lista);
    }
    public IActionResult Login()
    {
        if(User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home");
        return View();
        
    }
    public IActionResult Cadastro()
    {
        if(User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home");
        return View();
        
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
