using System.Threading.Tasks;

namespace LearnCsla.BusinessLibrary
{
  public interface ITenantResolver<TTenant> where TTenant : class, ITenantInfo
  {
    string TenantId { get; }
    Task<TTenant> GetTenantAsync();
  }

  public interface IDynamicRuleManager
  {
    void LoadAllTenantRules(Type targetType, Csla.Rules.BusinessRules rules);
  }

  public interface IRuleMetadata
  {
    string RuleSet { get; }
    Type TargetType { get; }
  }
}
