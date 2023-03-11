using System.Collections.Generic;
using System.Threading.Tasks;
using chub.Models;
using chub.Responses;

namespace chub.Services;

public interface ISearchService
{
    Task<ListResponse<Command>> Search(string query);
    string ShowCommandSelectList(IEnumerable<Command> commands);
}