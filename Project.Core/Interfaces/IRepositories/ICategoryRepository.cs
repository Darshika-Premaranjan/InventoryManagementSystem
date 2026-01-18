using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories
{
    public interface ICategoryRepository :IBaseRepository<Category>
    {
        //custom methods for category
        Task<bool> IsCategoryNameExist(string name, CancellationToken cancellationToken = default);
    }
}
