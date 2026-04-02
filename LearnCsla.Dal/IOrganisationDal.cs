using System.Collections.Generic;
using System.Threading.Tasks;

namespace LearnCsla.Dal
{
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
