using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Project.Core.Entities.Business;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IMapper;
using Project.Core.Interfaces.IRepositories;
using Project.Core.Interfaces.IServices;

namespace Project.Core.Services
{
    public class CategoryService : BaseService<Category, CategoryViewModel>, ICategoryService
    {
        private readonly IBaseMapper<Category , CategoryViewModel> _viewModelMapper;
        private readonly IBaseMapper<CategoryCreateViewModel, Category> _createMapper;
        private readonly IBaseMapper<CategoryUpdateViewModel, Category> _updateMapper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserContext _userContext;

        public CategoryService(
            IBaseMapper<Category, CategoryViewModel> viewMapper,
            IBaseMapper<CategoryCreateViewModel, Category> createMapper,
            IBaseMapper<CategoryUpdateViewModel, Category> updateMapper,
            ICategoryRepository categoryRepository,
            IUserContext userContext)
            : base(viewMapper, categoryRepository)
        {
            _createMapper = createMapper;
            _updateMapper = updateMapper;
            _viewModelMapper = viewMapper;
            _categoryRepository = categoryRepository;
            _userContext = userContext;
        }

        public async Task<CategoryViewModel> Create(CategoryCreateViewModel model, CancellationToken cancellationToken)
        {
            if (await _categoryRepository.IsCategoryNameExist(model.Name!, cancellationToken))
                throw new Exception("Category name already exists");

            var entity = _createMapper.MapModel(model);
            entity.EntryDate = DateTime.Now;
            entity.EntryBy = Convert.ToInt32(_userContext.UserId);
            var created = await _categoryRepository.Create(entity, cancellationToken);
            return _viewModelMapper.MapModel(created);
        }

        public async Task Update(CategoryUpdateViewModel model, CancellationToken cancellationToken)
        {
            var existing = await _categoryRepository.GetById(model.Id, cancellationToken);

            _updateMapper.MapModel(model, existing);
            existing.UpdatedDate = DateTime.Now;
            existing.UpdatedBy = Convert.ToInt32(_userContext.UserId);

            await _categoryRepository.Update(existing, cancellationToken);
        }

        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            var entity = await _categoryRepository.GetById(id, cancellationToken);
            await _categoryRepository.Delete(entity, cancellationToken);
        }

        public Task<bool> IsCategoryNameExist(string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
