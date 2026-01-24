using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Core.Entities.Business;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IMapper;
using Project.Core.Interfaces.IRepositories;
using Project.Core.Interfaces.IServices;
using Project.Core.Mapper;

namespace Project.Core.Services
{
    public class SupplierService : BaseService<Supplier, SupplierViewModel>, ISupplierService
    {
        private readonly IBaseMapper<Supplier, SupplierViewModel> _supplierViewModelMapper;
        private readonly IBaseMapper<SupplierCreateViewModel,Supplier> _supplierCreateMapper;
        private readonly IBaseMapper<SupplierUpdateViewModel,Supplier> _supplierUpdateMapper;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IUserContext _userContext;
        public SupplierService(
            IBaseMapper<Supplier, SupplierViewModel> supplierViewModelMapper,
            IBaseMapper<SupplierCreateViewModel, Supplier> supplierCreateMapper,
            IBaseMapper<SupplierUpdateViewModel, Supplier> supplierUpdateMapper,
            ISupplierRepository supplierRepository,
            IUserContext userContext) 
            : base(supplierViewModelMapper, supplierRepository)
        {
            _supplierCreateMapper = supplierCreateMapper;
            _supplierUpdateMapper = supplierUpdateMapper;
            _supplierViewModelMapper = supplierViewModelMapper;
            _supplierRepository = supplierRepository;
            _userContext = userContext;

        }
        public async Task<SupplierViewModel> Create(SupplierCreateViewModel model, CancellationToken cancellationToken)
        {
            var entity = _supplierCreateMapper.MapModel(model);
            entity.EntryDate = DateTime.Now;
            entity.EntryBy = Convert.ToInt32(_userContext.UserId);
            return _supplierViewModelMapper.MapModel(await _supplierRepository.Create(entity, cancellationToken));
        }
        public async Task Update(SupplierUpdateViewModel model, CancellationToken cancellationToken)
        {
            var existingData = await _supplierRepository.GetById(model.Id, cancellationToken);

            //Mapping through AutoMapper
            _supplierUpdateMapper.MapModel(model, existingData);

            // Set additional properties or perform other logic as needed
            existingData.UpdatedDate = DateTime.Now;
            existingData.UpdatedBy = Convert.ToInt32(_userContext.UserId);

            await _supplierRepository.Update(existingData, cancellationToken);
        }

        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            var entity = await _supplierRepository.GetById(id, cancellationToken);
            await _supplierRepository.Delete(entity, cancellationToken);
        }

    }
}
