using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Core.Entities.Business;

namespace Project.Core.Interfaces.IServices
{
    public interface ICategoryService : IBaseService<CategoryViewModel>
    {
        Task<CategoryViewModel> Create(CategoryCreateViewModel model, CancellationToken cancellationToken);
        Task Update(CategoryUpdateViewModel model, CancellationToken cancellationToken);
        Task Delete(int id, CancellationToken cancellationToken);
        Task<bool> IsCategoryNameExist(string name, CancellationToken cancellationToken);
    }
}
