using System;
using System.Security;
using System.Security.Claims;
using PostSharp.Aspects;
using PostSharp.Extensibility;

namespace Allegory.Standart.Aspects.Postsharp
{
    [Serializable]
    [MulticastAttributeUsage(MulticastTargets.Method, AllowMultiple = false, TargetExternalMemberAttributes = MulticastAttributes.Instance)]
    public class AuthorizeAspect : OnMethodBoundaryAspect
    {
        public string Roles { get; set; }
        public override void OnEntry(MethodExecutionArgs args)
        {
            if (!(ClaimsPrincipal.Current?.Identity.IsAuthenticated ?? false))
                throw new SecurityException("Session not found");

            string[] roles = Roles?.Split(',');
            if (roles != null)
            {
                bool isAuthorized = false;

                for (int i = 0; i < roles.Length; i++)
                {
                    if (ClaimsPrincipal.Current.IsInRole(roles[i]))
                    {
                        isAuthorized = true;
                        break;
                    }
                }
                if (!isAuthorized)
                    throw new SecurityException("You are not authorized");
            }
            base.OnEntry(args);
        }
    }
}
