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

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList()
        {
            StringBuilder strSql = GetSelectSql();
            strSql.Append(" order by MODIFYTIME");
            return SqlHelper.Query(strSql.ToString());
        }

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

    }
}
