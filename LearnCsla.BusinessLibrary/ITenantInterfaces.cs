using System;
using System.ComponentModel.Composition;
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
    Task LoadAllTenantRulesAsync(Type targetType, Csla.Rules.BusinessRules rules);
  }

  public interface IRuleMetadata
  {
    Type TargetType { get; }
    string ConditionField { get; }
    string ConditionValue { get; }
    string PrimaryPropertyName { get; }
    string[] InputPropertyNames { get; }
    string[] AffectedPropertyNames { get; }
  }

  [MetadataAttribute]
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class ExportTenantRuleAttribute : ExportAttribute, IRuleMetadata
  {
    public Type TargetType { get; }
    public string ConditionField { get; }
    public string ConditionValue { get; }
    public string PrimaryPropertyName { get; }
    public string[] InputPropertyNames { get; }
    public string[] AffectedPropertyNames { get; }

    public ExportTenantRuleAttribute(
        Type targetType, 
        string conditionField, 
        string conditionValue, 
        string primaryPropertyName,
        string[]? inputPropertyNames = null,
        string[]? affectedPropertyNames = null)
      : base(typeof(Csla.Rules.IBusinessRule))
    {
      TargetType = targetType;
      ConditionField = conditionField;
      ConditionValue = conditionValue;
      PrimaryPropertyName = primaryPropertyName;
      InputPropertyNames = inputPropertyNames ?? Array.Empty<string>();
      AffectedPropertyNames = affectedPropertyNames ?? Array.Empty<string>();
    }
  }
}
