using System;
using Microsoft.EntityFrameworkCore;
using Tgs.Payment.UmPay;
using Tgs.Payment.UmPay.Repositories;
using Tgs.Payment.UmPay.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static IServiceCollection AddUmPayRepositories(
            this IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> configure,
            int poolSize = 128)
        {
            return services
            .AddDbContext<TgsContext>(configure)
            .AddTransient<ITgsRepository, TgsRepository>();
        }
    }
}