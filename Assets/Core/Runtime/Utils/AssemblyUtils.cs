using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NS.Core.Utils {
    public class AssemblyUtils {
        public static IEnumerable<Assembly> GetAssemblies() {
#if UNITY_EDITOR
            return AppDomain.CurrentDomain.GetAssemblies().Where(a => {
                var name = a.FullName;
                return
                    !name.Contains("Test", StringComparison.OrdinalIgnoreCase) &&
                    !name.Contains("Editor", StringComparison.OrdinalIgnoreCase) &&
                    !a.IsDynamic;
            });
#else
            return AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic);
#endif
        }
    }
}