using Csla.Rules;
using Csla.Core;
using System.ComponentModel.Composition;

namespace LearnCsla.BusinessLibrary
{
  // --- Organisation Rules ---

  [ExportTenantRule(typeof(Organisation), "Country", "CountryA", nameof(Organisation.NameProperty))]
  public class NameShouldStartWithUpperCase : BusinessRule
  {
    public NameShouldStartWithUpperCase(IPropertyInfo primaryProperty) : base(primaryProperty)
    {
      // PrimaryProperty must be in InputProperties to use GetInputValue
      InputProperties.Add(primaryProperty);
    }

    protected override void Execute(IRuleContext context)
    {
      var value = context.GetInputValue<string>(InputProperties[0]);
      if (!string.IsNullOrEmpty(value) && !char.IsUpper(value[0]))
        context.AddErrorResult("CountryA: Name must start with an upper case letter.");
    }
  }

  [ExportTenantRule(typeof(Organisation), "Country", "CountryB", nameof(Organisation.NameProperty))]
  public class NameShouldStartWithLowerCase : BusinessRule
  {
    public NameShouldStartWithLowerCase(IPropertyInfo primaryProperty) : base(primaryProperty)
    {
      InputProperties.Add(primaryProperty);
    }

    protected override void Execute(IRuleContext context)
    {
      var value = context.GetInputValue<string>(InputProperties[0]);
      if (!string.IsNullOrEmpty(value) && !char.IsLower(value[0]))
        context.AddErrorResult("CountryB: Name must start with a lower case letter.");
    }
  }

  // --- Employee Rules ---

  [ExportTenantRule(typeof(Employee), "Country", "CountryA", 
      nameof(Employee.FirstNameProperty), 
      new[] { nameof(Employee.LastNameProperty) }, 
      new[] { nameof(Employee.FullNameProperty) })]
  public class FirstNameLastNameFullNameFormat : BusinessRule
  {
    public FirstNameLastNameFullNameFormat(IPropertyInfo primaryProperty, IPropertyInfo lastNameProperty, IPropertyInfo affectedProperty) 
      : base(primaryProperty) 
    {
      // Order: 0:Primary, 1:LastName, 2:FullName
      InputProperties.AddRange(new[] { primaryProperty, lastNameProperty, affectedProperty });
      // base adds primary to index 0, so affectedProperty goes to index 1
      AffectedProperties.Add(affectedProperty);
    }

    protected override void Execute(IRuleContext context)
    {
      var firstName = context.GetInputValue<string>(InputProperties[0]);
      var lastName = context.GetInputValue<string>(InputProperties[1]);
      
      if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
      {
        // Index 1 is FullName
        context.AddOutValue(AffectedProperties[1], $"{firstName} {lastName}");
      }
    }
  }

  [ExportTenantRule(typeof(Employee), "Country", "CountryB", 
      nameof(Employee.FirstNameProperty), 
      new[] { nameof(Employee.LastNameProperty) }, 
      new[] { nameof(Employee.FullNameProperty) })]
  public class LastNameCommaFirstNameFullNameFormat : BusinessRule
  {
    public LastNameCommaFirstNameFullNameFormat(IPropertyInfo primaryProperty, IPropertyInfo lastNameProperty, IPropertyInfo affectedProperty) 
      : base(primaryProperty) 
    {
      InputProperties.AddRange(new[] { primaryProperty, lastNameProperty, affectedProperty });
      AffectedProperties.Add(affectedProperty);
    }

    protected override void Execute(IRuleContext context)
    {
      var firstName = context.GetInputValue<string>(InputProperties[0]);
      var lastName = context.GetInputValue<string>(InputProperties[1]);
      
      if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
      {
        context.AddOutValue(AffectedProperties[1], $"{lastName}, {firstName}");
      }
    }
  }
}
