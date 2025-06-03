

using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class StockRepository : IStockRepository
    {
         private readonly AppDBContext _context;
        public StockRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            stockModel.Symbol = stockModel.Symbol.ToUpper();
            await _context.Stocks.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);
            if (stockModel == null)
            {
                return null;
            }

            _context.Stocks.Remove(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<List<Stock>> GetAllAsync(QueryObject query)
        {
            var stocks = _context.Stocks.Include(c => c.Comments).AsQueryable();

            if (!string.IsNullOrEmpty(query.CompanyName))
            {
                stocks = stocks.Where(s => EF.Functions.ILike(s.CompanyName, $"%{query.CompanyName}%")); // Use ILike instead of Contains for case-insensitive search in PostgreSQL
            }
            if (!string.IsNullOrEmpty(query.Symbol))
            {
                stocks = stocks.Where(s => EF.Functions.ILike(s.Symbol, $"%{query.Symbol}%")); // Use ILike instead of Contains for case-insensitive search in PostgreSQL
            }

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                // Use OrdinalIgnoreCase to handle case-insensitive sort key comparison and ignore culture (not because of PostgreSQL)
                if (query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = query.IsDescending
                          ? stocks.OrderByDescending(s => s.Symbol)
                          : stocks.OrderBy(s => s.Symbol);
                } 
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize; // for pagination

            return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context.Stocks.Include(c => c.Comments).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol)
        {
            return await _context.Stocks
                .FirstOrDefaultAsync(s => s.Symbol == symbol.ToUpper()); // Use ToLower() for case-insensitive search, I save symbol in uppercase in the database - PostgreSQL
                // .Include(c => c.Comments)
        }

        public Task<bool> StockExists(int id)
        {
            return _context.Stocks.AnyAsync(x => x.Id == id);
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto stockDto)
        {
        var existingStock = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);
            if (existingStock == null)
            {
                return null;
            }

            existingStock.Symbol = stockDto.Symbol.ToUpper();
            existingStock.CompanyName = stockDto.CompanyName;
            existingStock.Purchase = stockDto.Purchase;
            existingStock.LastDiv = stockDto.LastDiv;
            existingStock.Industry = stockDto.Industry;
            existingStock.MarketCap = stockDto.MarketCap;
          
            await _context.SaveChangesAsync();

            return existingStock;
        }
    }
}