using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Csla;
using Csla.Configuration;
using LearnCsla.BusinessLibrary;
using LearnCsla.Dal;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace LearnCsla.Tests.UnitTests
{
  public class MultitenancyPocTests
  {
    private IServiceProvider CreateProvider(string tenantId)
    {
      var services = new ServiceCollection();
      var mockDal = new Mock<IOrganisationDal>();
      
      var tenantDto = new OrganisationDto { OrganisationId = tenantId, OrganisationName = "test", Country = "CountryA" };

      mockDal.Setup(d => d.FetchAsync(It.IsAny<string>())).ReturnsAsync(tenantDto);
      mockDal.Setup(d => d.FetchAllAsync()).ReturnsAsync(new List<OrganisationDto> { tenantDto });
      mockDal.Setup(d => d.ExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

      services.AddSingleton(mockDal.Object);
      services.AddMultitenancy<TenantInfo, TenantResolver>();
      services.AddCsla();
      
      var provider = services.BuildServiceProvider();
      
      // Set the global User context for the application context
      var applicationContext = provider.GetRequiredService<ApplicationContext>();
      var identity = new ClaimsIdentity(new[] { new Claim("TenantId", tenantId) }, "Test");
      applicationContext.User = new ClaimsPrincipal(identity);
      
      // IMPORTANT: Set RuleSet globally too to ensure AddBusinessRules triggers for this partition
      applicationContext.RuleSet = tenantId;
      
      return provider;
    }

    [Fact]
    public async Task Organisation_Uses_TenantA_Rules()
    {
      var provider = CreateProvider("TenantA");
      var portal = provider.GetRequiredService<IDataPortal<Organisation>>();

      var org = await portal.CreateAsync();
      
      // TenantA rule requires Upper Case
      org.Name = "lower";
      
      Assert.False(org.IsValid);
      Assert.Contains(org.BrokenRulesCollection, r => r.Description.Contains("CountryA"));
    }

    [Fact]
    public async Task Organisation_Uses_TenantB_Rules()
    {
      var provider = CreateProvider("TenantB");
      var portal = provider.GetRequiredService<IDataPortal<Organisation>>();

      var org = await portal.CreateAsync();
      
      // We didn't setup a rule for TenantB in CountryA config in this test, 
      // but let's assume it has no rules or default ones.
      Assert.True(org.IsValid);
    }
  }
}
