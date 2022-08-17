using Common;
using DBUtility;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class PetitionFileBll
    {
        // 表名
        public string tableName = "ci_petition_doc";

        /// <summary>
        /// 增加一条单位数据
        /// </summary>
        public string Add(Model.PetitionFiles model)
        {
            // 获得索引ID
            string id = Utils.GetNewGuid();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into " + tableName + " (");
            strSql.Append("id,name,petitionId,typenum,url,csize,");
            strSql.Append("deleteUrl,deleteType,isDelete )");
            strSql.Append(" values ('");
            strSql.Append(id + "',@name,@petitionId,@typenum,@url,@csize,'','',0)");

            MySqlParameter[] parameters = {
                    new MySqlParameter("@name", model.name),
                    new MySqlParameter("@petitionId", model.petitionId),
                    new MySqlParameter("@typenum", model.type),
                    new MySqlParameter("@url", model.url),
                    new MySqlParameter("@csize", model.size)};
            int result = SqlHelper.ExecuteSql(strSql.ToString(), parameters);
            if (result > 0)
            {
                return id;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(string petitionId, int isDelete, string ids)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("petitionId=@petitionId, ");
            strSql.Append("isDelete=@isDelete ");
            strSql.Append(" where id in(" + ids + ")");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@petitionId", petitionId),
                    new MySqlParameter("@isDelete", isDelete)};

            int rows = SqlHelper.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,name,petitionId,typenum,");
            strSql.Append("url,csize,deleteUrl,deleteType,isDelete ");
            strSql.Append(" FROM " + tableName);
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by id");
            return SqlHelper.Query(strSql.ToString());
        }

        /// <summary>
        /// 删除一条数据（此删除只是更新字段，使其不再显示，并没有真正的删除）
        /// </summary>
        public bool Delete(string ids)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("isDelete = 1 ");
            strSql.Append(" where petitionId in (");
            strSql.Append(ids + ")");
            string sql = strSql.ToString();
            int rows = SqlHelper.ExecuteSql(strSql.ToString());
            if (rows < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool DeleteBatch(List<string> ids)
        {
            StringBuilder strSql;
            List<CommandInfo> sqlList = new List<CommandInfo>();
            CommandInfo cmd;
            foreach (string id in ids)
            {
                strSql = new StringBuilder();
                strSql.Append("update " + tableName + " set ");
                strSql.Append("isdelete = 1 ");
                strSql.Append(" where petitionId = '" + id + "'");
                cmd = new CommandInfo(strSql.ToString());
                sqlList.Add(cmd);
            }
            int result = SqlHelper.ExecuteSqlTran(sqlList);
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
