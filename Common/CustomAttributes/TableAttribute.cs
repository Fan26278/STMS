using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.CustomAttributes
{
   /// <summary>
   /// 表名特性
   /// </summary>
   [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute:Attribute
    {
        public string TableName { get; protected set; }
        public TableAttribute(string tableName)
        {
            this.TableName = tableName;
        }
    }
}
