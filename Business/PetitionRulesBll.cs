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
    public class PetitionRulesBll
    {
        // 表名
        public string tableName = "ci_petition_rules";

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
            strSql.Append(" FILESIZE,FILEPATH,FILESTATE,MODIFYTIME");
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
        public string Add(PetitionRules Model)
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

        #region 增加多条信息
        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="listModel">数据集合</param>
        /// <returns>true 插入成功， false 插入失败</returns>
        public bool AddList(List<Petition> listModel)
        {
            StringBuilder strSql;
            List<CommandInfo> sqlList = new List<CommandInfo>();
            CommandInfo cmd;
            foreach (Petition Model in listModel)
            {
                // 获得索引ID
                string id = Utils.GetNewGuid();
                strSql = new StringBuilder();
                strSql.Append("insert into " + tableName + " (");
                strSql.Append("id,createDate,pName,pIdCard,pAddress,caseType,");
                strSql.Append("caseName,caseSource,channels,receiver,rerm,ext1,ext2,ext3,ext4,ext5,modifytime,status,isdelete)");
                strSql.Append(" values (");
                strSql.Append("@id,@createDate,@pName,@pIdCard,@pAddress,@caseType,@caseName,@caseSource,@channels,@receiver,@rerm,");
                strSql.Append("@ext1,@ext2,@ext3,@ext4,@ext5,@modifytime,0,0)");

                MySqlParameter[] parameters = {
                new MySqlParameter("@id", id),
               new MySqlParameter("@createDate", Model.createDate),
               new MySqlParameter("@pName", Model.pName),
               new MySqlParameter("@pIdCard", Model.pIdCard),
               new MySqlParameter("@pAddress", Model.pAddress),
               new MySqlParameter("@caseType",Model.caseType),
               new MySqlParameter("@caseName",Model.caseName),
               new MySqlParameter("@caseSource",Model.caseSource),
               new MySqlParameter("@channels",Model.channels),
               new MySqlParameter("@receiver",Model.receiver),
               new MySqlParameter("@rerm",Model.rerm),
               new MySqlParameter("@ext1",Model.ext1),
               new MySqlParameter("@ext2",Model.ext2),
               new MySqlParameter("@ext3",Model.ext3),
               new MySqlParameter("@ext4",Model.ext4),
               new MySqlParameter("@ext5",Model.ext5),
               new MySqlParameter("@modifytime",Model.modifyTime),
            };

                cmd = new CommandInfo(strSql.ToString(), parameters);
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
        #endregion

        #region@GetModel@根据id获得案件信息的实体类
        /// <summary>
        /// 根据id获得案件信息的实体类
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public Petition GetModel(string id)
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
        public bool Update(Petition model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("createDate =@createDate,");
            strSql.Append("pName=@pName,");
            strSql.Append("pIdCard=@pIdCard,");
            strSql.Append("pAddress=@pAddress,");
            strSql.Append("caseType=@caseType,");
            strSql.Append("caseName=@caseName,");
            strSql.Append("caseSource=@caseSource,");
            strSql.Append("channels=@channels,");
            strSql.Append("receiver=@receiver,");
            strSql.Append("rerm=@rerm,");
            strSql.Append("ext1=@ext1,");
            strSql.Append("ext2=@ext2,");
            strSql.Append("ext3=@ext3,");
            strSql.Append("ext4=@ext4,");
            strSql.Append("ext5=@ext5,");
            strSql.Append("modifytime=Now()");
            strSql.Append(" where id=@id");

            MySqlParameter[] parameters = {
                    new MySqlParameter("@createDate", model.createDate),
                    new MySqlParameter("@pName", model.pName),
                    new MySqlParameter("@pIdCard", model.pIdCard),
                    new MySqlParameter("@pAddress", model.pAddress),
                    new MySqlParameter("@caseType", model.caseType),
                    new MySqlParameter("@caseName", model.caseName),
                    new MySqlParameter("@caseSource", model.caseSource),
                    new MySqlParameter("@channels", model.channels),
                    new MySqlParameter("@receiver", model.receiver),
                    new MySqlParameter("@rerm", model.rerm),
                    new MySqlParameter("@ext1", model.ext1),
                    new MySqlParameter("@ext2", model.ext2),
                    new MySqlParameter("@ext3", model.ext3),
                    new MySqlParameter("@ext4", model.ext4),
                    new MySqlParameter("@ext5", model.ext5),
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

        #region 更改状态
        /// <summary>
        /// 更改状态
        /// </summary>
        public bool Release(string id, int status)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("status =@status ");
            strSql.Append(" where id in('" + id + "')");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@status", MySqlDbType.Int32,4)
            };
            parameters[0].Value = status;

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

        #region 得到一个对象实体
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Petition DataRowToModel(DataRow row)
        {
            Petition model = new Petition();
            if (row != null)
            {
                if (row["id"] != null)
                {
                    model.id = row["id"].ToString();
                }
                if (row["createDate"] != null)
                {
                    model.createDate = row["createDate"].ToString();
                }
                if (row["pName"] != null)
                {
                    model.pName = row["pName"].ToString();
                }
                if (row["pIdCard"] != null)
                {
                    model.pIdCard = row["pIdCard"].ToString();
                }
                if (row["pAddress"] != null)
                {
                    model.pAddress = row["pAddress"].ToString();
                }
                if (row["caseType"] != null)
                {
                    model.caseType = row["caseType"].ToString();
                }
                if (row["caseName"] != null)
                {
                    model.caseName = row["caseName"].ToString();
                }
                if (row["caseSource"] != null)
                {
                    model.caseSource = row["caseSource"].ToString();

                }
                if (row["channels"] != null)
                {
                    model.channels = row["channels"].ToString();
                }
                if (row["receiver"] != null)
                {
                    model.receiver = row["receiver"].ToString();
                }
                if (row["rerm"] != null && row["rerm"].ToString() != "")
                {
                    model.rerm = int.Parse(row["rerm"].ToString());
                }
                if (row["EXT1"] != null)
                {
                    model.ext1 = row["EXT1"].ToString();
                }
                if (row["EXT2"] != null)
                {
                    model.ext2 = row["EXT2"].ToString();
                }
                if (row["EXT3"] != null)
                {
                    model.ext3 = row["EXT3"].ToString();
                }
                if (row["EXT4"] != null)
                {
                    model.ext4 = row["EXT4"].ToString();
                }
                if (row["EXT5"] != null)
                {
                    model.ext5 = row["EXT5"].ToString();
                }
                if (row["modifyTime"] != null)
                {
                    model.modifyTime = row["modifyTime"].ToString();
                }
                if (row["status"] != null && row["status"].ToString() != "")
                {
                    model.status = int.Parse(row["status"].ToString());
                }
            }
            return model;
        }
        #endregion

    }
}
