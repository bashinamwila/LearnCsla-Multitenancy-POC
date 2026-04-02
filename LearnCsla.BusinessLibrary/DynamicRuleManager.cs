using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using Csla.Rules;

namespace LearnCsla.BusinessLibrary
{
  [MetadataAttribute]
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class ExportRuleAttribute : Attribute, IRuleMetadata
  {
    public string RuleSet { get; }
    public Type TargetType { get; }

    public ExportRuleAttribute(Type targetType, string ruleSet)
    {
      TargetType = targetType;
      RuleSet = ruleSet;
    }
  }

  public class DynamicRuleManager : IDynamicRuleManager
  {
    private readonly AggregateCatalog _catalog;

    public DynamicRuleManager()
    {
      _catalog = new AggregateCatalog();
      _catalog.Catalogs.Add(new AssemblyCatalog(typeof(DynamicRuleManager).Assembly));
    }

    public void LoadAllTenantRules(Type targetType, BusinessRules rules)
    {
      foreach (var part in _catalog.Parts)
      {
        var metadata = part.ExportDefinitions
            .Select(d => d.Metadata)
            .FirstOrDefault(m => m.ContainsKey("RuleSet") && m.ContainsKey("TargetType"));

        if (metadata != null && (Type)metadata["TargetType"]! == targetType)
        {
          var ruleSet = (string)metadata["RuleSet"]!;
          
          // Get the Type without instantiating
          var ruleType = ReflectionModelServices.GetPartType(part).Value;

          IBusinessRule rule;
          if (targetType == typeof(Organisation))
          {
            rule = (IBusinessRule)Activator.CreateInstance(ruleType, Organisation.NameProperty)!;
          }
          else
          {
            continue;
          }

          rules.AddRule(rule, ruleSet);
        }
      }
    }
  }
}
