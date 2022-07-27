using Common;
using DBUtility;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Business
{
    public class AdminBll
    {
        // 表名
        public string tableName = "ci_admin";

        #region Oracel数据库进行操作
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + tableName);
            strSql.Append(" where isDelete = 0");
            if (!string.IsNullOrEmpty(strWhere))
            {
                strSql.Append(" and " + strWhere);
            }
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
            strSql.Append("select t1.id,t1.userpwd,t1.username ,");
            strSql.Append("t1.status,t1.name,t1.mobile,");
            strSql.Append("t1.avatar,t1.roleId,t1.rightIds,t1.isAdmin,");
            strSql.Append("t1.deptId,t1.description,t1.gender,t1.nation,");
            strSql.Append("t1.nativeplace,t1.entryDate,t1.email,t1.certificatesType,");
            strSql.Append("t1.certificatesNumber,t1.education,t1.birthday,t1.telephone,t1.modifyTime,");
            strSql.Append("t1.landLine,t1.postalAddress,");
            strSql.Append("t1.CREATORTIME,t1.LOGINTIME,t1.LOGINIPADDRESS,t1.PREVLOGINTIME, t1.PREVLOGINIPADDRESS ,");
            strSql.Append("t4.fullName as deptName,");
            strSql.Append("t2.name as roleName,t2.typeId as roleType ");
            strSql.Append(" from ci_admin t1 left join ci_role ");
            strSql.Append(" t2 on t1.roleId = t2.id ");
            strSql.Append(" left join ci_dept t4 on t1.deptId = t4.id ");
            strSql.Append(" where t1.isDelete = 0");

            if (!string.IsNullOrEmpty(strWhere))
            {
                strSql.Append(strWhere);
            }
            return strSql;
        }

        /// <summary>
        /// 根据用户ID获得用户信息的实体类
        /// </summary>
        public Model.Admin GetUserModel(string userId)
        {
            StringBuilder strSql = GetSelectSql(" and t1.id=@id ");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id", MySqlDbType.VarChar)
            };
            parameters[0].Value = userId;
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
        /// 根据条件获得用户信息的实体类
        /// </summary>
        public Model.Admin GetModel(string strWhere)
        {
            StringBuilder strSql = GetSelectSql(strWhere);
            DataSet ds = SqlHelper.Query(strSql.ToString());
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
        /// 根据条件获得用户信息的实体类(用户登陆用)
        /// </summary>
        public Model.Admin GetLoginModel(string userName)
        {
            string strWhere = " and t1.username = @username ";
            StringBuilder strSql = GetSelectSql(strWhere);

            MySqlParameter[] parameters = {
                new MySqlParameter("@userName", MySqlDbType.VarChar)
            };
            parameters[0].Value = userName;
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
        /// 分页显示管理员信息
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="strWhere"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
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
            strSql.Append(" order by t1.id asc");
            return SqlHelper.Query(strSql.ToString());
        }

        /// <summary>
        /// 增加一条单位数据
        /// </summary>
        public string Add(Model.Admin model)
        {
            string id = Utils.GetNewGuid();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into " + tableName + " (");
            strSql.Append("id,username ,status,name,mobile,roleId,");
            strSql.Append("rightIds,userpwd,isDelete,CREATORTIME,isAdmin,deptId,description,gender,nation,nativeplace,entryDate,certificatesType,");
            strSql.Append("certificatesNumber,education,birthday,telePhone,landLine,email,postalAddress,avatar)");
            strSql.Append(" values (");
            strSql.Append("@id,@username ,1,@name,@mobile,@roleId,");
            strSql.Append("@rightIds,@userpwd,0,now(),@isAdmin,@deptId,@description,@gender,@nation,@nativeplace,@entryDate,@certificatesType,");
            strSql.Append("@certificatesNumber,@education,@birthday,@telePhone,@landLine,@email,@postalAddress,@avatar)");
            MySqlParameter[] parameters = {
                new MySqlParameter("@id", id),
                new MySqlParameter("@username", model.username),
                new MySqlParameter("@name", model.name),
                new MySqlParameter("@mobile", model.mobile),
                new MySqlParameter("@roleId",model.roleId),
                new MySqlParameter("@rightIds", model.rightIds),
                new MySqlParameter("@userpwd", model.userpwd),
                new MySqlParameter("@isAdmin",model.isAdmin),
                new MySqlParameter("@deptId",model.deptId),
                new MySqlParameter("@description", model.description),
                new MySqlParameter("@gender", model.gender),
                new MySqlParameter("@nation", model.nation),
                new MySqlParameter("@nativeplace",model.nativeplace),
                new MySqlParameter("@entryDate",model.entryDate),
                new MySqlParameter("@certificatesType",model.certificatesType),
                new MySqlParameter("@certificatesNumber", model.certificatesNumber),
                new MySqlParameter("@education", model.education),
                new MySqlParameter("@birthday", model.birthday),
                new MySqlParameter("@telephone", model.telephone),
                new MySqlParameter("@landLine", model.landLine),
                new MySqlParameter("@email",model.email),
                new MySqlParameter("@postalAddress", model.postalAddress),
                 new MySqlParameter("@avatar", model.avatar)
            };
            int rows = SqlHelper.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return id;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 修改个人资料(不包括修改用户角色)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateLoginUser(Model.Admin model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("modifyTime =now(),description=@description,gender=@gender,");
            strSql.Append("nation =@nation,nativeplace=@nativeplace,entryDate=@entryDate,");
            strSql.Append("email =@email,certificatesType=@certificatesType,certificatesNumber=@certificatesNumber,");
            strSql.Append("education =@education,birthday=@birthday,telephone=@telephone,landLine=@landLine,postalAddress=@postalAddress,modifyUserId=@modifyUserId");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters = {
                new MySqlParameter("@description",  model.description),
                new MySqlParameter("@gender", model.gender),
                new MySqlParameter("@nation",  model.nation),
                new MySqlParameter("@nativeplace", model.nativeplace),
                new MySqlParameter("@entryDate",  model.entryDate),
                new MySqlParameter("@email", model.email),
                new MySqlParameter("@certificatesType",  model.certificatesType),
                new MySqlParameter("@certificatesNumber", model.certificatesNumber),
                new MySqlParameter("@education",  model.education),
                new MySqlParameter("@birthday", model.birthday),
                new MySqlParameter("@telephone",  model.telephone),
                new MySqlParameter("@landLine", model.landLine),
                new MySqlParameter("@postalAddress",  model.postalAddress),
                new MySqlParameter("@modifyUserId",  model.modifyUserId),
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
        /// 修改用户密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateUserPassword(Model.Admin model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("userpwd =@userpwd,modifyTime=now() ");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters = {
                new MySqlParameter("@userpwd", MySqlDbType.VarChar),
                new MySqlParameter("@id", MySqlDbType.VarChar)
            };

            parameters[0].Value = model.userpwd;
            parameters[1].Value = model.id;
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
        /// 修改管理员信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateUserinfo(Model.Admin model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("name=@name,mobile=@mobile,");
            strSql.Append("modifyTime =now(),roleId=@roleId,isAdmin=@isAdmin,");
            strSql.Append("deptId =@deptId,description=@description,gender=@gender,");
            strSql.Append("nation =@nation,nativeplace=@nativeplace,entryDate=@entryDate,");
            strSql.Append("email =@email,certificatesType=@certificatesType,certificatesNumber=@certificatesNumber,");
            strSql.Append("education =@education,birthday=@birthday,telephone=@telephone,landLine=@landLine,postalAddress=@postalAddress,avatar=@avatar,modifyUserId=@modifyUserId");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters = {
                new MySqlParameter("@name", model.name),
                new MySqlParameter("@mobile",  model.mobile),
                new MySqlParameter("@roleId", model.roleId),
                new MySqlParameter("@isAdmin",  model.isAdmin),
                new MySqlParameter("@deptId", model.deptId),
                new MySqlParameter("@description",  model.description),
                new MySqlParameter("@gender", model.gender),
                new MySqlParameter("@nation",  model.nation),
                new MySqlParameter("@nativeplace", model.nativeplace),
                new MySqlParameter("@entryDate",  model.entryDate),
                new MySqlParameter("@email", model.email),
                new MySqlParameter("@certificatesType",  model.certificatesType),
                new MySqlParameter("@certificatesNumber", model.certificatesNumber),
                new MySqlParameter("@education",  model.education),
                new MySqlParameter("@birthday", model.birthday),
                new MySqlParameter("@telephone",  model.telephone),
                new MySqlParameter("@landLine", model.landLine),
                new MySqlParameter("@postalAddress",  model.postalAddress),
                new MySqlParameter("@avatar",  model.avatar),
                new MySqlParameter("@modifyUserId",  model.modifyUserId),
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
        /// 更新用户登录时间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateLoginTime(string userId, string IPAddress)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("UPDATE CI_ADMIN SET PREVLOGINTIME = LOGINTIME, LOGINTIME = now(),  ");
            strSql.Append("PREVLOGINIPADDRESS = LOGINIPADDRESS, LOGINIPADDRESS = @LOGINIPADDRESS ");
            strSql.Append(" WHERE ID=@ID");
            MySqlParameter[] parameters = {
                 new MySqlParameter("@LOGINIPADDRESS", MySqlDbType.VarChar),
                new MySqlParameter("@ID", MySqlDbType.VarChar)
            };
            parameters[0].Value = IPAddress;
            parameters[1].Value = userId;
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
        /// 修改用户头像
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool UpdateUserAvatar(string userId, string filePath)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("UPDATE CI_ADMIN SET AVATAR = @AVATAR ");
            strSql.Append(" WHERE ID=@ID");
            MySqlParameter[] parameters = {
                 new MySqlParameter("@AVATAR", MySqlDbType.VarChar),
                new MySqlParameter("@ID", MySqlDbType.VarChar)
            };
            parameters[0].Value = filePath;
            parameters[1].Value = userId;
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
        /// 删除一条数据（此删除只是更新字段，使其不再显示，并没有真正的删除）
        /// </summary>
        public bool Delete(Model.Admin model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("isDelete = 1 ,deleteTime = now(), deleteUserId = @deleteUserId");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters = {
                 new MySqlParameter("@deleteUserId", model.deleteUserId),
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
        /// 更新表启用状态字段（status）
        /// </summary>
        public bool Disable(string id, int status)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("status =@status ,modifyTime = now()");
            strSql.Append(" where id=@id");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@status",status),
                    new MySqlParameter("@id", id)
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
        /// 得到一个用户信息实体类
        /// </summary>
        public Model.Admin DataRowToModel(DataRow row)
        {
            Model.Admin model = new Model.Admin();
            if (row != null)
            {
                if (row["id"] != null && row["id"].ToString() != "")
                {
                    model.id = row["id"].ToString();
                }
                if (row["username"] != null)
                {
                    model.username = row["username"].ToString();
                }
                if (row["userpwd"] != null)
                {
                    model.userpwd = row["userpwd"].ToString();
                }
                if (row["status"] != null && row["status"].ToString() != "")
                {
                    model.status = int.Parse(row["status"].ToString());
                }
                if (row["name"] != null)
                {
                    model.name = row["name"].ToString();
                }
                if (row["mobile"] != null)
                {
                    model.mobile = row["mobile"].ToString();
                }
                if (row["roleId"] != null && row["roleId"].ToString() != "")
                {
                    model.roleId = row["roleId"].ToString();
                }
                if (row["avatar"] != null)
                {
                    model.avatar = row["avatar"].ToString();
                }
                if (row["roleName"] != null)
                {
                    model.roleName = row["roleName"].ToString();
                }
                if (row["roleType"] != null && row["roleType"].ToString() != "")
                {
                    model.roleType = int.Parse(row["roleType"].ToString());
                }

                if (row["CREATORTIME"] != null && row["CREATORTIME"].ToString() != "")
                {
                    model.creatorTime = row["CREATORTIME"].ToString();
                }
                if (row["LOGINTIME"] != null && row["LOGINTIME"].ToString() != "")
                {
                    model.loginTime = row["LOGINTIME"].ToString();
                }
                if (row["LOGINIPADDRESS"] != null && row["LOGINIPADDRESS"].ToString() != "")
                {
                    model.loginIPAddress = row["LOGINIPADDRESS"].ToString();
                }

                if (row["PREVLOGINTIME"] != null && row["PREVLOGINTIME"].ToString() != "")
                {
                    model.prevLoginTime = row["PREVLOGINTIME"].ToString();
                }

                if (row["PREVLOGINIPADDRESS"] != null && row["PREVLOGINIPADDRESS"].ToString() != "")
                {
                    model.prevLoginIPAddress = row["PREVLOGINIPADDRESS"].ToString();
                }
                if (row["deptId"] != null && row["deptId"].ToString() != "")
                {
                    model.deptId = row["deptId"].ToString();
                }
                if (row["deptName"] != null)
                {
                    model.deptName = row["deptName"].ToString();
                }
                if (row["description"] != null)
                {
                    model.description = row["description"].ToString();
                }
                if (row["gender"] != null && row["gender"].ToString() != "")
                {
                    model.gender = int.Parse(row["gender"].ToString());
                }
                if (row["nation"] != null && row["nation"].ToString() != "")
                {
                    model.nation = int.Parse(row["nation"].ToString());
                }
                if (row["nativeplace"] != null)
                {
                    model.nativeplace = row["nativeplace"].ToString();
                }
                if (row["entryDate"] != null)
                {
                    model.entryDate = row["entryDate"].ToString();
                }
                if (row["email"] != null)
                {
                    model.email = row["email"].ToString();
                }
                if (row["certificatesType"] != null)
                {
                    model.certificatesType = row["certificatesType"].ToString();
                }
                if (row["certificatesNumber"] != null)
                {
                    model.certificatesNumber = row["certificatesNumber"].ToString();
                }
                if (row["education"] != null && row["education"].ToString() != "")
                {
                    model.education = int.Parse(row["education"].ToString());
                }
                if (row["birthday"] != null)
                {
                    model.birthday = row["birthday"].ToString();
                }
                if (row["telephone"] != null)
                {
                    model.telephone = row["telephone"].ToString();
                }
                if (row["landLine"] != null)
                {
                    model.landLine = row["landLine"].ToString();
                }
                if (row["postalAddress"] != null)
                {
                    model.postalAddress = row["postalAddress"].ToString();
                }
            }
            return model;
        }

        #endregion
        
    }
}
