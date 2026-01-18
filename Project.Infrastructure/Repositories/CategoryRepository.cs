using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public  CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
            {
            }
    public async Task<bool> IsCategoryNameExist(string name,CancellationToken cancellationToken = default)
        {
            return await _dbContext.Categories.AnyAsync(c  => c.Name == name, cancellationToken);
        }
    }
}
