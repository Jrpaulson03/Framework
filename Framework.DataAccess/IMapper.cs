using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.DataAccess {
    /// <summary>
    /// T: Return type.
    /// W: Type of data going in.
    /// </summary>
    public interface IMapper {
        T Map<T, W>(W data);
    }
}
