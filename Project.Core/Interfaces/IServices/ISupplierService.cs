using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Core.Entities.Business;

namespace Project.Core.Interfaces.IServices
{
    public interface ISupplierService :IBaseService<SupplierViewModel>
    {
        Task<SupplierViewModel> Create(SupplierCreateViewModel model, CancellationToken cancellationToken);
        Task Update(SupplierUpdateViewModel model, CancellationToken cancellationToken);
        Task Delete(int id, CancellationToken cancellationToken);

    }
}
