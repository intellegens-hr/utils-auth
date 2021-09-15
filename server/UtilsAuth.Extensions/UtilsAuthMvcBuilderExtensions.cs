using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UtilsAuth.Core.Api.Controllers;

namespace UtilsAuth.Extensions
{
    public static class UtilsAuthMvcBuilderExtensions
    {
        private static Assembly controllersAssembly = typeof(AuthenticationControllerApi).Assembly;

        public static IMvcBuilder AddUtilsAuthAuthenticationControllerAssemblyPart(this IMvcBuilder builder)
        {
            builder.RemoveUtilsAuthAuthenticationControllerAssemblyPart();
            builder.AddApplicationPart(controllersAssembly);

            return builder;
        }

        public static IMvcCoreBuilder AddUtilsAuthAuthenticationControllerAssemblyPart(this IMvcCoreBuilder builder)
        {
            builder.RemoveUtilsAuthAuthenticationControllerAssemblyPart();
            builder.AddApplicationPart(controllersAssembly);

            return builder;
        }

        public static IMvcBuilder RemoveUtilsAuthAuthenticationControllerAssemblyPart(this IMvcBuilder builder)
        {
            builder.ConfigureApplicationPartManager(x =>
            {
                x.ApplicationParts.RemoveAuthenticationControllerFromAppParts();
            });

            return builder;
        }

        public static IMvcCoreBuilder RemoveUtilsAuthAuthenticationControllerAssemblyPart(this IMvcCoreBuilder builder)
        {
            builder.ConfigureApplicationPartManager(x =>
            {
                x.ApplicationParts.RemoveAuthenticationControllerFromAppParts();
            });

            return builder;
        }

        private static void RemoveAuthenticationControllerFromAppParts(this IList<ApplicationPart> parts)
        {
            var part = parts
                .Where(x => x.GetType() == typeof(AssemblyPart))
                .Select(x => (AssemblyPart)x)
                .Where(x => x.Assembly.FullName == controllersAssembly.FullName)
                .FirstOrDefault();

            if (part != null)
                parts.Remove(part);
        }
    }
}