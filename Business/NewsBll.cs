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
    public class NewsBll
    {
        // 通知公告表名
        public string tableName = "ci_news";

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

            strSql.Append("select t1.id,t1.title,t1.detail,");
            strSql.Append(" t1.newsDate,t1.authorId,t1.creatorAvatar,t1.status,t1.newsType,");
            strSql.Append("t1.modifyTime,t2.name as author");
            strSql.Append(" from " + tableName + " t1");
            strSql.Append(" left join ci_admin t2 on t1.authorId=t2.id");
            strSql.Append(" where t1.isDelete = 0 ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(strWhere);
            }

            return strSql;
        }
        #endregion

        #region ADD@增加培训记录
        /// <summary>
        /// 增加培训记录
        /// </summary>
        /// <param name="Model">实体数据</param>
        /// <returns>true 插入成功， false 插入失败</returns>
        public string Add(News Model)
        {
            // 获得索引ID
            string id = Utils.GetNewGuid();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into " + tableName + " (");
            strSql.Append("id,title,authorid,detail,");
            strSql.Append("modifytime,status,isdelete,newsType,creatorAvatar,newsDate)");
            strSql.Append(" values (");
            strSql.Append("@id,@title,@authorid,@detail,");
            strSql.Append("now(),@status,0,@newsType,@creatorAvatar,@newsDate)");

            MySqlParameter[] parameters = {
                new MySqlParameter("@id", id),
               new MySqlParameter("@title", Model.title),
               new MySqlParameter("@authorid", Model.authorId),
               new MySqlParameter("@detail", Model.content),
               new MySqlParameter("@status",Model.status),
               new MySqlParameter("@newsType",Model.newsType),
               new MySqlParameter("@creatorAvatar",Model.creatorAvatar),
               new MySqlParameter("@newsDate",Model.newsDate)
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

        #region@GetModel@根据id获得通知公告的实体类
        /// <summary>
        /// 根据id获得通知公告的实体类
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public News GetModel(string id)
        {
            StringBuilder strSql = GetSelectSql();
            strSql.Append(" and t1.id=@id");
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


        /// <summary>
        /// 获得最新的10条新闻信息
        /// </summary>
        /// <returns></returns>
        public DataSet GetLatest()
        {
            string strWhere = "";

            strWhere = " and t1.status=1 and t1.newsType=2 ";


            StringBuilder strSql = GetSelectSql(strWhere);

            strSql.Append(" order by t1.newsdate DESC");

            strSql.Append(" limit 0, 10");

            return SqlHelper.Query(strSql.ToString());
        }
        /// <summary>
        /// 获得通知信息
        /// </summary>
        /// <returns></returns>
        public DataSet GetMessage(string userId)
        {
            string strWhere = " and t1.userId='" + userId + "'";

            StringBuilder strSql = GetSendMsgSql(strWhere);

            strSql.Append(" order by t1.CREATORTIME DESC");

            strSql.Append(" limit 0, 7");

            return SqlHelper.Query(strSql.ToString());
        }
        public StringBuilder GetSendMsgSql(string strWhere = "")
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select t1.id,t1.userId,t1.newsId,t1.isRead,t1.isClosed,t1.creatorAvatar,t2.title,t2.detail,");
            strSql.Append(" t2.newsDate,t2.authorId,t2.status,t2.newsType,");
            strSql.Append("t2.modifyTime,t3.name as author");
            strSql.Append(" from ci_send_message t1");
            strSql.Append(" left join ci_news t2 on t1.newsId=t2.id");
            strSql.Append(" left join ci_admin t3 on t2.authorId=t3.id");
            strSql.Append(" where t1.isRead = 0 ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(strWhere);
            }

            return strSql;
        }

        #region Update@更新一条数据
        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public bool Update(News model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("title =@title,");
            strSql.Append("detail=@detail,");
            strSql.Append("newsType=@newsType,");
            strSql.Append("status=@status,");
            strSql.Append("newsDate=@newsDate,");
            strSql.Append("modifytime=now(),");
            strSql.Append("authorid=@authorid, ");
            strSql.Append("creatorAvatar=@creatorAvatar ");
            strSql.Append(" where id=@id");

            MySqlParameter[] parameters = {
                    new MySqlParameter("@title", model.title),
                    new MySqlParameter("@detail", model.content),
                    new MySqlParameter("@newsType", model.newsType),
                    new MySqlParameter("@status", model.status),
                    new MySqlParameter("@newsDate", model.newsDate),
                    new MySqlParameter("@authorid", model.authorId),
                    new MySqlParameter("@creatorAvatar", model.creatorAvatar),
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

        #region Update@更新IsClosed
        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public bool UpdateIsClosed(string userId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql = new StringBuilder();
            strSql.Append("update ci_send_message set isClosed=0");
            strSql.Append(" where userId='" + userId + "'");
            int rows = SqlHelper.ExecuteSql(strSql.ToString());
            if (rows >= 0)
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
            strSql.Append("isdelete = 1 ");
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

        public bool DeleteMsg(string newsIds)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from ci_send_message ");
            strSql.Append(" where newsId in (");
            strSql.Append(newsIds + ")");
            string sql = strSql.ToString();
            int rows = SqlHelper.ExecuteSql(strSql.ToString());
            if (rows >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region 根据条件查询通知公告信息
        /// <summary>
        /// 根据条件查询通知公告信息
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<News> GetListModel(string strWhere)
        {

            List<News> list = new List<News>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select t1.id,t1.title,t1.detail,t1.newsdate,");
            strSql.Append("t1.authorid,t1.status,t1.modifytime,t1.newsType,t1.creatorAvatar,");
            strSql.Append("t2.name as author from " + tableName + " t1");
            strSql.Append(" left join ci_admin t2 on t1.authorid=t2.id");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by t1.id desc");
            DataSet dataset = SqlHelper.Query(strSql.ToString());
            News news;
            for (int i = 0; i < dataset.Tables[0].Rows.Count; i++)
            {
                news = DataRowToModel(dataset.Tables[0].Rows[i]);
                list.Add(news);
            }

            return list;
        }
        #endregion

        #region 更改状态
        /// <summary>
        /// 更改状态(发布)
        /// </summary>
        public bool Release(string id, int status, string time)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tableName + " set ");
            strSql.Append("status =@status, ");
            strSql.Append("newsDate =@time ");
            strSql.Append(" where id in(" + id + ")");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@status", status),
                    new MySqlParameter("@time", time)
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
        public bool UpdateMsgStatus(string id, string field, string val)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update ci_send_message set ");
            strSql.Append(field + "=" + val);
            strSql.Append(" where id ='" + id + "'");

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

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="listModel">数据集合</param>
        /// <returns>true 插入成功， false 插入失败</returns>
        public bool AddList(List<Message> listModel)
        {
            StringBuilder strSql;
            List<CommandInfo> sqlList = new List<CommandInfo>();
            CommandInfo cmd;
            foreach (Message Model in listModel)
            {
                // 获得索引ID
                string id = Utils.GetNewGuid();
                strSql = new StringBuilder();
                strSql.Append("insert into ci_send_message (");
                strSql.Append("id,userId,newsId,isRead,creatorTime,creatorUserId,creatorAvatar,isdelete,isClosed)");
                strSql.Append(" values (");
                strSql.Append("@id,@userId,@newsId,0,@creatorTime,@creatorUserId,@creatorAvatar,0,0)");

                MySqlParameter[] parameters = {
                   new MySqlParameter("@id", id),
                   new MySqlParameter("@userId", Model.userId),
                   new MySqlParameter("@newsId", Model.newsId),
                   new MySqlParameter("@creatorTime", Model.creatorTime),
                   new MySqlParameter("@creatorUserId", Model.creatorUserId),
                   new MySqlParameter("@creatorAvatar", Model.creatorAvatar)
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

        #region 得到一个对象实体
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public News DataRowToModel(DataRow row)
        {
            News model = new Model.News();
            if (row != null)
            {
                if (row["id"] != null)
                {
                    model.id = row["id"].ToString();
                }
                if (row["title"] != null)
                {
                    model.title = row["title"].ToString();
                }
                if (row["authorId"] != null)
                {
                    model.authorId = row["authorId"].ToString();
                }
                if (row["author"] != null)
                {
                    model.author = row["author"].ToString();
                }
                if (row["detail"] != null)
                {
                    model.content = row["detail"].ToString();
                }
                if (row["newsDate"] != null)
                {
                    model.newsDate = row["newsDate"].ToString();
                }
                if (row["modifyTime"] != null)
                {
                    model.modifyTime = row["modifyTime"].ToString();
                }
                if (row["status"] != null && row["status"].ToString() != "")
                {
                    model.status = int.Parse(row["status"].ToString());
                }
                if (row["newsType"] != null)
                {
                    model.newsType = row["newsType"].ToString();
                }
                if (row["creatorAvatar"] != null)
                {
                    model.creatorAvatar = row["creatorAvatar"].ToString();
                }
            }
            return model;
        }
        #endregion


    }
}
