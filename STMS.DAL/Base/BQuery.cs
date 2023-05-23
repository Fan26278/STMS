using Common;
using STMP.DbUtility;
using STMS.Models.UIModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMS.DAL
{
        public class BQuery<T>
        {
                #region 查询
                /// <summary>
                /// 按条件查询获取实体信息（单个）
                /// </summary>
                /// <param name="strWhere"></param>
                /// <param name="strCols"></param>
                /// <param name="paras"></param>
                /// <returns></returns>
                public T GetModel(string strWhere, string strCols, params SqlParameter[] paras)
                {
                        //生成查询语句
                        string selSql = CreateSql.CreateSelectSql<T>(strWhere, strCols);
                        //生成Reader对象
                        SqlDataReader reader = SqlHelper.ExecuteReader(selSql, 1, paras);
                        //转换为实体对象
                        T model = DbConvert.SqlDataReaderToModel<T>(reader, strCols);
                        //关闭阅读器
                        reader.Close();
                        return model;
                }

                /// <summary>
                /// 根据Id获取信息实体
                /// </summary>
                /// <param name="id"></param>
                /// <param name="strCols"></param>
                /// <returns></returns>
                public T GetById(int id, string strCols)
                {
                        Type type = typeof(T);
                        //构建条件
                        string strWhere = $"[{type.GetPrimary()}]=@Id";
                        SqlParameter[] paras = { new SqlParameter("@Id", id) };
                        return GetModel(strWhere, strCols, paras);
                }

                /// <summary>
                /// 根据条件判断存在
                /// </summary>
                /// <param name="strWhere"></param>
                /// <param name="paras"></param>
                /// <returns>true or false</returns>
                public bool Exists(string strWhere, params SqlParameter[] paras)
                {
                        Type type = typeof(T);
                        string sql = $"SELECT COUNT(1) FROM {type.GetTName()} WHERE {strWhere}";
                        object val = SqlHelper.ExecuteScalar(sql, 1, paras);
                        if (val.GetInt() > 0)
                                return true;
                        else
                                return false;
                }

                /// <summary>
                /// 根据 名称 判断是否已存在
                /// </summary>
                /// <param name="sName">名称列名</param>
                /// <param name="vName">名称值</param>
                /// <param name="id"></param>
                /// <returns></returns>
                public bool ExistsByName(string sName, string vName)
                {
                        string strWhere = $"{sName}=@{sName}";
                        strWhere += " and IsDeleted=0";//有效数据查询
                        SqlParameter[] paras = {
                           new SqlParameter($"@{sName}", vName)
                       };
                        return Exists(strWhere, paras);
                }

                /// <summary>
                /// 同一级别下,检查是否同名
                /// </summary>
                /// <param name="sName">名称列名</param>
                /// <param name="vName">名称值</param>
                /// <param name="sParent">父级Id列名</param>
                /// <param name="parId">父级Id</param>
                /// <returns></returns>
                public bool ExistsByName(string sName, string vName, string sParent, int parId)
                {
                        string strWhere = $"{sName}=@{sName}";
                        if (parId > 0)
                                strWhere += $" and {sParent}=@{sParent}";
                        strWhere += " and IsDeleted=0";
                        SqlParameter[] paras = {
                                new SqlParameter($"@{sName}", vName),
                                 new SqlParameter($"@{sParent}", parId)
                       };
                        return Exists(strWhere, paras);
                }

                /// <summary>
                /// 获取所有列表
                /// </summary>
                /// <param name="cols"></param>
                ///  <param name="IsDeleted">删除标识值  0 1</param>
                /// <returns></returns>
                public List<T> GetModelList(string cols,int IsDeleted)
                {
                        return GetModelList($"IsDeleted={IsDeleted}", cols);
                }

                //public List<T> GetDeletedModelList(string cols)
                //{
                //        return GetModelList("IsDeleted=1", cols);
                //}

                /// <summary>
                /// 按条件查询返回实体列表 SqlDataReader---》查询效率高  
                /// </summary>
                /// <param name="strWhere">条件</param>
                /// <param name="strCols">查询字段</param>
                /// <param name="paras">参数数组</param>
                /// <returns>List<T></returns>
                public List<T> GetModelList(string strWhere, string strCols, params SqlParameter[] paras)
                {
                        //if (string.IsNullOrEmpty(strWhere))
                        //        strWhere = "1=1";
                        //生成查询语句
                        string selSql = CreateSql.CreateSelectSql<T>(strWhere, strCols);
                        //生成Reader
                        SqlDataReader reader = SqlHelper.ExecuteReader(selSql, 1, paras);
                        //转换为List<T>列表
                        List<T> list = DbConvert.SqlDataReaderToList<T>(reader, strCols);
                        //关闭阅读器
                        reader.Close();
                        return list;
                }
                /// <summary>
                /// 返回带行序号的列表
                /// </summary>
                /// <param name="strWhere"></param>
                /// <param name="strCols">strCols 不能包含Id</param>
                /// <param name="paras"></param>
                /// <returns></returns>
                public List<T> GetRowsModelList(string strWhere, string strCols, params SqlParameter[] paras)
                {
                        if (string.IsNullOrEmpty(strWhere))
                                strWhere = "1=1";
                        //生成查询语句
                        string selSql = CreateSql.CreateRowsSelectSql<T>(strWhere, strCols);
                        //生成Reader
                        SqlDataReader reader = SqlHelper.ExecuteReader(selSql, 1, paras);
                        //转换为List<T>列表
                        List<T> list = DbConvert.SqlDataReaderToList<T>(reader, strCols + ",Id");
                        //关闭阅读器
                        reader.Close();
                        return list;

                }

                /// <summary>
                /// 执行sql语句或存储过程，返回DataTable
                /// </summary>
                /// <param name="sql"></param>
                /// <param name="isProc"></param>
                /// <param name="listCols"></param>
                /// <param name="paras"></param>
                /// <returns></returns>
                public DataTable GetList(string sql, int isProc,  params SqlParameter[] paras)
                {
                        DataTable dt = SqlHelper.GetDataTable(sql, isProc, paras);
                        return dt;
                }

                /// <summary>
                /// 执行sql语句或存储过程，返回DataSet 可以是多个结果集
                /// </summary>
                /// <param name="sql"></param>
                /// <param name="isProc"></param>
                /// <param name="paras"></param>
                /// <returns></returns>
                public DataSet GetDs(string sql, int isProc, params SqlParameter[] paras)
                {
                        DataSet ds = SqlHelper.GetDataSet(sql, isProc, paras);
                        return ds;
                }

                /// <summary>
                /// 分页查询
                /// </summary>
                /// <typeparam name="T1"></typeparam>
                /// <param name="sql"></param>
                /// <param name="strCols"></param>
                /// <param name="startIndex"></param>
                /// <param name="pageSize"></param>
                /// <returns></returns>
                public PageModel<T> GetPageList(string sql, string strCols, int startIndex, int pageSize)
                {
                        SqlParameter[] paras ={
                        new SqlParameter("@sql",sql),
                        new SqlParameter("@startIndex",startIndex),
                        new SqlParameter("@endIndex",startIndex +pageSize -1)
                                 };
                        DataSet ds = GetDs("proc_Page", 2, paras);
                        int total = (int)ds.Tables[0].Rows[0][0];
                        List<T> list = DbConvert.DataTableToList<T>(ds.Tables[1], strCols);
                        return new PageModel<T>() { TotalCount = total, ReList = list };
                }

               

                #endregion
        }
}
