using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Bitacora.Infrastructure;

public class BitacoraDbContextFactory : IDesignTimeDbContextFactory<BitacoraDbContext>
{
    public BitacoraDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BitacoraDbContext>();
        
        optionsBuilder.UseSqlite("Data Source=App_Data/bitacora.db");

        return new BitacoraDbContext(optionsBuilder.Options);
    }
}