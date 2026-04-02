using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Csla.Core;
using Csla.Rules;
using LearnCsla.Dal;

namespace LearnCsla.BusinessLibrary
{
  public class DynamicRuleManager : IDynamicRuleManager
  {
    private readonly AggregateCatalog _catalog;
    private readonly IOrganisationDal _dal;

    public DynamicRuleManager(IOrganisationDal dal)
    {
      _dal = dal;
      _catalog = new AggregateCatalog();
      _catalog.Catalogs.Add(new AssemblyCatalog(typeof(DynamicRuleManager).Assembly));
    }

    public async Task LoadAllTenantRulesAsync(Type targetType, BusinessRules rules)
    {
      var tenants = await _dal.FetchAllAsync();
      if (tenants == null) return;

      foreach (var part in _catalog.Parts)
      {
        var metadata = part.ExportDefinitions
            .Select(d => d.Metadata)
            .FirstOrDefault(m => m.ContainsKey("TargetType") && m.ContainsKey("ConditionField"));

        if (metadata != null && (Type)metadata["TargetType"] == targetType)
        {
          var conditionField = (string)metadata["ConditionField"];
          var conditionValue = (string)metadata["ConditionValue"];
          var primaryPropName = (string)metadata["PrimaryPropertyName"];
          var inputPropNames = (string[])metadata["InputPropertyNames"];
          var affectedPropNames = (string[])metadata["AffectedPropertyNames"];

          foreach (var tenant in tenants)
          {
            if (tenant == null || string.IsNullOrEmpty(tenant.OrganisationId)) continue;

            var propInfo = tenant.GetType().GetProperty(conditionField);
            var tenantValue = propInfo?.GetValue(tenant)?.ToString();

            if (tenantValue == conditionValue)
            {
              var ruleTypeLazy = ReflectionModelServices.GetPartType(part);
              var ruleType = ruleTypeLazy.Value;

              var primaryProp = ResolveProperty(targetType, primaryPropName);
              var inputProps = inputPropNames.Select(n => ResolveProperty(targetType, n)).ToArray();
              var affectedProps = affectedPropNames.Select(n => ResolveProperty(targetType, n)).ToArray();

              IBusinessRule rule;
              if (inputProps.Length == 0 && affectedProps.Length == 0)
              {
                rule = (IBusinessRule)Activator.CreateInstance(ruleType, primaryProp)!;
              }
              else
              {
                var args = new List<object> { primaryProp };
                args.AddRange(inputProps);
                args.AddRange(affectedProps);
                rule = (IBusinessRule)Activator.CreateInstance(ruleType, args.ToArray())!;
              }

              rules.AddRule(rule, tenant.OrganisationId);

              foreach (var inputProp in inputProps)
              {
                var depRule = new Csla.Rules.CommonRules.Dependency(primaryProp,inputProp);
                rules.AddRule(depRule, tenant.OrganisationId);
              }
            }
          }
        }
      }
    }

    private IPropertyInfo ResolveProperty(Type type, string name)
    {
      var field = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
      if (field == null) throw new InvalidOperationException($"Property {name} not found on {type.Name}");
      return (IPropertyInfo)field.GetValue(null)!;
    }
  }
}
