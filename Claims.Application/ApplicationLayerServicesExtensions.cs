using Claims.Application.Interfaces.Services;
using Claims.Application.Services;
using Claims.Application.Validation;
using Claims.Domain.Entities;
using Claims.Domain.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.Application
{
    public static class ApplicationLayerServicesExtensions
    {
        public static IServiceCollection AddApplicationLayerServices(this IServiceCollection services)
        {
            // Register AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddTransient<IClaimService, ClaimService>();
            services.AddTransient<ICoverService, CoverService>();
            services.AddTransient<IValidator<Claim>, EnhancedClaimValidator>();
            services.AddTransient<CoverValidator>();

            return services;
        }
    }
}
