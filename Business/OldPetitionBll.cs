using Common;
using DBUtility;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Business
{
    public class OldPetitionBll
    {
        // 表名
        public string tableName = "ci_old_petition";

        #region Exists:是否存在记录
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + tableName);
            strSql.Append(" where isDelete = 0 " + strWhere);
            return SqlHelper.Exists(strSql.ToString());
        }
        #endregion

        #region GetList@获得查询分页数据
        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        /// <param name="pageSize">页尺寸</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount">记录数</param>
        /// <returns>DataSet</returns>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = GetSelectSql(strWhere);
            string countSql = PagingHelper.CreateCountingSql(strSql.ToString());
            recordCount = Convert.ToInt32(SqlHelper.GetSingle(countSql));
            string pageSql = PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder);
            return SqlHelper.Query(pageSql);
        }

        #endregion

        #region GetList@获得查询分页数据
        /// <summary>
        /// 获得查询SQL语句 
        /// </summary>
        /// <param name="strWhere">查询条件sql</param>
        /// <returns>SQL语句</returns>
        public StringBuilder GetSelectSql(string strWhere = "")
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("select ID,FILENAME,FILETYPE,");
            strSql.Append(" FILESIZE,FILEPATH,FILESTATE,MODIFYTIME,TITLE,PNAME,PDATE,ATTENDNAME");
            strSql.Append(" from " + tableName);
            strSql.Append(" where ISDELETE = 0 ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(strWhere);
            }

            return strSql;
        }
        #endregion

        #region ADD@增加记录
        /// <summary>
        /// 增加案件信息
        /// </summary>
        /// <param name="Model">实体数据</param>
        /// <returns>true 插入成功， false 插入失败</returns>
        public string Add(OldPetition Model)
        {
            // 获得索引ID
            string id = Utils.GetNewGuid();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into " + tableName + " (");
            strSql.Append("id,FILENAME,FILETYPE,FILESIZE,FILEPATH,MODIFYTIME,FILESTATE,ISDELETE)");
            strSql.Append(" values (");
            strSql.Append("@id,@FILENAME,@FILETYPE,@FILESIZE,@FILEPATH,@MODIFYTIME,0,0)");

            MySqlParameter[] parameters = {
                new MySqlParameter("@id", id),
               new MySqlParameter("@FILENAME", Model.fileName),
               new MySqlParameter("@FILETYPE", Model.fileType),
               new MySqlParameter("@FILESIZE", Model.fileSize),
               new MySqlParameter("@FILEPATH", Model.filePath),       
               new MySqlParameter("@MODIFYTIME",Model.modifyTime),
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
        #endregion


        #region@GetModel@根据id获得案件信息的实体类
        /// <summary>
        /// 根据id获得案件信息的实体类
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public OldPetition GetModel(string id)
        {
            StringBuilder strSql = GetSelectSql();
            strSql.Append(" and id=@id");
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

        #endregion

        #region Update@更新一条数据
        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public bool Update(OldPetition model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("TITLE =@TITLE,");
            strSql.Append("PNAME=@PNAME,");
            strSql.Append("PDATE=@PDATE,");
            strSql.Append("ATTENDNAME=@ATTENDNAME");
            strSql.Append(" where id=@id");

            MySqlParameter[] parameters = {
                    new MySqlParameter("@TITLE", model.title),
                    new MySqlParameter("@pName", model.pName),
                    new MySqlParameter("@PDATE", model.pDate),
                    new MySqlParameter("@ATTENDNAME", model.attendName),
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

        #endregion

        #region Delete@删除数据（此删除只是更新字段，使其不再显示，并没有真正的删除）
        /// <summary>
        /// 删除数据（此删除只是更新字段，使其不再显示，并没有真正的删除）
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>true 删除成功， false 删除失败</returns>
        public bool Delete(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("isdelete = 1,modifytime=Now() ");
            strSql.Append(" where id in (");
            strSql.Append(id + ")");
            string sql = strSql.ToString();
            int rows = SqlHelper.ExecuteSql(strSql.ToString());
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion


        #region GetList:获得数据列表
        /// <summary>
        /// 获得数据列表
        /// <param name="strWhere">查询条件</param>
        /// <returns>DataSet</returns> 
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = GetSelectSql(strWhere);
            strSql.Append(" order by modifyTime desc");
            return SqlHelper.Query(strSql.ToString());
        }
        #endregion

        #region 得到一个对象实体
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public OldPetition DataRowToModel(DataRow row)
        {
            OldPetition model = new OldPetition();
            if (row != null)
            {
                if (row["ID"] != null)
                {
                    model.id = row["ID"].ToString();
                }
                if (row["FILENAME"] != null)
                {
                    model.fileName = row["FILENAME"].ToString();
                }
                if (row["FILETYPE"] != null)
                {
                    model.fileType = row["FILETYPE"].ToString();
                }
                if (row["FILESIZE"] != null && row["FILESIZE"].ToString() != "")
                {
                    model.fileSize = int.Parse(row["FILESIZE"].ToString());
                }
                if (row["FILEPATH"] != null)
                {
                    model.filePath = row["FILEPATH"].ToString();
                }
                if (row["FILESTATE"] != null && row["FILESTATE"].ToString() != "")
                {
                    model.fileState = row["FILESTATE"].ToString();
                }
                if (row["modifyTime"] != null)
                {
                    model.modifyTime = row["modifyTime"].ToString();
                }
                if (row["TITLE"] != null)
                {
                    model.title = row["TITLE"].ToString();
                }
                if (row["PNAME"] != null)
                {
                    model.pName = row["PNAME"].ToString();

                }
                if (row["PDATE"] != null)
                {
                    model.pDate = row["PDATE"].ToString();
                }
                if (row["ATTENDNAME"] != null)
                {
                    model.attendName = row["ATTENDNAME"].ToString();
                }
            }
            return model;
        }
        #endregion

    }
}
