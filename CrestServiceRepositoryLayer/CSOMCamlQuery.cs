using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SP = Microsoft.SharePoint.Client;
using CrestServiceRepositoryLayer.Interfaces;

namespace CrestServiceRepositoryLayer
{
    public class CSOMCamlQuery: ICamlQuery
    {
        public string Caml { get; set; }
        public string OrderByFields { get; set; }
        public string ViewFields { get;  set; }
        public int RowLimit { get;  set; }


        public object ExecuteQuery()
        {
            SP.CamlQuery query = new SP.CamlQuery();
            query.ViewXml = CreateCSOMQuery();

            return query;
        }

        private string CreateCSOMQuery()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<View>");
            sb.Append("<Query>");
            sb.Append(Caml);
            if (!String.IsNullOrEmpty(OrderByFields))
            {
                sb.Append("<OrderBy>");
                sb.Append(OrderByFields);
                sb.Append("</OrderBy>");
            }
            sb.Append("</Query>");

            if (RowLimit != null && RowLimit > 0)
            {
                sb.Append("<RowLimit>" + RowLimit.ToString() + "</RowLimit>");
            }

            if (!String.IsNullOrEmpty(ViewFields))
            {
                sb.Append("<ViewFields>" + ViewFields.ToString() + "</ViewFields>");
            }
            sb.Append("</View>");
            

            return sb.ToString();
        }
    }
}
