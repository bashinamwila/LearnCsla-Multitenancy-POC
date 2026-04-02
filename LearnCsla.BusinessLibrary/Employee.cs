using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Collections.Generic;
using Csla;
using LearnCsla.Dal;

namespace LearnCsla.BusinessLibrary
{
  [Serializable]
  [CslaImplementProperties]
  public partial class Employee : BusinessBase<Employee>
  {
    public partial string EmployeeId { get; set; }
    public partial string FirstName { get; set; }
    public partial string LastName { get; set; }
    public partial string FullName { get; private set; }

    // Debugging helper
    public string[] GetRuleInfo() => BusinessRules.GetRuleDescriptions();
    public string GetActiveRuleSet() => BusinessRules.RuleSet;

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      
      var ruleManager = ApplicationContext.GetRequiredService<IDynamicRuleManager>();
      ruleManager.LoadAllTenantRulesAsync(typeof(Employee), BusinessRules).GetAwaiter().GetResult();

      BusinessRules.AddRule(new Csla.Rules.CommonRules.Dependency(LastNameProperty, FirstNameProperty));
    }

    [Create]
    private void Create([Inject] ITenantResolver<TenantInfo> resolver)
    {
      BusinessRules.RuleSet = resolver.TenantId;
    }

    [Fetch]
    private async Task Fetch(string id, [Inject] IOrganisationDal dal, [Inject] ITenantResolver<TenantInfo> resolver)
    {
      BusinessRules.RuleSet = resolver.TenantId;
      BusinessRules.CheckRules();
    }
  }
}
