using Allegory.Saler.ClientUsers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;
using Volo.Abp.Uow;

namespace Volo.Abp.Identity;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(AbpUserClaimsPrincipalFactory))]
public class SalerUserClaimsPrincipalFactory : AbpUserClaimsPrincipalFactory
{
    protected IClientUserRepository ClientUserRepository { get; }

    public SalerUserClaimsPrincipalFactory(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> options,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IAbpClaimsPrincipalFactory abpClaimsPrincipalFactory,
        IClientUserRepository clientUserRepository)
        : base(
            userManager,
            roleManager,
            options,
            currentPrincipalAccessor,
            abpClaimsPrincipalFactory)
    {
        ClientUserRepository = clientUserRepository;
    }

    [UnitOfWork]
    public async override Task<ClaimsPrincipal> CreateAsync(IdentityUser user)
    {
        var principal = await base.CreateAsync(user);
        var identity = principal.Identities.First();

        var clientId = (await ClientUserRepository.FindAsync(clientUser => clientUser.UserId == user.Id))?.ClientId;
        if (clientId.HasValue)
            identity.AddIfNotContains(new Claim(SalerClaimTypes.ClientId, clientId.Value.ToString()));

        return principal;
    }
}
