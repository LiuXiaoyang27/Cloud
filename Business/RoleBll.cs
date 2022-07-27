using Common;
using DBUtility;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Business
{
    public class RoleBll
    {
        // 用户角色表名
        public string tableName = "ci_role";

        public string tableValueName = "ci_role_value";

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public bool Exists(string roleId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + tableName);
            strSql.Append(" where isDelete = 0 and id=@id ");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@id", roleId)
            };
            return SqlHelper.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 增加一条用户角色数据
        /// </summary>
        public bool Add(Model.Role model)
        {
            // 获得主键ID
            string newId = Utils.GetNewGuid();
            StringBuilder roleSql = new StringBuilder();
            roleSql.Append("insert into " + tableName + " (");
            roleSql.Append("id,name,status,isDelete,typeId,isAdmin,modifyTime )");
            roleSql.Append(" values (");
            roleSql.AppendFormat("@id,@name,1,0,@typeId,0,now())");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id", newId),
                new MySqlParameter("@name", model.name),
                new MySqlParameter("@typeId",model.type)
            };
            List<CommandInfo> sqllist = new List<CommandInfo>();
            CommandInfo cmd = new CommandInfo(roleSql.ToString(), parameters);
            sqllist.Add(cmd);

            StringBuilder roleValueSql;
            if (model.roleValues != null)
            {
                foreach (Model.RoleValue valueModel in model.roleValues)
                {
                    roleValueSql = new StringBuilder();
                    roleValueSql.Append("insert into " + tableValueName + "(");
                    roleValueSql.Append("roleId,menuId,typeNumber)");
                    roleValueSql.Append(" values (");
                    roleValueSql.Append("@roleId,@menuId,@typeNumber)");
                    MySqlParameter[] parameters2 = {
                            new MySqlParameter("@roleId", newId),
                            new MySqlParameter("@menuId",  valueModel.menuId),
                            new MySqlParameter("@typeNumber", valueModel.typeNumber)
                    };
                    cmd = new CommandInfo(roleValueSql.ToString(), parameters2);
                    sqllist.Add(cmd);
                }
            }
            int count = SqlHelper.ExecuteSqlTran(sqllist);
            return count > 0;
        }

        /// <summary>
        /// 更新角色是否可用
        /// </summary>
        public bool UpdateStatus(string roleId, int status)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("status =  " + status);
            strSql.Append(" ,modifyTime = now() ");
            strSql.Append(" where id=@id ");
            MySqlParameter[] parameters = { new MySqlParameter("@id", roleId) };

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
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.Role model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("name=@name,");
            strSql.Append("modifyTime=now(),");
            strSql.Append("typeId=@typeId");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@name", model.name),
                    new MySqlParameter("@typeId", model.type),
                    new MySqlParameter("@id", model.id)
            };
            List<CommandInfo> sqllist = new List<CommandInfo>();
            CommandInfo cmd = new CommandInfo(strSql.ToString(), parameters);
            sqllist.Add(cmd);

            //先删除该角色所有权限
            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("delete from " + tableValueName + " where roleId=@roleId ");
            MySqlParameter[] parameters2 = { new MySqlParameter("@roleId", model.id) };
            cmd = new CommandInfo(strSql2.ToString(), parameters2);
            sqllist.Add(cmd);

            //添加权限
            if (model.roleValues != null)
            {
                StringBuilder roleValueSql;
                foreach (Model.RoleValue modelt in model.roleValues)
                {
                    roleValueSql = new StringBuilder();
                    roleValueSql.Append("insert into " + tableValueName + "(");
                    roleValueSql.Append("roleId,menuId,typeNumber)");
                    roleValueSql.Append(" values (");
                    roleValueSql.Append("@roleId,@menuId,@typeNumber)");
                    MySqlParameter[] parameters3 = {
                            new MySqlParameter("@roleId", model.id),
                            new MySqlParameter("@menuId", modelt.menuId),
                            new MySqlParameter("@typeNumber", modelt.typeNumber)
                    };
                    cmd = new CommandInfo(roleValueSql.ToString(), parameters3);
                    sqllist.Add(cmd);
                }
            }

            int rowsAffected = SqlHelper.ExecuteSqlTran(sqllist);
            if (rowsAffected > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 获得查询SQL语句
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public StringBuilder GetSelectSql(string strWhere = "", string filedOrder = "")
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,name,typeId,isAdmin,");
            strSql.Append("status,isDelete,modifyTime ");
            strSql.Append(" FROM " + tableName);
            strSql.Append(" WHERE isDelete = 0 ");
            if (!string.IsNullOrEmpty(strWhere))
            {
                strSql.Append(strWhere);
            }
            if (!string.IsNullOrEmpty(filedOrder))
            {
                strSql.Append(filedOrder);
            }
            return strSql;
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = GetSelectSql(strWhere);
            string countSql = PagingHelper.CreateCountingSql(strSql.ToString());
            recordCount = Convert.ToInt32(SqlHelper.GetSingle(countSql));
            string pageSql = PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder);
            return SqlHelper.Query(pageSql);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = GetSelectSql(strWhere);
            strSql.Append(" order by id asc");
            return SqlHelper.Query(strSql.ToString());
        }

        /// <summary>
        /// 删除一条数据，及子表所有相关数据
        /// </summary>
        public bool Delete(string roleId)
        {
            List<CommandInfo> sqllist = new List<CommandInfo>();
            //删除管理角色权限
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + tableValueName);
            strSql.Append(" where roleId=@roleId");
            MySqlParameter[] parameters = { new MySqlParameter("@roleId", roleId) };
            CommandInfo cmd = new CommandInfo(strSql.ToString(), parameters);
            sqllist.Add(cmd);
            //删除管理角色
            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("delete from " + tableName);
            strSql2.Append(" where id=@id");
            MySqlParameter[] parameters2 = { new MySqlParameter("@id", roleId) };
            cmd = new CommandInfo(strSql2.ToString(), parameters2);
            sqllist.Add(cmd);

            int rowsAffected = SqlHelper.ExecuteSqlTran(sqllist);
            if (rowsAffected > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.Role GetModel(string roleId)
        {
            StringBuilder strSql = GetSelectSql(" and id=@id ");
            MySqlParameter[] parameters = { new MySqlParameter("@id", roleId) };

            Model.Role model = new Model.Role();
            DataSet ds = SqlHelper.Query(strSql.ToString(), parameters);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {

                DataRow dataRow = ds.Tables[0].Rows[0];
                #region 父表信息
                if (dataRow["id"].ToString() != "")
                {
                    model.id = dataRow["id"].ToString();
                }
                model.name = ds.Tables[0].Rows[0]["name"].ToString();
                if (dataRow["typeId"].ToString() != "")
                {
                    model.type = int.Parse(dataRow["typeId"].ToString());
                }
                if (dataRow["isAdmin"].ToString() != "")
                {
                    model.isAdmin = int.Parse(dataRow["isAdmin"].ToString());
                }

                if (dataRow["status"].ToString() != "")
                {
                    model.status = int.Parse(dataRow["status"].ToString());
                }

                if (dataRow["isDelete"].ToString() != "")
                {
                    model.isDelete = int.Parse(dataRow["isDelete"].ToString());
                }
                #endregion

                #region 子表信息

                StringBuilder strSql2 = new StringBuilder();
                strSql2.Append("select t1.roleId,t1.menuId,");
                strSql2.Append("t1.typeNumber,t2.module ");
                strSql2.Append(" from " + tableValueName);
                strSql2.Append(" t1 left join ci_menu t2 on t1.menuId = t2.id ");
                strSql2.Append(" where t1.roleId=@roleId order by t1.menuId ");
                MySqlParameter[] parameters2 = { new MySqlParameter("@roleId", roleId) };
                DataSet ds2 = SqlHelper.Query(strSql2.ToString(), parameters2);
                if (ds2.Tables[0].Rows.Count > 0)
                {
                    List<Model.RoleValue> models = new List<Model.RoleValue>();
                    Model.RoleValue values;
                    foreach (DataRow dr in ds2.Tables[0].Rows)
                    {
                        values = new Model.RoleValue();
                        values.roleId = dr["roleId"].ToString();
                        values.menuId = dr["menuId"].ToString();
                        values.typeNumber = dr["typeNumber"].ToString();
                        values.module = dr["module"].ToString();
                        models.Add(values);
                    }
                    model.roleValues = models;
                }
                #endregion

                return model;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 检查角色名称是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataSet CheckName(string name, string roleId = "")
        {

            StringBuilder strSql = GetSelectSql(" and name='" + name + "' ");
            if (!string.IsNullOrEmpty(roleId))
            {
                strSql.Append(" and id <> '" + roleId + "' ");
            }
            return SqlHelper.Query(strSql.ToString());
        }
    }
}
