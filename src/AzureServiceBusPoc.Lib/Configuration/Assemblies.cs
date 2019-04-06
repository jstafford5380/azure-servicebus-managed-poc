using System;
using System.Linq;
using System.Reflection;

namespace AzureServiceBusPoc.Lib.Configuration
{
    public static class Assemblies
    {
        public static Assembly[] ContainingTypes(params Type[] types) 
            => types.Select(type => type.Assembly).ToArray();
    }
}