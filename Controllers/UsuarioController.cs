using ProvaSiac.Models;
using ProvaSiac.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ProvaSiac.Controllers;

public class UsuarioController : Controller
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;

    public UsuarioController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(string email, string senha)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            TempData["Erro"] = "Usuário não encontrado";
            return RedirectToAction("Login", "Home");
        }

        if (!await _userManager.CheckPasswordAsync(user, senha))
        {
            TempData["Erro"] = "Senha incorreta";
            return RedirectToAction("Login", "Home");
        }

        await _signInManager.PasswordSignInAsync(email, senha, true, lockoutOnFailure: false);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost("Cadastro")]
    public async Task<IActionResult> Cadastro(CadastroDto dto)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("Cadastro", "Home");

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user != null)
        {
            TempData["Erro"] = "Usuário já existente";
            return RedirectToAction("Cadastro", "Home");
        }

        var newUser = new Usuario()
        {
            Nome = dto.Nome,
            Email = dto.Email,
            UserName = dto.Email
        };
        var result = await _userManager.CreateAsync(newUser, dto.Senha);

        if (!result.Succeeded)
        {
            TempData["Erro"] = string.Join(", ",
                result.Errors.Select(e => e.Description));

            return RedirectToAction("Cadastro", "Home");
        }

        await _userManager.AddToRoleAsync(newUser, "Operador");
        await _signInManager.SignInAsync(newUser, false);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("Logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}