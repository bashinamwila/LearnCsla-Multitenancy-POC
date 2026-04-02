using System.Security.Claims;
using System.Threading.Tasks;
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
      
      mockDal.Setup(d => d.FetchAsync(It.IsAny<string>()))
             .ReturnsAsync(new OrganisationDto { OrganisationId = "1", OrganisationName = "test", Country = "USA" });
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
      
      // TenantA rule requires Upper Case. 
      // Setting name should trigger validation.
      org.Name = "lower";
      
      Assert.False(org.IsValid);
      Assert.Contains(org.BrokenRulesCollection, r => r.Description.Contains("TenantA"));
    }

    [Fact]
    public async Task Organisation_Uses_TenantB_Rules()
    {
      var provider = CreateProvider("TenantB");
      var portal = provider.GetRequiredService<IDataPortal<Organisation>>();

      var org = await portal.CreateAsync();
      
      // TenantB rule requires Lower Case
      org.Name = "UPPER";
      
      Assert.False(org.IsValid);
      Assert.Contains(org.BrokenRulesCollection, r => r.Description.Contains("TenantB"));
    }
  }
}
