using System.Security.Claims;
using System.Threading.Tasks;
using Csla;

namespace LearnCsla.BusinessLibrary
{
  public class TenantResolver : ITenantResolver<TenantInfo>
  {
    private readonly ApplicationContext _context;
    private readonly IDataPortal<TenantInfo> _portal;

    public TenantResolver(ApplicationContext context, IDataPortal<TenantInfo> portal)
    {
      _context = context;
      _portal = portal;
    }

    public string TenantId
    {
      get
      {
        var user = _context.User as ClaimsPrincipal;
        return user?.FindFirst("TenantId")?.Value ?? ApplicationContext.DefaultRuleSet;
      }
    }

    public async Task<TenantInfo> GetTenantAsync()
    {
      return await _portal.FetchAsync(TenantId);
    }
  }
}
