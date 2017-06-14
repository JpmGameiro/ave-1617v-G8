using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapperEmit
{
    public interface IMapper <TSrc, TDest> : IMapperEmit
    {
        TDest Map (TSrc src);
        TDest[] Map (TSrc[] src);   
        IEnumerable<TDest> MapLazy(IEnumerable<TSrc> src);
    }
}
