using System.Linq.Expressions;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Project.API.Helpers;
using Project.Core.Common;
using Project.Core.Entities.Business;
using Project.Core.Interfaces.IServices;
using Project.Core.Services;


namespace Project.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryService _categoryService;
        private readonly IMemoryCache _memoryCache;
        public CategoryController(ILogger<CategoryController> logger, ICategoryService categoryService, IMemoryCache memoryCache)
        {
            _logger = logger;
            _categoryService = categoryService;
            _memoryCache = memoryCache;
        }
        [HttpGet("paginated-data")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize, string? search, string? sortBy, string? sortOrder, CancellationToken cancellationToken)
        {
            try
            {
                int pageSizeValue = pageSize ?? 10;
                int pageNumberValue = pageNumber ?? 1;
                sortBy = sortBy ?? "Id";
                sortOrder = sortOrder ?? "desc";

                var filters = new List<ExpressionFilter>();
                if (!string.IsNullOrWhiteSpace(search) && search != null)
                {
                    filters.AddRange(new[]
                    {
                        new ExpressionFilter
                        {
                            PropertyName = "Name",
                            Value = search,
                            Comparison = Comparison.Contains
                        },

                        new ExpressionFilter
                        {
                            PropertyName = "Description",
                            Value= search,
                            Comparison = Comparison.Contains
                        }
                    });

                }
            var category = await _categoryService.GetPaginatedData(pageNumberValue, pageSizeValue, filters, sortBy, sortOrder, cancellationToken);
            var response = new ResponseViewModel<PaginatedDataViewModel<CategoryViewModel>>
            {
                Success = true,
                Message = "Category retrived successfully",
                Data = category
            };
            return Ok(response);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex,"An error occurred while retrieving category");
                var errorResponse = new ResponseViewModel<IEnumerable<CategoryViewModel>>
                {
                    Success = false,
                    Message = "Error retrieving category",
                    Error = new ErrorViewModel
                    {
                        Code = "ERROR_CODE",
                        Message = ex.Message
                    }
                };
                        return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            try
            {
                var category = await _categoryService.GetAll(cancellationToken);
                var response = new ResponseViewModel<IEnumerable<CategoryViewModel>>
                {
                    Success = true,
                    Message = "Category retrived successfully",
                    Data = category
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving category");

                var errorResponse = new ResponseViewModel<IEnumerable<CategoryViewModel>>
                {
                    Success = false,
                    Message = "Error retrieving category",
                    Error = new ErrorViewModel
                    {
                        Code = "ERROR_CODE",
                        Message = ex.Message
                    }
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
        
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            try
            {
                var category = new CategoryViewModel();
                if(_memoryCache.TryGetValue($"Category_{id}", out CategoryViewModel cachedCategory))
                {
                    category = cachedCategory;
                }
                else
                {
                    category = await _categoryService.GetById(id, cancellationToken);
                    if(category == null)
                    {
                        _memoryCache.Set($"Category_{id}", category, TimeSpan.FromMinutes(10)); 
                    }
                }
                var response = new ResponseViewModel<CategoryViewModel>
                {
                    Success = true,
                    Message = "Category retrived sccessfully",
                    Data = category
                };
                return Ok(response);

            }
            catch (Exception ex)
            {
                if (ex.Message == "No data found")
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ResponseViewModel<CategoryViewModel>
                    {
                        Success = false,
                        Message = "Category not found",
                        Error = new ErrorViewModel
                        {
                            Code = "NOT_FOUND",
                            Message = "Category not found"
                        }
                    });
                }

                _logger.LogError(ex, $"An error occurred while retrieving the category");

                var errorResponse = new ResponseViewModel<CategoryViewModel>
                {
                    Success = false,
                    Message = "Error retrieving Category",
                    Error = new ErrorViewModel
                    {
                        Code = "ERROR_CODE",
                        Message = ex.Message
                    }
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                string message = "";
                if (await _categoryService.IsExists("Name", model.Name, cancellationToken))
                {
                    message = $"The category name- '{model.Name}' already exists";
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseViewModel<CategoryViewModel>
                    {
                        Success = false,
                        Message = message,
                        Error = new ErrorViewModel
                        {
                            Code = "DUPLICATE_NAME",
                            Message = message
                        }
                    });
                }

                try
                {
                    var data = await _categoryService.Create(model, cancellationToken);

                    var response = new ResponseViewModel<CategoryViewModel>
                    {
                        Success = true,
                        Message = "Category created successfully",
                        Data = data
                    };

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the category");
                    message = $"An error occurred while adding the category- " + ex.Message;

                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel<CategoryViewModel>
                    {
                        Success = false,
                        Message = message,
                        Error = new ErrorViewModel
                        {
                            Code = "ADD_ROLE_ERROR",
                            Message = message
                        }
                    });
                }
            }

            return StatusCode(StatusCodes.Status400BadRequest, new ResponseViewModel<CategoryViewModel>
            {
                Success = false,
                Message = "Invalid input",
                Error = new ErrorViewModel
                {
                    Code = "INPUT_VALIDATION_ERROR",
                    Message = ModelStateHelper.GetErrors(ModelState)
                }
            });
        }

        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> Edit(CategoryUpdateViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                string message = "";
                if (await _categoryService.IsExistsForUpdate(model.Id, "Name", model.Name, cancellationToken))
                {
                    message = $"The category name- '{model.Name}' already exists";
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseViewModel
                    {
                        Success = false,
                        Message = message,
                        Error = new ErrorViewModel
                        {
                            Code = "DUPLICATE_NAME",
                            Message = message
                        }
                    });
                }


                try
                {
                    await _categoryService.Update(model, cancellationToken);

                    // Remove data from cache by key
                    _memoryCache.Remove($"Category_{model.Id}");

                    var response = new ResponseViewModel
                    {
                        Success = true,
                        Message = "Category updated successfully"
                    };

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the category");
                    message = $"An error occurred while updating the category- " + ex.Message;

                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel
                    {
                        Success = false,
                        Message = message,
                        Error = new ErrorViewModel
                        {
                            Code = "UPDATE_ROLE_ERROR",
                            Message = message
                        }
                    });
                }
            }

            return StatusCode(StatusCodes.Status400BadRequest, new ResponseViewModel
            {
                Success = false,
                Message = "Invalid input",
                Error = new ErrorViewModel
                {
                    Code = "INPUT_VALIDATION_ERROR",
                    Message = ModelStateHelper.GetErrors(ModelState)
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _categoryService.Delete(id, cancellationToken);

                // Remove data from cache by key
                _memoryCache.Remove($"Category_{id}");

                var response = new ResponseViewModel
                {
                    Success = true,
                    Message = "Category deleted successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex.Message == "No data found")
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ResponseViewModel
                    {
                        Success = false,
                        Message = "Product not found",
                        Error = new ErrorViewModel
                        {
                            Code = "NOT_FOUND",
                            Message = "Category not found"
                        }
                    });
                }

                _logger.LogError(ex, "An error occurred while deleting the category");

                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel
                {
                    Success = false,
                    Message = "Error deleting the category",
                    Error = new ErrorViewModel
                    {
                        Code = "DELETE_ROLE_ERROR",
                        Message = ex.Message
                    }
                });

            }
        }
    }

}
