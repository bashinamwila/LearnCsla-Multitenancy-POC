using System;
using System.Threading.Tasks;
using Csla;
using LearnCsla.Dal;

namespace LearnCsla.BusinessLibrary
{
  [CslaImplementProperties]
  public partial class TenantInfo : ReadOnlyBase<TenantInfo>, ITenantInfo
  {
   
    public partial string Id { get; private set; }

    
    public  partial string Name { get; private set; }

   [Fetch]
    private async Task FetchAsync(string id, [Inject] IOrganisationDal dal)
    {
      var dto = await dal.FetchAsync(id);
      LoadProperty(IdProperty, dto.OrganisationId);
      LoadProperty(NameProperty, dto.OrganisationName);
    }
  }
}
