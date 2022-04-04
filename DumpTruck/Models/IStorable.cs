using Microsoft.EntityFrameworkCore;

namespace DumpTruck.Models;

public interface IStorable
{
    void Save(DbContext context, int? parentId = null);
    void Delete(DbContext context);
}