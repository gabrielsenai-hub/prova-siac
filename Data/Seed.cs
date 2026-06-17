using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProvaSiac.Models;
using Microsoft.AspNetCore.Identity;

public class Seed
    {
        private const string MasterRole = "Master";
        private const string AdminEmail = "adminMaster@gmail.com";
        private const string AdminPassword = "MeuSiac123";
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            // Garante que a Role Master exista antes de tentar atribuí-la ao usuário
            if (!await roleManager.RoleExistsAsync(MasterRole))
            {
                await roleManager.CreateAsync(new IdentityRole(MasterRole));
            }
            if (!await roleManager.RoleExistsAsync("Operador"))
            {
                await roleManager.CreateAsync(new IdentityRole("Operador"));
            }
        }

        // ==========================================================
        // MÉTODO 2: Criação e Atribuição do Usuário Master
        // ==========================================================
        public static async Task SeedMasterUser(UserManager<Usuario> userManager)
        {
            // 1. Tenta encontrar o usuário pelo email
            var user = await userManager.FindByEmailAsync(AdminEmail);

            if (user == null)
            {
                // Usuário não existe, então cria

                var masterUser = new Usuario
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    EmailConfirmed = true,
                    Nome = "Master Administrator",
                };

                var createResult = await userManager.CreateAsync(masterUser, AdminPassword);

                if (createResult.Succeeded)
                {
                    // Atribui a Role Master APÓS a criação
                    var roleResult = await userManager.AddToRoleAsync(masterUser, MasterRole);

                    if (!roleResult.Succeeded)
                    {
                        // Logar erro de atribuição de Role
                        System.Console.WriteLine($"ERRO: Falha ao atribuir Role Master ao novo usuário.");
                    }
                }
                else
                {
                    // Logar erro de criação de usuário
                    System.Console.WriteLine($"ERRO: Falha ao criar usuário Master: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                // Usuário existe, garante que ele está na Role Master (caso tenha sido removido)
                if (!await userManager.IsInRoleAsync(user, MasterRole))
                {
                    await userManager.AddToRoleAsync(user, MasterRole);
                }
            }
        }
    }