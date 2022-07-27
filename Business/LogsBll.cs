using Common;
using DBUtility;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text;

namespace Business
{
    public class LogsBll
    {
        // 表名
        public string tableName = "ci_log";

        /// <summary>
        /// 增加一条操作日志数据
        /// </summary>
        /// <param name="model">日志类</param>
        /// <returns>返回受影像的行数</returns>
        public int Add(Model.Logs model)
        {
            StringBuilder strSql = new StringBuilder();
            string guidId = Utils.GetNewGuid();
            strSql.Append("insert into " + tableName + " (");
            strSql.Append("id, userId,ip,detail,typeNum,userName,");
            strSql.Append(" modifyTime,typeName,infoLevel )");
            strSql.Append(" values (");
            strSql.Append("'" + guidId + "',@userId,@ip,@detail,@typeNum,");
            strSql.Append("@userName,now(),@typeName,@infoLevel)");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@userId", MySqlDbType.VarChar),
                    new MySqlParameter("@ip", MySqlDbType.VarChar, 100),
                    new MySqlParameter("@detail", MySqlDbType.VarChar),
                    new MySqlParameter("@typeNum", MySqlDbType.Int32),
                    new MySqlParameter("@userName", MySqlDbType.VarChar,100),
                    new MySqlParameter("@typeName",MySqlDbType.VarChar,100),
                    new MySqlParameter("@infoLevel",MySqlDbType.Int32, 1)
            };
            parameters[0].Value = model.userId;
            parameters[1].Value = model.ip;
            parameters[2].Value = model.detail;
            parameters[3].Value = model.typeNum;
            parameters[4].Value = model.userName;
            parameters[5].Value = model.typeName;
            parameters[6].Value = model.infoLevel;
            return SqlHelper.ExecuteSql(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT t1.id,t1.userId,t1.userName,");
            strSql.Append("t1.ip,t1.detail,t1.typeName,t1.modifyTime,t1.infoLevel,");
            strSql.Append("t2.name as nickName FROM ci_log t1");
            strSql.Append(" LEFT JOIN ci_admin t2 on t1.userId = t2.id ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" WHERE " + strWhere);
            }
            string strCountSql = PagingHelper.CreateCountingSql(strSql.ToString());
            recordCount = Convert.ToInt32(SqlHelper.GetSingle(strCountSql));
            string strPageSql = PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder);
            return SqlHelper.Query(strPageSql);
        }
        /// <summary>
        /// 删除七天前的日志文件
        /// </summary>
        /// <returns>删除的操作日志数量</returns>
        public int Delete()
        {
            string dateTime = DateTime.Now.AddDays(-7).ToString("yyyy/MM/dd HH:mm:ss");
            StringBuilder strSql = new StringBuilder();
            strSql.Append("DELETE FROM " + tableName + " WHERE ");
            strSql.Append("modifyTime <  '" + dateTime + "'");
            return SqlHelper.ExecuteSql(strSql.ToString());
        }
    }
}
