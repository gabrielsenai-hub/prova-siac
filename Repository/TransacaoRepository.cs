using ProvaSiac.Data;
using ProvaSiac.Models;
using Microsoft.EntityFrameworkCore;

namespace ProvaSiac.Repository;

public class TransacaoRepository : ITransacaoRepository
{
    private readonly ApplicationDbContext _context;
    
    public TransacaoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Transacao>> GetAllAsync()
    {
        return await _context.Transacoes.AsNoTracking().ToListAsync();
    }
}