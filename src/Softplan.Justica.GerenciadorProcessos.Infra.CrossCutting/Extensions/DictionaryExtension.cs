using System.Collections.Generic;
using System.Linq;

namespace Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Extensions
{
    public static class DictionaryExtension
    {
        public static IDictionary<T1, T2> Merge<T1, T2>(this IDictionary<T1, T2> dic1, IDictionary<T1, T2> dic2)
        {
            if (dic1 == null || dic2 == null)
            {
                return dic1 ?? dic2;
            }

            return dic1.Concat(dic2).GroupBy(i => i.Key).ToDictionary(group => group.Key, group => group.First().Value);
        }
    }
}