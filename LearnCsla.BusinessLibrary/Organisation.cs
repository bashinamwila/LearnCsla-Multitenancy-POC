using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Csla;
using LearnCsla.Dal;

namespace LearnCsla.BusinessLibrary
{
  [CslaImplementProperties]
  public partial class Organisation : BusinessBase<Organisation>
  {
    
    public partial string Id { get; private set; }


  
    [Display(Name = "Organisation Name")]
    public partial string Name { get; set; }
    
  
    public  partial string Country { get; set; }
    

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      
      var ruleManager = ApplicationContext.GetRequiredService<IDynamicRuleManager>();
      ruleManager.LoadAllTenantRules(typeof(Organisation), BusinessRules);

      BusinessRules.RuleSet = ApplicationContext.DefaultRuleSet;
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(NameProperty, "Organisation Name is required"));
    }

    [Create]
    private async Task Create([Inject] ITenantResolver<TenantInfo> resolver, [Inject] IOrganisationDal dal)
    {
      BusinessRules.RuleSet = resolver.TenantId;
      
      var exists = await dal.ExistsAsync("TenantA");
      Id = exists ? "TenantB" : "TenantA";
      
      BusinessRules.CheckRules();
    }

    [Fetch]
    private async Task Fetch(string id, [Inject] IOrganisationDal dal, [Inject] ITenantResolver<TenantInfo> resolver)
    {
      BusinessRules.RuleSet = resolver.TenantId;
      
      var data = await dal.FetchAsync(id);
      using (BypassPropertyChecks)
      {
        Id = data.OrganisationId;
        Name = data.OrganisationName;
        Country = data.Country;
      }
      BusinessRules.CheckRules();
    }

    [Update]
    private async Task Update([Inject] IOrganisationDal dal)
    {
      using (BypassPropertyChecks)
      {
        var dto = new OrganisationDto 
        { 
          OrganisationId = Id, 
          OrganisationName = Name, 
          Country = Country 
        };
        await dal.UpdateAsync(dto);
      }
    }
  }
}
