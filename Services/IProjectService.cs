using System.Threading.Tasks;
using chub.Models;

namespace chub.Services;

public interface IProjectService
{
    Task<Project> ShowProjects();
}