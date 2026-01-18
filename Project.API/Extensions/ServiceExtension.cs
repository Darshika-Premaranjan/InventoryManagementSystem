using Project.API.Middlewares;
using Project.Core.Interfaces.IRepositories;
using Project.Core.Interfaces.IServices;
using Project.Core.Services;
using Project.Infrastructure.Repositories;

namespace Project.API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection RegisterService(this IServiceCollection services)
        {
            #region Services
            services.AddSingleton<IUserContext, UserContext>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();


            #endregion

            #region Repositories
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IAuthRepository, AuthRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();


            #endregion

            return services;
        }
    }
}
