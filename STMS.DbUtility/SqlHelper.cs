using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMS.DbUtility
{
    public class SqlHelper
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        private static readonly string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        /// <summary>
        /// 增、删、改的通用方法
        /// 执行Sql语句或存储过程，返回受影响的行数
        /// SQL注入 
        /// </summary>
        /// <param name="sql">sql语句或存储过程名</param>
        /// <param name="cmdType">执行的脚本类型 1:sql语句  2:存储过程</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sql, int cmdType, params SqlParameter[] parameters)
        {
            //select @@Identity 返回上一次插入记录时自动产生的ID
            int result = 0;//返回结果
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                //执行脚本的对象cmd
                SqlCommand cmd = BuilderCommand(conn, sql, cmdType, null, parameters);
                result = cmd.ExecuteNonQuery();//执行T-SQL并返回受影响行数
                cmd.Parameters.Clear();
            }
            //using原理：类似于try finally
            return result;
        }

        /// <summary>
        /// 执行sql查询，返回第一行第一列的值
        /// </summary>
        /// <param name="sql">sql语句或存储过程</param>
        /// <param name="cmdType">执行的脚本类型 1:sql语句  2:存储过程</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static object ExecuteScalar(string sql, int cmdType, params SqlParameter[] parameters)
        {
            object result = null;//返回结果
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                //执行脚本的对象cmd
                SqlCommand cmd = BuilderCommand(conn, sql, cmdType, null, parameters);
                result = cmd.ExecuteScalar();//执行T-SQL并返回第一行第一列的值
                cmd.Parameters.Clear();
                if (result == null || result == DBNull.Value)
                {
                    return null;
                }
                else
                {
                    return result;
                }
            }
        }

        /// <summary>
        /// 执行sql查询,返回SqlDataReader对象
        /// </summary>
        /// <param name="sql">sql语句或存储过程</param>
        /// <param name="cmdType">执行的脚本类型 1:sql语句  2:存储过程</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string sql, int cmdType, params SqlParameter[] parameters)
        {
            SqlConnection conn = new SqlConnection(connStr);
            SqlCommand cmd = BuilderCommand(conn, sql, cmdType, null, parameters);
            SqlDataReader reader;
            try
            {
                //conn.Open();连接必须是打开的
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);//推荐
                return reader;
            }
            catch (Exception ex)
            {
                conn.Close();
                throw new Exception("创建reader对象发生异常", ex);
            }

        }

        /// <summary>
        /// 执行查询，查询结果填充到DataTable 只针对查询一个表的情况
        /// </summary>
        /// <param name="sql">sql语句或存储过程</param>
        /// <param name="cmdType">执行的脚本类型 1:sql语句  2:存储过程</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql, int cmdType, params SqlParameter[] parameters)
        {
            DataTable dt = null;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = BuilderCommand(conn, sql, cmdType, null, parameters);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
                cmd.Parameters.Clear();
            }
            return dt;
        }

        /// <summary>
        /// 执行查询，数据填充到DataSet（多个结果集）
        /// </summary>
        /// <param name="sql">sql语句或存储过程</param>
        /// <param name="cmdType">执行的脚本类型 1:sql语句  2:存储过程</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static DataSet GetDataSet(string sql, int cmdType, params SqlParameter[] parameters)
        {
            DataSet ds = null;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = BuilderCommand(conn, sql, cmdType, null, parameters);
                //数据适配器
                //conn 自动打开  断开式连接
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                cmd.Parameters.Clear();
                //自动关闭conn
            }
            return ds;
        }

        /// <summary>
        /// 事务 执行批量sql
        /// </summary>
        /// <param name="listSql"></param>
        /// <returns></returns>
        public static bool ExecuteTrans(List<string> listSql)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();//必要条件
                SqlTransaction trans = conn.BeginTransaction();//开启事务
                SqlCommand cmd = BuilderCommand(conn, "", 1, trans);
                try
                {
                    int count = 0;
                    for (int i = 0; i < listSql.Count; i++)
                    {
                        if (listSql[i].Length > 0)
                        {
                            cmd.CommandText = listSql[i];
                            cmd.CommandType = CommandType.Text;
                            count += cmd.ExecuteNonQuery();//一般执行的是增删改命令
                        }
                    }
                    trans.Commit();//提交
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();//回滚
                    throw new Exception("执行事务出现异常", ex);
                }
            }
        }

        /// <summary>
        /// 事务 批量执行 CommandInfo 包括sql,脚本类型，参数列表
        /// </summary>
        /// <param name="comList"></param>
        /// <returns></returns>
        public static bool ExecuteTrans(List<CommandInfo> comList)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                SqlCommand cmd = BuilderCommand(conn, "", 1, trans);
                try
                {
                    int count = 0;
                    for (int i = 0; i < comList.Count; i++)
                    {
                        cmd.CommandText = comList[i].CommandText;

                        if (comList[i].IsProc)
                            cmd.CommandType = CommandType.StoredProcedure;
                        else
                            cmd.CommandType = CommandType.Text;

                        if (comList[i].Paras != null && comList[i].Paras.Length > 0)
                        {
                            cmd.Parameters.Clear();
                            foreach (var p in comList[i].Paras)
                            {
                                cmd.Parameters.Add(p);
                            }
                        }
                        count += cmd.ExecuteNonQuery();

                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception("执行事务出现异常", ex);
                }
            }
        }

        //public static T ExecuteSql<T>(string sql, int cmdType, DbParameter[] paras, Func<IDbCommand, T> action)
        //{
        //    using (DbConnection conn = new SqlConnection(connStr))
        //    {
        //        conn.Open();
        //        IDbCommand cmd = conn.CreateCommand();
        //        cmd.CommandText = sql;
        //        if (cmdType==2)
        //            cmd.CommandType = CommandType.StoredProcedure;
        //        return action(cmd);
        //    }
        //}

        public static T ExecuteTrans<T>(Func<IDbCommand, T> action)
        {
            using (IDbConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                IDbTransaction trans = conn.BeginTransaction();
                IDbCommand cmd = conn.CreateCommand();
                cmd.Transaction = trans;
                return action(cmd);
            }
        }




        /// <summary>
        /// 构建SqlCommand
        /// </summary>
        /// <param name="conn">数据库连接对象</param>
        /// <param name="sql">SQL语句或存储过程</param>
        /// <param name="comType">命令字符串的类型</param>
        /// <param name="trans">事务</param>
        /// <param name="paras">参数数组</param>
        /// <returns></returns>
        private static SqlCommand BuilderCommand(SqlConnection conn, string sql, int cmdType, SqlTransaction trans, params SqlParameter[] paras)
        {
            if (conn == null) throw new ArgumentNullException("连接对象不能为空！");
            SqlCommand command = new SqlCommand(sql, conn);
            if (cmdType == 2)
                command.CommandType = CommandType.StoredProcedure;
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            if (trans != null)
                command.Transaction = trans;
            if (paras != null && paras.Length > 0)
            {
                command.Parameters.Clear();
                command.Parameters.AddRange(paras);
            }
            return command;
        }

        /// <summary>
        /// 添加参数集合
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="paras"></param>
        public static void AddParas(IDbCommand cmd, SqlParameter[] paras)
        {
            cmd.Parameters.Clear();
            if(paras!=null &&paras.Length >0)
            {
                foreach(SqlParameter p in paras)
                {
                    cmd.Parameters.Add(p);
                }
            }
        }

    }
}
