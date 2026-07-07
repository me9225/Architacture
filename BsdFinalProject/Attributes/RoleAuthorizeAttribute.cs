using Microsoft.AspNetCore.Authorization;
using System;
using BsdFinalProject.Models;

namespace BsdFinalProject.Attributes
{
    // Simple attribute wrapper to allow usage like [RoleAuthorize(Role.Admin)]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        public RoleAuthorizeAttribute(Role role)
        {
            Roles = role.ToString();
        }
    }
}
