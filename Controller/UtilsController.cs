using Business;
using Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    /// <summary>
    /// 日志操作类
    /// </summary>
    public class UtilsController
    {
        private readonly UtilsBll dal;

        public UtilsController()
        {
            dal = new UtilsBll();
        }

        public JObject GetIndexReport()
        {
            JObject result = new JObject();

            // 首页统计信息           
            DataTable dtTotal = dal.GetIndexReport().Tables[0];
            JObject jsonTotal = new JObject();
            jsonTotal["petition_count"] = dtTotal.Rows[0]["petition_count"].ToString();
            jsonTotal["finish_count"] = dtTotal.Rows[0]["finish_count"].ToString();
            jsonTotal["user_count"] = dtTotal.Rows[0]["user_count"].ToString();
            jsonTotal["notfinish_count"] = dtTotal.Rows[0]["notfinish_count"].ToString();
            jsonTotal["overdue_count"] = dtTotal.Rows[0]["overdue_count"].ToString();

            // 最新警告信息
            //DataTable dtWarn = dal.WarnIndexReport().Tables[0];
            //JArray jsonWarn = new JArray();
            //for (int i = 0; i < dtWarn.Rows.Count; i++)
            //{
            //    JObject obj = new JObject();
            //    obj["date"] = dtWarn.Rows[i]["LASTMODIFYTIME"].ToString();
            //    obj["vin"] = dtWarn.Rows[i]["VIN"].ToString();
            //    jsonWarn.Add(obj);
            //}

            //更新ci_send_message 表 isClosed 字段
            //new NewsController().UpdateIsClosed();

            // 最新通知信息
            DataTable dtNews = new NewsController().GetLatest().Tables[0];
            JArray jsonNews = new JArray();
            for (int i = 0; i < dtNews.Rows.Count; i++)
            {
                JObject item = new JObject();
                DataRow dataRow = dtNews.Rows[i];
                item["id"] = dataRow["id"].ToString();
                item["title"] = dataRow["title"].ToString();
                item["newsDate"] = dataRow["newsDate"].ToString();
                item["modifyTime"] = dataRow["modifyTime"].ToString();
                item["newsType"] = dataRow["newsType"].ToString();
                item["author"] = dataRow["author"].ToString();
                jsonNews.Add(item);
            }

            // 折线图数据
            DataTable dtAct = dal.ActIndexReport().Tables[0];

            JArray jsonAct = new JArray();

            DateTime startDate = DateTime.Parse(DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd"));
            DateTime endDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));

            while (startDate <= endDate)
            {
                JObject obj = new JObject();
                obj["name"] = startDate.ToString("yyyy-MM-dd");
                obj["value"] = GetActData(dtAct, startDate.ToString("yyyy-MM-dd"));
                startDate = startDate.AddDays(1);
                jsonAct.Add(obj);
            }

            result["total"] = jsonTotal;
            // result["warn"] = jsonWarn;
            result["news"] = jsonNews;
            result["act"] = jsonAct;
            return result;
        }

        public int GetActData(DataTable dtAct, string dateTime)
        {
            for (int i = 0; i < dtAct.Rows.Count; i++)
            {
                string date = dtAct.Rows[i]["time"].ToString();
                if (date.Equals(dateTime))
                {
                    return int.Parse(dtAct.Rows[i]["total"].ToString());
                }
                
            }
            return 0;
        }
    }
}

