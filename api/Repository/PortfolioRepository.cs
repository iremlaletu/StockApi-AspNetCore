

using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly AppDBContext _context;
        public PortfolioRepository(AppDBContext context)
        {
            // Initialize the repository with the database context. This allows access to the database for portfolio operations
            _context = context;
        }

        public async Task<List<Stock>> GetUserPortfolio(AppUser user)
        {
            // Returns all stocks in the user's portfolio by matching their user ID
            return await _context.Portfolios
                .Where(p => p.AppUserId == user.Id)
                .Select(stock => new Stock
                {
                    Id = stock.StockId,
                    Symbol = stock.Stock.Symbol,
                    CompanyName = stock.Stock.CompanyName,
                    Purchase = stock.Stock.Purchase,
                    LastDiv = stock.Stock.LastDiv,
                    Industry = stock.Stock.Industry,
                    MarketCap = stock.Stock.MarketCap,

                })
                .ToListAsync();

        }
    }
}

// This repository handles data access logic related to the user's stock portfolio.