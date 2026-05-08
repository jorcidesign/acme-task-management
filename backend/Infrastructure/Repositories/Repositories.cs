using AcmeTaskApi.Application.Common;
using AcmeTaskApi.Application.Interfaces.Repositories;
using AcmeTaskApi.Domain.Enums;
using AcmeTaskApi.Domain.Models;
using AcmeTaskApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
 
namespace AcmeTaskApi.Infrastructure.Repositories;
 
public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
 
    public UserRepository(AppDbContext context) => _context = context;
 
    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => _context.Users
                   .AsNoTracking()
                   .SingleOrDefaultAsync(u => u.Email == email, ct);
 
    public Task<User?> GetByIdAsync(long id, CancellationToken ct = default)
        => _context.Users
                   .AsNoTracking()
                   .SingleOrDefaultAsync(u => u.Id == id, ct);
 
    public async Task<User> AddAsync(User user, CancellationToken ct = default)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(ct);
        return user;
    }
}
 
public sealed class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;
 
    public TaskRepository(AppDbContext context) => _context = context;
 
    public Task<TaskItem?> GetByIdAndUserAsync(long taskId, long userId, CancellationToken ct = default)
        => _context.Tasks
                   .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId, ct);
 
    public async Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetPagedByUserAsync(
        long userId,
        PaginationParams pagination,
        TaskStatus? statusFilter = null,
        DateTime? dueDateFrom = null,
        DateTime? dueDateTo = null,
        CancellationToken ct = default)
    {
        var query = _context.Tasks
                            .AsNoTracking()
                            .Where(t => t.UserId == userId);
 
        if (statusFilter.HasValue)
            query = query.Where(t => t.Status == statusFilter.Value);
 
        if (dueDateFrom.HasValue)
            query = query.Where(t => t.DueDate >= dueDateFrom.Value.ToUniversalTime());
 
        if (dueDateTo.HasValue)
            query = query.Where(t => t.DueDate <= dueDateTo.Value.ToUniversalTime());
 
        var total = await query.CountAsync(ct);
 
        var items = await query
            .OrderBy(t => t.DueDate == null)   // tasks with due dates first
            .ThenBy(t => t.DueDate)
            .ThenBy(t => t.CreatedAt)
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync(ct);
 
        return (items, total);
    }
 
    public async Task<TaskItem> AddAsync(TaskItem task, CancellationToken ct = default)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(ct);
        return task;
    }
 
    public Task UpdateAsync(TaskItem task, CancellationToken ct = default)
    {
        _context.Tasks.Update(task);
        return _context.SaveChangesAsync(ct);
    }
 
    public Task DeleteAsync(TaskItem task, CancellationToken ct = default)
    {
        _context.Tasks.Remove(task);
        return _context.SaveChangesAsync(ct);
    }
}