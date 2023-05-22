
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data.SqlClient;
using System.Collections;

namespace Common
{
    /// <summary>
    /// 类型转换处理---DataRow/DataTable/SqlDataReader    T/List<T>
    /// </summary>
    public class DbConvert
    {
        /// <summary>
        /// 将DataRow转换成实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        private static T DataRowToModel<T>(DataRow dr, string cols)
        {
            //创建实例对象
            T model = Activator.CreateInstance<T>();

            Type type = typeof(T);
            if (dr != null)
            {
                //获取指定列名的属性数组
                var properties = PropertyHelper.GetTypeProperties<T>(cols);
                //将第列的值赋值给对应的属性
                foreach (var p in properties)
                {
                    string colName = p.GetColName();
                    if (dr[colName] is DBNull)
                        p.SetValue(model, null);//为属性设置值 
                    else
                    {
                        SetPropertyValue<T>(model, dr[colName], p);
                    }
                }
                return model;
            }
            else
                return default(T);

        }

        /// <summary>
        /// 将DataTable转换成List<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> DataTableToList<T>(DataTable dt, string cols)
        {
            List<T> list = new List<T>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    //将每行转换为一个对象
                    T model = DataRowToModel<T>(dr, cols);
                    list.Add(model);
                }
            }
            return list;
        }
        /// <summary>
        /// 将SqlDataReader对象转换成实体（返回一条数据）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static T SqlDataReaderToModel<T>(SqlDataReader reader, string cols)
        {
            //创建指定类型的实例
            T model = Activator.CreateInstance<T>();
            Type type = typeof(T);
            //获取指定列名的属性数组
            var properties = PropertyHelper.GetTypeProperties<T>(cols);
            if (reader.Read())
            {
                //将指定列的值赋值给对应的属性
                foreach (var p in properties)
                {
                    string colName = p.GetColName();
                    if (reader[colName] is DBNull)
                    {
                        p.SetValue(model, null);
                    }
                    else
                    {
                        SetPropertyValue<T>(model, reader[colName], p);
                    }
                }
                return model;
            }
            else return default(T);
        }

        /// <summary>
        /// 将SqlDataReader转换成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<T> SqlDataReaderToList<T>(SqlDataReader reader, string cols)
        {
            List<T> list = new List<T>();
            Type type = typeof(T);
            var properties = PropertyHelper.GetTypeProperties<T>(cols);
            while (reader.Read())
            {
                T model = Activator.CreateInstance<T>();
                foreach (var p in properties)
                {
                    string colName = p.GetColName();
                    if (reader[colName] is DBNull)
                    {
                        p.SetValue(model, null);
                    }
                    else
                    {
                        SetPropertyValue<T>(model, reader[colName], p);
                    }
                }
                list.Add(model);
            }
            return list;
        }
        //设置属性值    属性的数据类型Nullable<int>   int?
        private static void SetPropertyValue<T>(T model, object obj, PropertyInfo property)
        {
            //数据类型是泛型类型&&泛型类型定义如果是Nullable<>  
            if (property.PropertyType.IsGenericType &&
           property.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                property.SetValue(model, Convert.ChangeType(obj, Nullable.GetUnderlyingType(property.PropertyType)));
            }
            else
            {
                property.SetValue(model, Convert.ChangeType(obj, property.PropertyType));
            }



        }


    }
}
