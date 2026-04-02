using Csla.Rules;
using Csla.Core;
using System.ComponentModel.Composition;

namespace LearnCsla.BusinessLibrary
{
  [Export(typeof(IBusinessRule))]
  [ExportRule(typeof(Organisation), "TenantA")]
  public class NameShouldStartWithUpperCase : BusinessRule
  {
    public NameShouldStartWithUpperCase(IPropertyInfo primaryProperty) : base(primaryProperty)
    {
      if (primaryProperty != null)
        InputProperties.Add(primaryProperty);
    }

    protected override void Execute(IRuleContext context)
    {
      var value = context.GetInputValue<string>(PrimaryProperty!);
      if (!string.IsNullOrEmpty(value) && !char.IsUpper(value[0]))
        context.AddErrorResult("TenantA: Name must start with an upper case letter.");
    }
  }

  [Export(typeof(IBusinessRule))]
  [ExportRule(typeof(Organisation), "TenantB")]
  public class NameShouldStartWithLowerCase : BusinessRule
  {
    public NameShouldStartWithLowerCase(IPropertyInfo primaryProperty) : base(primaryProperty)
    {
      if (primaryProperty != null)
        InputProperties.Add(primaryProperty);
    }

    protected override void Execute(IRuleContext context)
    {
      var value = context.GetInputValue<string>(PrimaryProperty!);
      if (!string.IsNullOrEmpty(value) && !char.IsLower(value[0]))
        context.AddErrorResult("TenantB: Name must start with a lower case letter.");
    }
  }
}
