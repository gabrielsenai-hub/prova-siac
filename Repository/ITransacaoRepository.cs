using ProvaSiac.Models;

namespace ProvaSiac.Repository;

public interface ITransacaoRepository
{
    public Task<List<Transacao>> GetAllAsync();
}