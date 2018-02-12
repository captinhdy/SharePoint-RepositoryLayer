using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrestServiceRepositoryLayer.Interfaces
{
    public interface ICamlQuery
    {
        string Caml { get; set; }
        string OrderByFields { get; set; }
        string ViewFields { get;  set; }
        int RowLimit { get;  set; }

        object ExecuteQuery();
    }
}
