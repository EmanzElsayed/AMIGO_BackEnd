using Amigo.Domain.Abstraction.Repositories;
using Amigo.Domain.DTO.AvailableSlots;
using Amigo.SharedKernal.Constants;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Amigo.Persistence.Repositories
{
    public class SlotsRepo(AmigoDbContext _dbContext)
        : GenericRepo<AvailableSlots, Guid>(_dbContext), ISlotsRepo
    {
        private const string TableName = "\"AvailableSlots\"";
        private const string Schema = "tour";

     
        public async Task<HashSet<Guid>> ReserveBulkAsync(
            IReadOnlyCollection<SlotReservationRequest> requests,
            CancellationToken ct = default)
        {
            if (requests.Count == 0)
                return new HashSet<Guid>();

            var (sql, parameters) = BuildBulkUpdateQuery(
                requests.Select(x => (x.SlotId, x.Quantity)).ToList(),
                isIncrease: true,
                checkCapacity: true);

            var result = await _dbContext.Database
                .SqlQueryRaw<Guid>(sql, parameters)
                .ToListAsync(ct);

            return result.ToHashSet();
        }

     
        public async Task BulkDecreaseReservedCountAsync(
            IReadOnlyCollection<(Guid SlotId, int Quantity)> updates,
            CancellationToken ct = default)
        {
            if (updates.Count == 0)
                return;

            var (sql, parameters) = BuildBulkUpdateQuery(
                updates.ToList(),
                isIncrease: false,
                checkCapacity: false);

            await _dbContext.Database.ExecuteSqlRawAsync(
                sql,
                parameters,
                ct);
        }

       
        private (string sql, object[] parameters) BuildBulkUpdateQuery(
            IReadOnlyCollection<(Guid SlotId, int Quantity)> data,
            bool isIncrease,
            bool checkCapacity)
        {
            var sql = new StringBuilder();

            sql.AppendLine("WITH data(\"Id\",\"Qty\") AS ( VALUES ");

            var parameters = new List<object>();
            int i = 0;

            foreach (var item in data)
            {
                if (i > 0)
                    sql.Append(",");

                sql.Append($"(@p{i * 2}, @p{i * 2 + 1})");

                parameters.Add(item.SlotId);
                parameters.Add(item.Quantity);

                i++;
            }

            sql.AppendLine(")");

            sql.AppendLine($@"
            UPDATE {Schema}.{TableName} s
            SET ""ReservedCount"" = s.""ReservedCount"" {(isIncrease ? "+" : "-")} data.""Qty""
            FROM data
            WHERE s.""Id"" = data.""Id""
            ");

           
            if (checkCapacity)
            {
                sql.AppendLine(@"
                AND s.""ReservedCount"" + data.""Qty"" <= s.""MaxCapacity""
                ");
            }

            if (!isIncrease)
            {
                sql.AppendLine(@"
                AND s.""ReservedCount"" - data.""Qty"" >= 0
                ");
            }

            if (isIncrease)
            {
                sql.AppendLine(@"RETURNING s.""Id"";");
            }

            return (sql.ToString(), parameters.ToArray());
        }
    }
}