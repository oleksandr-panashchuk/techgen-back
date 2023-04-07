using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techgen.Domain
{
    public interface IEntity<TKey>
         where TKey : IComparable<TKey>, IComparable
    {
        TKey Id { get; set; }
    }

    public interface IEntity : IEntity<int>
    {
    }

}
