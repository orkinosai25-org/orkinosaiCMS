using Microsoft.EntityFrameworkCore;
using OrkinosaiCMS.Core.Entities.Subscriptions;
using OrkinosaiCMS.Core.Interfaces.Repositories;
using OrkinosaiCMS.Core.Interfaces.Services;
using OrkinosaiCMS.Infrastructure.Data;

namespace OrkinosaiCMS.Infrastructure.Services.Subscriptions;

/// <summary>
/// Service implementation for customer management.
/// Redesigned from Mosaic with improved error handling and validation.
/// </summary>
public class CustomerService : ICustomerService
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public CustomerService(
        IRepository<Customer> customerRepository, 
        IUnitOfWork unitOfWork,
        ApplicationDbContext context)
    {
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid customer ID", nameof(id));

        return await _customerRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Customer?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
            throw new ArgumentException("Invalid user ID", nameof(userId));

        return await _context.Customers
            .Include(c => c.Subscriptions)
            .Include(c => c.PaymentMethods)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted, cancellationToken);
    }

    public async Task<Customer?> GetByStripeCustomerIdAsync(string stripeCustomerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(stripeCustomerId))
            throw new ArgumentException("Stripe customer ID cannot be empty", nameof(stripeCustomerId));

        return await _context.Customers
            .Include(c => c.Subscriptions)
            .Include(c => c.PaymentMethods)
            .FirstOrDefaultAsync(c => c.StripeCustomerId == stripeCustomerId && !c.IsDeleted, cancellationToken);
    }

    public async Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        if (customer == null)
            throw new ArgumentNullException(nameof(customer));

        if (string.IsNullOrWhiteSpace(customer.Email))
            throw new ArgumentException("Customer email is required", nameof(customer));

        customer.CreatedOn = DateTime.UtcNow;
        customer.CreatedBy = "System"; // TODO: Replace with actual user context
        
        var result = await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return result;
    }

    public async Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        if (customer == null)
            throw new ArgumentNullException(nameof(customer));

        if (customer.Id <= 0)
            throw new ArgumentException("Invalid customer ID", nameof(customer));

        customer.ModifiedOn = DateTime.UtcNow;
        customer.ModifiedBy = "System"; // TODO: Replace with actual user context
        
        _customerRepository.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return customer;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid customer ID", nameof(id));

        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer == null)
            throw new InvalidOperationException($"Customer with ID {id} not found");

        // Soft delete
        customer.IsDeleted = true;
        customer.DeletedOn = DateTime.UtcNow;
        customer.ModifiedOn = DateTime.UtcNow;
        customer.ModifiedBy = "System"; // TODO: Replace with actual user context
        
        _customerRepository.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Customer> Customers, int TotalCount)> GetAllAsync(
        int page = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
            throw new ArgumentException("Page must be greater than 0", nameof(page));

        if (pageSize < 1 || pageSize > 100)
            throw new ArgumentException("Page size must be between 1 and 100", nameof(pageSize));

        var query = _context.Customers
            .Where(c => !c.IsDeleted)
            .Include(c => c.Subscriptions)
            .OrderByDescending(c => c.CreatedOn);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var customers = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (customers, totalCount);
    }
}
