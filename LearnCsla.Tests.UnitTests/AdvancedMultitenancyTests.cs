using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Csla;
using Csla.Configuration;
using LearnCsla.BusinessLibrary;
using LearnCsla.Dal;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace LearnCsla.Tests.UnitTests
{
  public class AdvancedMultitenancyTests
  {
    private IServiceProvider CreateProvider(string tenantId)
    {
      var services = new ServiceCollection();
      var mockDal = new Mock<IOrganisationDal>();
      
      var tenantA = new OrganisationDto { OrganisationId = "TenantA", OrganisationName = "Org A", Country = "CountryA" };
      var tenantB = new OrganisationDto { OrganisationId = "TenantB", OrganisationName = "Org B", Country = "CountryB" };

      mockDal.Setup(d => d.FetchAsync("TenantA")).ReturnsAsync(tenantA);
      mockDal.Setup(d => d.FetchAsync("TenantB")).ReturnsAsync(tenantB);
      mockDal.Setup(d => d.FetchAllAsync()).ReturnsAsync(new List<OrganisationDto> { tenantA, tenantB });
      mockDal.Setup(d => d.ExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

      services.AddSingleton(mockDal.Object);
      services.AddMultitenancy<TenantInfo, TenantResolver>();
      services.AddCsla();
      
      var provider = services.BuildServiceProvider();
      
      var applicationContext = provider.GetRequiredService<ApplicationContext>();
      var identity = new ClaimsIdentity(new[] { new Claim("TenantId", tenantId) }, "Test");
      applicationContext.User = new ClaimsPrincipal(identity);
      applicationContext.RuleSet = tenantId;
      
      return provider;
    }

    [Fact]
    public async Task Employee_Uses_FirstLast_Format_For_CountryA()
    {
      var provider = CreateProvider("TenantA");
      var portal = provider.GetRequiredService<IDataPortal<Employee>>();

      var emp = await portal.CreateAsync();
      
     // Assert.Equal("TenantA", emp.GetActiveRuleSet());
     // var rules = emp.GetRuleInfo();
     // Assert.Contains(rules, r => r.Contains("FirstNameLastNameFullNameFormat"));

      emp.FirstName = "John";
      emp.LastName = "Doe";
      
      Assert.Equal("John Doe", emp.FullName);
    }

    [Fact]
    public async Task Employee_Uses_LastCommaFirst_Format_For_CountryB()
    {
      var provider = CreateProvider("TenantB");
      var portal = provider.GetRequiredService<IDataPortal<Employee>>();

      var emp = await portal.CreateAsync();
      
     // Assert.Equal("TenantB", emp.GetActiveRuleSet());
     // var rules = emp.GetRuleInfo();
    //  Assert.Contains(rules, r => r.Contains("LastNameCommaFirstNameFullNameFormat"));

      emp.FirstName = "John";
      emp.LastName = "Doe";
      
      Assert.Equal("Doe, John", emp.FullName);
    }

    [Fact]
    public async Task Organisation_Uses_UpperCase_For_CountryA()
    {
      var provider = CreateProvider("TenantA");
      var portal = provider.GetRequiredService<IDataPortal<Organisation>>();

      var org = await portal.CreateAsync();
     // Assert.Equal("TenantA", org.GetActiveRuleSet());
      
      org.Name = "lower";
      
      Assert.False(org.IsValid);
      Assert.Contains(org.BrokenRulesCollection, r => r.Description.Contains("CountryA"));
    }
  }
}
