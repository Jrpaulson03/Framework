using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Extensions
{
    public static class ExceptionExtensions {

        public static IEnumerable<Exception> FlattenHierarchy(this Exception ex) {
            if (ex == null) {
                throw new ArgumentNullException("ex");
            }
            var innerException = ex;
            do {
                yield return innerException;
                innerException = innerException.InnerException;
            }
            while (innerException != null);
        }
    }
}
