using Csla;

namespace LearnCsla.BusinessLibrary
{
  public interface ITenantInfo : IReadOnlyBase
  {
    string Id { get; }
    string Name { get; }
  }
}
