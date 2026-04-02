using Microsoft.Extensions.DependencyInjection;

namespace LearnCsla.BusinessLibrary
{
  public static class MultitenancyServiceCollectionExtensions
  {
    public static IServiceCollection AddMultitenancy<TTenant, TResolver>(this IServiceCollection services)
        where TResolver : class, ITenantResolver<TTenant>
        where TTenant : class, ITenantInfo
    {
      services.AddScoped<ITenantResolver<TTenant>, TResolver>();
      services.AddSingleton<IDynamicRuleManager, DynamicRuleManager>();
      return services;
    }
  }
}
