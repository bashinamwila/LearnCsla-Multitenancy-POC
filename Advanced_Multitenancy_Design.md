# Advanced Multitenancy POC - Design Document

This document outlines the strategy for evolving the Multitenancy POC into a data-driven system where business rules are partitioned based on tenant configurations (e.g., Country) fetched from a data source.

## 1. Enhanced Metadata Discovery
We will replace the simple `ExportRule` attribute with a more descriptive `ExportTenantRuleAttribute`. This attribute describes the **Conditions** and **Parameters** required to instantiate the rule.

**The Metadata Schema:**
- `TargetType`: The Type of the Business Object (e.g., `typeof(Organisation)`).
- `ConditionField`: The field in the Tenant storage to check (e.g., `"Country"`).
- `ConditionValue`: The value that triggers this rule (e.g., `"CountryA"`).
- `PrimaryPropertyName`: String name of the primary property field (e.g., `"NameProperty"`).
- `InputPropertyNames`: Array of secondary property field names.
- `AffectedPropertyNames`: Array of affected property field names.

## 2. Data-Driven Discovery Engine (`DynamicRuleManager`)
The logic will shift from hardcoded metadata to matching rules against tenant configurations fetched at runtime.

**The Workflow:**
1.  **Tenant Fetching**: The manager calls `IOrganisationDal.FetchAllAsync()` to get all active tenants and their configurations.
2.  **MEF Scanning**: It scans all `IBusinessRule` exports in the assembly.
3.  **Condition Matching**: For every tenant, it evaluates the MEF rules. If a rule's `ConditionField` matches the tenant's value, the rule is selected.
4.  **Property Resolution**: The manager uses reflection on the `TargetType` to find the static `PropertyInfo<T>` fields matching the names in the attribute.
5.  **Cache Population**: Rules are registered in the global CSLA cache partitioned by the `TenantId` (RuleSet).

## 3. Multi-Parameter Rules
Rules with multiple dependencies (like name formatting) will follow the standard CSLA pattern:
- **PrimaryProperty**: The property that triggers the rule.
- **InputProperties**: Additional properties needed for the calculation.
- **AffectedProperties**: The property whose value is set by the rule.

## 4. Business Objects
- **Employee**: A new editable object (`EmployeeId`, `FirstName`, `LastName`, `FullName`) using CSLA 10 Source Generators.
- **RuleSet Management**: Instances will resolve their `TenantId` and set `BusinessRules.RuleSet` to ensure the correct partition is used.

## 5. Planned Rules
| Rule | Condition | Target Property | Dependencies |
| :--- | :--- | :--- | :--- |
| `NameShouldStartWithUpperCase` | `Country == "CountryA"` | `Organisation.Name` | - |
| `NameShouldStartWithLowerCase` | `Country == "CountryB"` | `Organisation.Name` | - |
| `FirstNameLastNameFullNameFormat` | `Country == "CountryA"` | `Employee.FirstName` | `LastName`, `FullName` |
| `LastNameCommaFirstNameFullNameFormat` | `Country == "CountryB"` | `Employee.FirstName` | `LastName`, `FullName` |
