using System.Collections.Generic;
using System.Threading.Tasks;

namespace LearnCsla.Dal
{
  public class OrganisationDto
  {
    public string OrganisationId { get; set; } = string.Empty;
    public string OrganisationName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
  }

  public interface IOrganisationDal
  {
    Task<OrganisationDto> FetchAsync(string id);
    Task<List<OrganisationDto>> FetchAllAsync();
    Task InsertAsync(OrganisationDto dto);
    Task UpdateAsync(OrganisationDto dto);
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
  }
}
