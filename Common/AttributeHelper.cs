using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.CustomAttributes;
using System.Reflection;

namespace Common
{
    public static class AttributeHelper
    {
        /// <summary>
        /// 获取映射表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTName(this Type type)
        {
            string tableName = string.Empty;
            //获取指定类型的自定义特性
            object[] attrs = type.GetCustomAttributes(false);
            foreach (var  attr in attrs)
            {
                if(attr is TableAttribute)
                {
                    TableAttribute tableAttribute = attr as TableAttribute;
                    tableName = tableAttribute.TableName;
                }
            }
            if(string.IsNullOrEmpty(tableName))
            {
                tableName = type.Name;
            }
            return tableName;
        }

        /// <summary>
        /// 获取映射列名
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetColName(this PropertyInfo property)
        {
            string columnName = string.Empty;
            //获取指定类型的自定义特性
            object[] attrs = property.GetCustomAttributes(false);
            foreach (var attr in attrs)
            {
                if(attr is ColumnAttribute)
                {
                    ColumnAttribute colAttr = attr as ColumnAttribute;
                    columnName = colAttr.ColumnName;
                }
            }
            if(string.IsNullOrEmpty(columnName))
            {
                columnName = property.Name;
            }
            return columnName;
        }

        /// <summary>
        /// 判断主键是否自增
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsIncrement(this Type type)
        {
            //获取指定类型的自定义特性
            object[] attributes = type.GetCustomAttributes(false);
            foreach (var attr in attributes)
            {
                if (attr is PrimaryKeyAttribute)
                {
                    PrimaryKeyAttribute primaryKeyAttr = attr as PrimaryKeyAttribute;
                    return primaryKeyAttr.autoIncrement;
                }
            }
            return false;
        } 
 
        ///<summary>  
        /// 获取主键名  
        /// </summary>  
        /// <param name="type"></param>  
        /// <returns></returns>  
        public static string GetPrimary(this Type type)
        {
            object[] attributes = type.GetCustomAttributes(false);
            foreach (var attr in attributes)
            {
                if (attr is PrimaryKeyAttribute)
                {
                    PrimaryKeyAttribute primaryKeyAttr = attr as PrimaryKeyAttribute;
                    return primaryKeyAttr.PrimaryKey;
                }
            }
            return null;
        }

        /// <summary>  
        /// 判断属性是否为主键  
        /// </summary>  
        /// <param name="type"></param>  
        /// <param name="property"></param>  
        /// <returns></returns>  
        public static bool IsPrimary(this Type type, PropertyInfo property)
        {
            //获取主键名
            string primaryKeyName = type.GetPrimary();
            //获取指定属性的映射列名
            string columnName = property.GetColName();
            //判断是否相等
            return (primaryKeyName == columnName);
        }


        

        
    }
}
