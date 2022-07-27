using DBUtility;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Business
{
    public class OptionsBll
    {

        /// <summary>
        /// 通过关键字获得系统参数信息
        /// </summary>
        /// <param name="optionName">参数名称</param>
        /// <returns>参数内容</returns>
        public DataTable GetOptions(string optionName = "")
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select op_name,op_value from ci_options ");
            strSql.Append("where 1=1 ");
            if (!string.IsNullOrEmpty(optionName))
            {
                strSql.Append(" and op_name='" + optionName + "'");
            }
            DataSet ds = SqlHelper.Query(strSql.ToString());
            return ds.Tables[0];
        }

        /// <summary>
        /// 更新系统参数信息
        /// </summary>
        /// <param name="optionValue">参数内容</param>
        /// <param name="optionName">参数名称</param>
        /// <returns></returns>
        public bool UpdateOptions(List<Model.Options> list)
        {
            StringBuilder strSql;
            CommandInfo cmd;
            List<CommandInfo> sqllist = new List<CommandInfo>();

            foreach (Model.Options model in list)
            {
                strSql = new StringBuilder();
                strSql.Append("update ci_options set ");
                strSql.Append("op_value = @op_value");
                strSql.Append(" where op_name=@op_name" );
                MySqlParameter[] parameters = {
                            new MySqlParameter("@op_value", model.op_value),
                            new MySqlParameter("@op_name",  model.op_name)
                };
                cmd = new CommandInfo(strSql.ToString(), parameters);
                sqllist.Add(cmd);
            }
            int result = SqlHelper.ExecuteSqlTran(sqllist);
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
