using Common;
using DBUtility;
using Model;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text;

namespace Business
{
    public class DeptBll
    {
        // 表名
        public string tableName = "CI_DEPT";
        /// <summary>
        /// 数据是否存在
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public bool IsExist(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + tableName + "");
            strSql.Append(" where ISDELETE <> 1 and " + strWhere);
            return SqlHelper.Exists(strSql.ToString());
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="strWhere">请求参数</param>
        /// <returns></returns>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM " + tableName + " WHERE ISDELETE <> 1 ");
            if (!string.IsNullOrEmpty(strWhere))
            {
                strSql.Append(strWhere);
            }
            strSql.Append(" ORDER BY ENCODE ");
            return SqlHelper.Query(strSql.ToString());
        }
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        public DataSet GetInfo(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from " + tableName + " where ISDELETE <> 1 and ID = '" + id + "'");
            return SqlHelper.Query(strSql.ToString());
        }
        /// <summary>
        /// 根据部门ID获得部门信息的实体类
        /// </summary>
        public Dept GetModel(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM " + tableName + " WHERE ISDELETE <> 1 and ID=@ID ");
            MySqlParameter[] parameters = { new MySqlParameter("@id", id) };
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
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        public int Delete(Dept entity)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ISDELETE = 1,DELETEUSERID = @DELETEUSERID,DELETETIME=now()");
            strSql.Append(" where ID = @ID");
            MySqlParameter[] parameters = {
                new MySqlParameter("@DELETEUSERID", entity.DELETEUSERID),
                new MySqlParameter("@ID", entity.ID)
            };
            return SqlHelper.ExecuteSql(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public int Create(Dept entity)
        {
            string id = Utils.GetNewGuid();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into " + tableName + " (");
            strSql.Append("ID,PARENTID,FULLNAME,ENCODE,SIMPLESPELLING,ENABLEDMARK,DESCRIPTION,ISDELETE,CREATORTIME,CREATORUSERID,MOBILE,TEL,SORTCODE");
            strSql.Append(") ");
            strSql.Append("values (");
            strSql.Append("@ID,@PARENTID,@FULLNAME,@ENCODE,@SIMPLESPELLING,@ENABLEDMARK,@DESCRIPTION,@ISDELETE,now(),@CREATORUSERID,@MOBILE,@TEL,");
            strSql.Append("(SELECT SORTCODE FROM(SELECT IFNULL(MAX(SORTCODE) ,0) + 1 as SORTCODE FROM " + tableName + ") t1 )");
            strSql.Append(")");
            MySqlParameter[] parameters = {
                new MySqlParameter("@ID", id),
                new MySqlParameter("@ParentId", entity.PARENTID),
                new MySqlParameter("@FullName", entity.FULLNAME),
                new MySqlParameter("@EnCode", entity.ENCODE),
                new MySqlParameter("@SimpleSpelling", entity.SIMPLESPELLING),
                new MySqlParameter("@EnabledMark", entity.ENABLEDMARK),
                new MySqlParameter("@Description", entity.DESCRIPTION),
                new MySqlParameter("@ISDELETE", entity.ISDELETE),
                new MySqlParameter("@CreatorUserId", entity.CREATORUSERID),
                new MySqlParameter("@MOBILE", entity.MOBILE),
                new MySqlParameter("@TEL", entity.TEL),
            };
            return SqlHelper.ExecuteSql(strSql.ToString(), parameters);
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public int Update(Dept entity)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("PARENTID=@PARENTID,FULLNAME=@FULLNAME,ENCODE=@ENCODE,SIMPLESPELLING=@SIMPLESPELLING,ENABLEDMARK=@ENABLEDMARK,DESCRIPTION=@DESCRIPTION,");
            strSql.Append("LASTMODIFYTIME=now(),LASTMODIFYUSERID=@LASTMODIFYUSERID,MOBILE=@MOBILE,TEL=@TEL");
            strSql.Append(" where ID = @ID");
            MySqlParameter[] parameters = {
                new MySqlParameter("@ParentId", entity.PARENTID),
                new MySqlParameter("@FullName", entity.FULLNAME),
                new MySqlParameter("@EnCode", entity.ENCODE),
                new MySqlParameter("@SimpleSpelling", entity.SIMPLESPELLING),
                new MySqlParameter("@EnabledMark", entity.ENABLEDMARK),
                new MySqlParameter("@Description", entity.DESCRIPTION),
                new MySqlParameter("@LastModifyUserId", entity.LASTMODIFYUSERID),
                new MySqlParameter("@MOBILE", entity.MOBILE),
                new MySqlParameter("@TEL", entity.TEL),
                new MySqlParameter("@ID", entity.ID)
            };
            return SqlHelper.ExecuteSql(strSql.ToString(), parameters);
        }
        /// <summary>
        /// 上移
        /// </summary>
        /// <param name="id">主键值</param>
        public int First(Dept entity)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("SORTCODE = SORTCODE-1,LASTMODIFYTIME=now(),LASTMODIFYUSERID=@LASTMODIFYUSERID ");
            strSql.Append(" where ID = @ID");
            MySqlParameter[] parameters = {
                new MySqlParameter("@LastModifyUserId", entity.LASTMODIFYUSERID),
                new MySqlParameter("@ID", entity.ID),
            };
            return SqlHelper.ExecuteSql(strSql.ToString(), parameters);
        }
        /// <summary>
        /// 下移
        /// </summary>
        /// <param name="id">主键值</param>
        public int Next(Dept entity)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("SORTCODE = SORTCODE+1,LASTMODIFYTIME=now(),LASTMODIFYUSERID=@LASTMODIFYUSERID  ");
            strSql.Append(" where ID = @ID");
            MySqlParameter[] parameters = {
                new MySqlParameter("@LastModifyUserId", entity.LASTMODIFYUSERID),
                new MySqlParameter("@ID", entity.ID)
            };
            return SqlHelper.ExecuteSql(strSql.ToString(), parameters);
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        /// <returns></returns>
        public Dept DataRowToModel(DataRow row)
        {
            Dept model = new Model.Dept();
            if (row != null)
            {
                if (row["ID"] != null && row["ID"].ToString() != "")
                {
                    model.ID = row["ID"].ToString();
                }
                if (row["PARENTID"] != null && row["PARENTID"].ToString() != "")
                {
                    model.PARENTID = row["PARENTID"].ToString();
                }
                if (row["FULLNAME"] != null)
                {
                    model.FULLNAME = row["FULLNAME"].ToString();
                }
                if (row["ENABLEDMARK"] != null && row["ENABLEDMARK"].ToString() != "")
                {
                    model.ENABLEDMARK = int.Parse(row["ENABLEDMARK"].ToString());
                }
                if (row["DESCRIPTION"] != null)
                {
                    model.DESCRIPTION = row["DESCRIPTION"].ToString();
                }
                if (row["ENCODE"] != null)
                {
                    model.ENCODE = row["ENCODE"].ToString();
                }
                if (row["SIMPLESPELLING"] != null)
                {
                    model.SIMPLESPELLING = row["SIMPLESPELLING"].ToString();
                }
                if (row["SORTCODE"] != null && row["SORTCODE"].ToString() != "")
                {
                    model.SORTCODE = int.Parse(row["SORTCODE"].ToString());
                }
                if (row["MOBILE"] != null)
                {
                    model.MOBILE = row["MOBILE"].ToString();
                }
                if (row["TEL"] != null)
                {
                    model.TEL = row["TEL"].ToString();
                }

            }
            return model;
        }
    }
}
