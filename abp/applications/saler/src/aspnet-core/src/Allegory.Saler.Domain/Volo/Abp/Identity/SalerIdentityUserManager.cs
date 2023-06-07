using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Settings;
using Volo.Abp.Threading;

namespace Volo.Abp.Identity;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IdentityUserManager))]
public class SalerIdentityUserManager : IdentityUserManager
{
    public SalerIdentityUserManager(
        IdentityUserStore store,
        IIdentityRoleRepository roleRepository,
        IIdentityUserRepository userRepository,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<IdentityUser> passwordHasher,
        IEnumerable<IUserValidator<IdentityUser>> userValidators,
        IEnumerable<IPasswordValidator<IdentityUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<IdentityUserManager> logger,
        ICancellationTokenProvider cancellationTokenProvider,
        IOrganizationUnitRepository organizationUnitRepository,
        ISettingProvider settingProvider) :
        base(store,
            roleRepository,
            userRepository,
            optionsAccessor,
            passwordHasher,
            userValidators,
            passwordValidators,
            keyNormalizer,
            errors,
            services,
            logger,
            cancellationTokenProvider,
            organizationUnitRepository,
            settingProvider)
    {

    }

    //public async override Task<IList<Claim>> GetClaimsAsync(IdentityUser user)
    //{
    //    var claims = await base.GetClaimsAsync(user);
    //    return claims;
    //}
}
