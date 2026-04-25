using Amigo.Domain.Abstraction.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.Repositories
{
    public class SlotsRepo(AmigoDbContext _dbContext) 
        : GenericRepo<AvailableSlots, Guid>(_dbContext), ISlotsRepo
    {
        public async Task<List<AvailableSlots>?> GetLockedSlotsAsync(List<Guid> slotsIds)
        {
            return await _dbContext.AvailableSlots
                        .FromSqlInterpolated($@"
                        SELECT *
                        FROM ""AvailableSlots""
                        WHERE ""Id"" = ANY({slotsIds})
                        FOR UPDATE
                        ORDER BY ""Id""")
                        .ToListAsync();
        }
        public async Task<bool> TryReserveSlotAsync(Guid slotId, int qty)
        {
            var rows = await _dbContext.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE ""AvailableSlots""
                SET ""ReservedCount"" = ""ReservedCount"" + {qty}
                WHERE ""Id"" = {slotId}
                AND ""ReservedCount"" + {qty} <= ""MaxCapacity"";
            ");

            return rows > 0;
        }
    }
}
