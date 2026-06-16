namespace ProvaSiac.Models;

public class Transacao
{
    public int Id { get; set; }
    public string Usuario { get; set; }
    public int IdProduto { get; set; }
    public string NomePoduto { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public int Quantidade { get; set; }
    public TipoTransacao Tipo { get; set; }
    public DateTime Horario { get; set; } = DateTime.Now;
}