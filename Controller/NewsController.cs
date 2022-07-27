using Business;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace Controller
{
    /// <summary>
    /// 通知公告操作类
    /// </summary>
    public class NewsController
    {
        private readonly NewsBll dal;

        public NewsController()
        {
            dal = new NewsBll();
        }


        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        /// <summary>
        /// 获得最新的10条公告信息
        /// </summary>
        /// <returns></returns>
        public DataSet GetLatest()
        {
            return dal.GetLatest();
        }

        public DataSet GetMessage(string userId)
        {
            return dal.GetMessage(userId);
        }

        /// <summary>
        /// 将DataRow转为JSON格式数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public JObject GetJsonObj(DataRow dataRow)
        {
            JObject item = new JObject();
            item["id"] = dataRow["id"].ToString();
            item["title"] = dataRow["title"].ToString();
            item["status"] = int.Parse(dataRow["status"].ToString());
            item["author"] = dataRow["author"].ToString();
            item["content"] = dataRow["detail"].ToString();
            item["newsDate"] = dataRow["newsDate"].ToString();

            item["modifyTime"] = dataRow["modifyTime"].ToString();
            item["newsType"] = dataRow["newsType"].ToString();
            return item;
        }
        public JObject GetJsonObj(News dataRow)
        {
            JObject item = new JObject();
            item["id"] = dataRow.id;
            item["title"] = dataRow.title;
            item["status"] = dataRow.status;
            item["author"] = dataRow.author;
            item["authorId"] = dataRow.authorId;
            item["newsDate"] = dataRow.newsDate;
            item["content"] = dataRow.content;
            item["modifyTime"] = dataRow.modifyTime.ToString();
            item["newsType"] = dataRow.newsType.ToString();
            return item;
        }
        /// <summary>
        /// 将DataTable转为JSON格式数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public JArray GetJsonList(DataTable dt)
        {
            JArray items = new JArray();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                items.Add(GetJsonObj(dt.Rows[i]));
            }
            return items;
        }

        /// <summary>
        /// 将DataTable转为JSON格式数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public JArray GetLatestJsonList(DataTable dt)
        {
            JArray items = new JArray();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                items.Add(GetLatestJsonObj(dt.Rows[i]));
            }
            return items;
        }
        /// <summary>
        /// 将DataTable转为JSON格式数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public JArray GetMessageJsonList(DataTable dt)
        {
            JArray items = new JArray();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                items.Add(GetMessageJsonObj(dt.Rows[i]));
            }
            return items;
        }
        /// <summary>
        /// 将DataRow转为JSON格式数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public JObject GetMessageJsonObj(DataRow dataRow)
        {
            JObject item = new JObject();
            item["id"] = dataRow["id"].ToString();
            item["userId"] = dataRow["userId"].ToString();
            item["newsId"] = dataRow["newsId"].ToString();
            item["isRead"] = dataRow["isRead"].ToString();
            item["isClosed"] = dataRow["isClosed"].ToString();
            item["creatorAvatar"] = dataRow["creatorAvatar"].ToString();
            item["title"] = dataRow["title"].ToString();
            item["status"] = int.Parse(dataRow["status"].ToString());
            item["author"] = dataRow["author"].ToString();
            item["content"] = dataRow["detail"].ToString();
            item["newsDate"] = dataRow["newsDate"].ToString();
            item["authorId"] = dataRow["authorId"].ToString();
            item["modifyTime"] = dataRow["modifyTime"].ToString();
            item["newsType"] = dataRow["newsType"].ToString();
            return item;
        }
        /// <summary>
        /// 将DataRow转为JSON格式数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public JObject GetLatestJsonObj(DataRow dataRow)
        {
            JObject item = new JObject();
            item["id"] = dataRow["id"].ToString();
            item["title"] = dataRow["title"].ToString();
            item["status"] = int.Parse(dataRow["status"].ToString());
            item["author"] = dataRow["author"].ToString();
            item["content"] = dataRow["detail"].ToString();
            item["newsDate"] = dataRow["newsDate"].ToString();
            item["authorId"] = dataRow["authorId"].ToString();
            item["modifyTime"] = dataRow["modifyTime"].ToString();
            item["newsType"] = dataRow["newsType"].ToString();
            return item;
        }

        /// <summary>
        /// 根据条件查询通知公告信息
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<News> GetListModel(string strWhere)
        {
            return dal.GetListModel(strWhere);
        }

        /// <summary>
        /// 更改状态
        /// </summary>
        public bool Release(string id, int status, string time)
        {
            return dal.Release(id, status, time);
        }

        /// <summary>
        /// 更改状态
        /// </summary>
        public bool UpdateMsgStatus(string id, string field, string val)
        {
            return dal.UpdateMsgStatus(id, field, val);
        }

        /// <summary>
        /// 更新msg isClosed字段
        /// </summary>
        /// <returns></returns>
        public bool UpdateIsClosed(string userId)
        {
            return dal.UpdateIsClosed(userId);
        }

        /// <summary>
        /// 新增培训记录
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public string Add(News Model)
        {
            return dal.Add(Model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(News model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 获得培训信息的实体类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public News GetModel(string id)
        {
            return dal.GetModel(id);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string id)
        {
            return dal.Delete(id);
        }

        public bool DeleteMsg(string newsid)
        {
            return dal.DeleteMsg(newsid);
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="listModel"></param>
        /// <returns></returns>
        public bool AddList(List<Message> listModel)
        {
            return dal.AddList(listModel);
        }

    }
}
