using Common;
using DBUtility;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text;

namespace Business
{
    public class MenuBll
    {
        // 辅助资料频道表名
        public string tableName = "ci_menu";

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + tableName);
            strSql.Append(" where isDelete=0 and " + strWhere);
            return SqlHelper.Exists(strSql.ToString());
        }

        /// <summary>
        /// 获得查询SQL语句
        /// </summary>
        /// <param name="strWhere">查询条件sql</param>
        /// <returns>SQL语句</returns>
        public StringBuilder GetSelectSql(string strWhere = "")
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,name,parentId,cLevel,fontIcon,");
            strSql.Append("module,status,ordnum,isDelete,linkUrl,linkTarget,");
            strSql.Append("typeNumber,remark,modifyTime,navType");
            strSql.Append(" from " + tableName);
            strSql.Append(" where isDelete=0");
            if (!string.IsNullOrEmpty(strWhere))
            {
                strSql.Append(strWhere);
            }
            return strSql;
        }
        /// <summary>
        /// 根据ID获得频道信息的实体类
        /// </summary>
        public Model.Menu GetModel(string menuId)
        {
            StringBuilder strSql = GetSelectSql(" and id=@id ");
            MySqlParameter[] parameters = { new MySqlParameter("@id", menuId) };

            Model.Menu model = new Model.Menu();
            DataSet ds = SqlHelper.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = GetSelectSql(strWhere);
            strSql.Append(" order by ordNum asc");
            return SqlHelper.Query(strSql.ToString());
        }

        /// <summary>
        /// 获得角色的菜单信息
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public DataSet GetRoleList(string roleId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select DISTINCT t2.id,t2.name,t2.parentId,");
            strSql.Append("t2.cLevel,t2.fontIcon,t2.module,");
            strSql.Append("t2.status,t2.ordnum,t2.isDelete,t2.linkUrl,t2.navType,");
            strSql.Append("t2.typeNumber,t2.remark,t2.modifyTime,t2.linkTarget ");
            strSql.Append(" from ci_role_value t1");
            strSql.Append(" LEFT JOIN ci_menu t2 on t1.menuId = t2.id ");
            strSql.Append(" WHERE t1.roleId = @roleId and ");
            strSql.Append(" t2.status =1 and t2.isDelete = 0");
            strSql.Append(" ORDER BY t2.id DESC");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@roleId", roleId)};
            return SqlHelper.Query(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 增加一条频道数据
        /// </summary>
        public string Add(Model.Menu model)
        {
            // 获得索引ID
            string id = Utils.GetNewGuid();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into " + tableName + " (");
            strSql.Append("id,name,cLevel,ordnum,");
            strSql.Append("typeNumber,remark,parentId,modifyTime,");
            strSql.Append("module,linkUrl,navType,fontIcon,status,LinkTarget,isDelete )");
            strSql.Append(" values (");
            strSql.Append("@id,@name,@cLevel,@ordnum,");
            strSql.Append("@typeNumber,@remark,@parentId,now(),");
            strSql.Append("@module,@linkUrl,@navType,@fontIcon,@status,@LinkTarget,0)");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@id",id),
                    new MySqlParameter("@name",model.name),
                    new MySqlParameter("@cLevel", model.level),
                    new MySqlParameter("@ordnum",model.ordnum),
                    new MySqlParameter("@typeNumber", model.typeNumber),
                    new MySqlParameter("@remark", model.remark),
                    new MySqlParameter("@parentId",model.parentId),
                    new MySqlParameter("@module",  model.module),
                    new MySqlParameter("@linkUrl", model.linkUrl),
                    new MySqlParameter("@navType", model.navType),
                    new MySqlParameter("@fontIcon", model.fontIcon),
                    new MySqlParameter("@status", model.status),
                    new MySqlParameter("@LinkTarget", model.LinkTarget)
            };
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
        public bool Update(Model.Menu model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("name=@name, ");
            strSql.Append("parentId=@parentId, ");
            strSql.Append("cLevel=@cLevel, ");
            strSql.Append("modifyTime=now(), ");
            strSql.Append("module=@module, ");
            strSql.Append("linkUrl=@linkUrl, ");
            strSql.Append("LinkTarget=@LinkTarget, ");
            strSql.Append("navType=@navType, ");
            strSql.Append("fontIcon=@fontIcon, ");
            strSql.Append("typeNumber=@typeNumber, ");
            strSql.Append("ordnum=@ordnum, ");
            strSql.Append("remark=@remark ");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters = {
                new MySqlParameter("@name",model.name),
                new MySqlParameter("@parentId",model.parentId),
                new MySqlParameter("@cLevel", model.level),
                new MySqlParameter("@module",  model.module),
                new MySqlParameter("@linkUrl", model.linkUrl),
                new MySqlParameter("@LinkTarget", model.LinkTarget),
                new MySqlParameter("@navType", model.navType),
                new MySqlParameter("@fontIcon", model.fontIcon),
                new MySqlParameter("@typeNumber", model.typeNumber),
                new MySqlParameter("@ordnum",model.ordnum),
                new MySqlParameter("@remark", model.remark),
                new MySqlParameter("@id", model.id)
            };

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
        /// 检查调用别名是否存在
        /// </summary>
        /// <param name="module"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataSet CheckModule(string module, string menuId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,name,module,status,isDelete");
            strSql.Append(" FROM " + tableName);
            strSql.Append(" where isDelete = 0 and module='" + module + "' ");
            if (string.IsNullOrEmpty(menuId))
            {
                strSql.Append(" and id <> '" + menuId + "'");
            }
            strSql.Append(" order by id DESC");
            return SqlHelper.Query(strSql.ToString());
        }

        /// <summary>
        /// 删除一条数据（此删除只是更新字段，使其不再显示，并没有真正的删除）
        /// </summary>
        public bool Delete(string menuId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("isDelete = 1 ");
            strSql.Append(" ,modifyTime = now() ");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters = { new MySqlParameter("@id", menuId) };

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
        /// 更新频道是否可用
        /// </summary>
        public bool UpdateStatus(string menuId, int status)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("status =  " + status);
            strSql.Append(" ,modifyTime = now() ");
            strSql.Append(" where id=@id ");
            MySqlParameter[] parameters = { new MySqlParameter("@id", menuId) };

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
        /// 得到一个对象实体
        /// </summary>
        public Model.Menu DataRowToModel(DataRow row)
        {
            Model.Menu model = new Model.Menu();
            if (row != null)
            {
                if (row["ID"] != null && row["ID"].ToString() != "")
                {
                    model.id = row["Id"].ToString();
                }
                if (row["NAME"] != null)
                {
                    model.name = row["NAME"].ToString();
                }
                if (row["ParentId"] != null && row["ParentId"].ToString() != "")
                {
                    model.parentId = row["ParentId"].ToString();
                }
                if (row["cLevel"] != null && row["cLevel"].ToString() != "")
                {
                    model.level = int.Parse(row["cLevel"].ToString());
                }
                if (row["fontIcon"] != null)
                {
                    model.fontIcon = row["fontIcon"].ToString();
                }
                if (row["module"] != null)
                {
                    model.module = row["module"].ToString();
                }
                if (row["status"] != null && row["status"].ToString() != "")
                {
                    model.status = int.Parse(row["status"].ToString());
                }
                if (row["ordnum"] != null && row["ordnum"].ToString() != "")
                {
                    model.ordnum = int.Parse(row["ordnum"].ToString());
                }
                if (row["linkUrl"] != null)
                {
                    model.linkUrl = row["linkUrl"].ToString();
                }
                if (row["navType"] != null)
                {
                    model.navType = row["navType"].ToString();
                }
                if (row["typeNumber"] != null)
                {
                    model.typeNumber = row["typeNumber"].ToString();
                }
                if (row["remark"] != null)
                {
                    model.remark = row["remark"].ToString();
                }
                if (row["navType"] != null)
                {
                    model.navType = row["navType"].ToString();
                }
                if (row["LinkTarget"] != null)
                {
                    model.LinkTarget = row["LinkTarget"].ToString();
                }
                if (row["modifyTime"] != null && row["modifyTime"].ToString() != "")
                {
                    model.modifyTime = DateTime.Parse(row["modifyTime"].ToString());
                }
            }
            return model;
        }

    }
}
