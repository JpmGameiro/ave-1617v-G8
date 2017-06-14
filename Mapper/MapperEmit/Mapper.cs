using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MapperEmit
{
    public class Mapper<TSrc, TDest> : MapperEmit, IMapper<TSrc,TDest>
    {
        
        public Mapper(Type klassSrc, Type klassDest) : base(klassSrc, klassDest)
        {
            
        }

        public TDest Map (TSrc src)
        {
            return (TDest)base.Map(src);
        }

        public TDest[] Map (TSrc[] src)
        {
            mappingArray = true;
            TDest[] dests = new TDest[src.Length];
            for (int i = 0; i < dests.Length; ++i)
            {
                dests[i] = Map(src[i]);
            }
            return dests;
        }

        public C Map<C>(TSrc[] src) where C : ICollection<TDest>, new()
        {
            C c = new C();
            mappingArray = true;
            for (int i = 0; i < src.Length; ++i)
            {
                c.Add(Map(src[i]));
            }
            return c;
        }

        public IEnumerable<TDest> MapLazy(IEnumerable<TSrc> src)
        {
            mappingArray = true;
            foreach (TSrc tSrc in src)
            {
                yield return Map(tSrc);
            }
        }

        public Mapper<TSrc, TDest> For<R>(string s, Func<R> func) where R : class
        {
            PropertyInfo p = typeof(TDest).GetProperty(s);
            if (p != null)
            {
                if (p.PropertyType == typeof(R))
                    map.Add(new KeyValuePair<string, Func<object>>(s, func));
                else throw new MismatchTypeException("Types not Compatibles");
            }
            return this;
        }
    }
}
