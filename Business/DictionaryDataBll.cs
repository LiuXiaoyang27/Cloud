using Common;
using DBUtility;
using Model;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text;

namespace Business
{
    public class DictionaryDataBll
    {
        // 表名
        public string tableName = "CI_DICTIONARY_DATA";
        /// <summary>
        /// 数据是否存在
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public bool IsExist(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + tableName + " t1 ");
            strSql.Append(" where t1.ISDELETE <> 1 and " + strWhere);
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
            strSql.Append("SELECT t1.*,t2.FULLNAME AS DictionaryDataName FROM " + tableName + " t1  ");
            strSql.Append(" LEFT JOIN CI_DICTIONARY_TYPE t2 ON t1.TYPEID = t2.ID WHERE t1.ISDELETE <> 1 ");
            if (!string.IsNullOrEmpty(strWhere))
            {
                strSql.Append(strWhere);
            }
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
            strSql.Append("SELECT * FROM " + tableName + " WHERE ISDELETE <> 1 and ID=@ID ");
            MySqlParameter[] parameters = { new MySqlParameter("@ID", id) };
           return SqlHelper.Query(strSql.ToString(), parameters);
        }
        /// <summary>
        /// 根据类别ID获得类别信息的实体类
        /// </summary>
        public Model.DictionaryData GetModel(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM " + tableName + " WHERE ISDELETE <> 1 and ID=@ID ");
            MySqlParameter[] parameters = { new MySqlParameter("@ID", id) };
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
        public int Delete( DictionaryData entity)
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
        public int Create(DictionaryData entity)
        {
            string id = Utils.GetNewGuid();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into " + tableName + " (");
            strSql.Append("ID,PARENTID,FULLNAME,ENCODE,SIMPLESPELLING,ENABLEDMARK,DESCRIPTION,ISDELETE,CREATORTIME,CREATORUSERID,TYPEID,SORTCODE");
            strSql.Append(") ");
            strSql.Append("values (");
            strSql.Append("@ID,@PARENTID,@FULLNAME,@ENCODE,@SIMPLESPELLING,@ENABLEDMARK,@DESCRIPTION,@ISDELETE,now(),@CREATORUSERID,@TYPEID,");

            strSql.Append("(SELECT SORTCODE FROM(SELECT IFNULL(MAX(SORTCODE) ,0) + 1 as SORTCODE FROM " + tableName + " WHERE TYPEID = '" + entity.TYPEID + "') t1 )");
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
                new MySqlParameter("@TypeId", entity.TYPEID)
            };
            return SqlHelper.ExecuteSql(strSql.ToString(), parameters);
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public int Update(DictionaryData entity)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("PARENTID=@PARENTID,FULLNAME=@FULLNAME,ENCODE=@ENCODE,SIMPLESPELLING=@SIMPLESPELLING,ENABLEDMARK=@ENABLEDMARK,DESCRIPTION=@DESCRIPTION,");
            strSql.Append("LASTMODIFYTIME=now(),LASTMODIFYUSERID=@LASTMODIFYUSERID,TYPEID=@TYPEID ");
            strSql.Append(" where ID = @ID");
            MySqlParameter[] parameters = {
                new MySqlParameter("@ParentId", entity.PARENTID),
                new MySqlParameter("@FullName", entity.FULLNAME),
                new MySqlParameter("@EnCode", entity.ENCODE),
                new MySqlParameter("@SimpleSpelling", entity.SIMPLESPELLING),
                new MySqlParameter("@EnabledMark", entity.ENABLEDMARK),
                new MySqlParameter("@Description", entity.DESCRIPTION),
                new MySqlParameter("@LastModifyUserId", entity.LASTMODIFYUSERID),
                new MySqlParameter("@TypeId", entity.TYPEID),
                new MySqlParameter("@ID", entity.ID)
            };
            return SqlHelper.ExecuteSql(strSql.ToString(), parameters);
        }
        /// <summary>
        /// 上移
        /// </summary>
        /// <param name="id">主键值</param>
        public int First(DictionaryData entity)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("SORTCODE = SORTCODE-1,LASTMODIFYTIME=now(),LASTMODIFYUSERID=@LASTMODIFYUSERID ");
            strSql.Append(" where ID = @ID");
            MySqlParameter[] parameters = {
                new MySqlParameter("@LastModifyUserId", entity.LASTMODIFYUSERID),
                 new MySqlParameter("@ID", entity.ID)
            };
            return SqlHelper.ExecuteSql(strSql.ToString(), parameters);
        }
        /// <summary>
        /// 下移
        /// </summary>
        /// <param name="id">主键值</param>
        public int Next(DictionaryData entity)
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
        public Model.DictionaryData DataRowToModel(DataRow row)
        {
            Model.DictionaryData model = new Model.DictionaryData();
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
                if (row["TYPEID"] != null && row["TYPEID"].ToString() != "")
                {
                    model.TYPEID = row["TYPEID"].ToString();
                }

            }
            return model;
        }
    }
}
