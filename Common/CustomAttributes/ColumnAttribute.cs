using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.CustomAttributes
{
    /// <summary>
    /// 列名映射特性
    /// </summary>
    
    [AttributeUsage(AttributeTargets.Property)]
    

    public class ColumnAttribute:Attribute
    {
        public string ColumnName { get; protected set; }
        public ColumnAttribute(string colName)
        {
            this.ColumnName = colName;
        }
    }
}
