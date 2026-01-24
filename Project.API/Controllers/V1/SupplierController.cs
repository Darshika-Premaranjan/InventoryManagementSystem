using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Project.API.Helpers;
using Project.Core.Common;
using Project.Core.Entities.Business;
using Project.Core.Interfaces.IServices;

namespace Project.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SupplierController : ControllerBase
    {
        private readonly ILogger<SupplierController> _logger;
        private readonly ISupplierService _supplierService;
        private readonly IMemoryCache _memoryCache;

        public SupplierController(ILogger<SupplierController> logger, ISupplierService supplierService, IMemoryCache memoryCache)
        {
            _logger = logger;
            _supplierService = supplierService;
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
                    // Add filters for relevant properties
                    filters.AddRange(new[]
                    {
                        new ExpressionFilter
                        {
                            PropertyName = "Name",
                            Value = search,
                            Comparison = Comparison.Contains
                        }
                    });
                }

                var suppliers = await _supplierService.GetPaginatedData(pageNumberValue, pageSizeValue, filters, sortBy, sortOrder, cancellationToken);

                var response = new ResponseViewModel<PaginatedDataViewModel<SupplierViewModel>>
                {
                    Success = true,
                    Message = "Supplier retrieved successfully",
                    Data = suppliers
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving suppliers");

                var errorResponse = new ResponseViewModel<IEnumerable<SupplierViewModel>>
                {
                    Success = false,
                    Message = "Error retrieving suppliers",
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
                var suppliers = await _supplierService.GetAll(cancellationToken);

                var response = new ResponseViewModel<IEnumerable<SupplierViewModel>>
                {
                    Success = true,
                    Message = "Suppliers retrieved successfully",
                    Data = suppliers
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving suppliers");

                var errorResponse = new ResponseViewModel<IEnumerable<SupplierViewModel>>
                {
                    Success = false,
                    Message = "Error retrieving suppliers",
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
                var suppliers = new SupplierViewModel();

                // Attempt to retrieve the suppliers from the cache
                if (_memoryCache.TryGetValue($"Supplier_{id}", out SupplierViewModel cachedSupplier))
                {
                    suppliers = cachedSupplier;
                }
                else
                {
                    // If not found in cache, fetch the suppliers  from the data source
                    suppliers = await _supplierService.GetById(id, cancellationToken);

                    if (suppliers != null)
                    {
                        // Cache the suppliers  with an expiration time of 10 minutes
                        _memoryCache.Set($"Supplier_{id}", suppliers, TimeSpan.FromMinutes(10));
                    }
                }

                var response = new ResponseViewModel<SupplierViewModel>
                {
                    Success = true,
                    Message = "Supplier retrieved successfully",
                    Data = suppliers
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex.Message == "No data found")
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ResponseViewModel<SupplierViewModel>
                    {
                        Success = false,
                        Message = "Supplier not found",
                        Error = new ErrorViewModel
                        {
                            Code = "NOT_FOUND",
                            Message = "Supplier not found"
                        }
                    });
                }

                _logger.LogError(ex, $"An error occurred while retrieving the Supplier");

                var errorResponse = new ResponseViewModel<SupplierViewModel>
                {
                    Success = false,
                    Message = "Error retrieving Supplier",
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
        public async Task<IActionResult> Create(SupplierCreateViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                string message = "";
                if (await _supplierService.IsExists("Name", model.Name, cancellationToken))
                {
                    message = $"The supplier name- '{model.Name}' already exists";
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseViewModel<SupplierViewModel>
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
                    var data = await _supplierService.Create(model, cancellationToken);

                    var response = new ResponseViewModel<SupplierViewModel>
                    {
                        Success = true,
                        Message = "Supplier created successfully",
                        Data = data
                    };

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the supplier");
                    message = $"An error occurred while adding the supplier- " + ex.Message;

                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel<SupplierViewModel>
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

            return StatusCode(StatusCodes.Status400BadRequest, new ResponseViewModel<SupplierViewModel>
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
        public async Task<IActionResult> Edit(SupplierUpdateViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                string message = "";
                if (await _supplierService.IsExistsForUpdate(model.Id, "Name", model.Name, cancellationToken))
                {
                    message = $"The supplier name- '{model.Name}' already exists";
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
                    await _supplierService.Update(model, cancellationToken);

                    // Remove data from cache by key
                    _memoryCache.Remove($"Supplier_{model.Id}");

                    var response = new ResponseViewModel
                    {
                        Success = true,
                        Message = "Supplier updated successfully"
                    };

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the supplier");
                    message = $"An error occurred while updating the supplier- " + ex.Message;

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
                await _supplierService.Delete(id, cancellationToken);

                // Remove data from cache by key
                _memoryCache.Remove($"Supplier_{id}");

                var response = new ResponseViewModel
                {
                    Success = true,
                    Message = "Supplier deleted successfully"
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
                        Message = "Supplier not found",
                        Error = new ErrorViewModel
                        {
                            Code = "NOT_FOUND",
                            Message = "Supplier not found"
                        }
                    });
                }

                _logger.LogError(ex, "An error occurred while deleting the Supplier");

                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel
                {
                    Success = false,
                    Message = "Error deleting the Supplier",
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
