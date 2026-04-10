using ErpApp.Domain.Entities;

namespace ErpApp.Domain.Ports
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLog log);
    }
}
