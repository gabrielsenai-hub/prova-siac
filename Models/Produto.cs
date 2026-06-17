using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProvaSiac.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Marca { get; set; }
        public string? Modelo { get; set; }
        public int Quantidade { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}