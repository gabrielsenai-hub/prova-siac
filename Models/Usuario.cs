using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ProvaSiac.Models
{
    public class Usuario : IdentityUser
    {
        public string Nome { get; set; }
        public DateTime DataCriacao { get; set; } =  DateTime.Now;
    }
}