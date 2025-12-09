using OrkinosaiCMS.Core.Entities.Sites;
using OrkinosaiCMS.Core.Interfaces.Repositories;
using OrkinosaiCMS.Core.Interfaces.Services;

namespace OrkinosaiCMS.Infrastructure.Services;

/// <summary>
/// Service implementation for permission management operations
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IRepository<RolePermission> _rolePermissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PermissionService(
        IRepository<Permission> permissionRepository,
        IRepository<UserRole> userRoleRepository,
        IRepository<RolePermission> rolePermissionRepository,
        IUnitOfWork unitOfWork)
    {
        _permissionRepository = permissionRepository;
        _userRoleRepository = userRoleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _permissionRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _permissionRepository.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _permissionRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Permission> CreateAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        permission.CreatedOn = DateTime.UtcNow;
        await _permissionRepository.AddAsync(permission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return permission;
    }

    public async Task<Permission> UpdateAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        permission.ModifiedOn = DateTime.UtcNow;
        _permissionRepository.Update(permission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return permission;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var permission = await _permissionRepository.GetByIdAsync(id, cancellationToken);
        if (permission != null)
        {
            _permissionRepository.Remove(permission);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> UserHasPermissionAsync(int userId, string permissionName, CancellationToken cancellationToken = default)
    {
        // Get user's roles
        var userRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == userId, cancellationToken);
        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

        // Get permission
        var permission = await GetByNameAsync(permissionName, cancellationToken);
        if (permission == null)
        {
            return false;
        }

        // Check if any of user's roles have this permission
        foreach (var roleId in roleIds)
        {
            var hasPermission = await _rolePermissionRepository.AnyAsync(
                rp => rp.RoleId == roleId && rp.PermissionId == permission.Id, cancellationToken);

            if (hasPermission)
            {
                return true;
            }
        }

        return false;
    }

    public async Task<IEnumerable<Permission>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken = default)
    {
        // Get user's roles
        var userRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == userId, cancellationToken);
        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

        // Get all permissions for these roles
        var permissions = new HashSet<Permission>();
        foreach (var roleId in roleIds)
        {
            var rolePermissions = await _rolePermissionRepository.FindAsync(
                rp => rp.RoleId == roleId, cancellationToken);
            
            foreach (var rp in rolePermissions)
            {
                var permission = await _permissionRepository.GetByIdAsync(rp.PermissionId, cancellationToken);
                if (permission != null)
                {
                    permissions.Add(permission);
                }
            }
        }

        return permissions;
    }
}
