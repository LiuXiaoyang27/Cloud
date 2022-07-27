using System;
using System.Data;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

namespace Common
{
    /// <summary>
    /// Json帮助类
    /// </summary>
    public static class JsonHelp
    {
        /// <summary>
        /// 成功状态
        /// </summary>
        public static int SUCCESS = 200;
        /// <summary>
        /// 失败状态
        /// </summary>
        public static int ERROR = -1;
        /// <summary>
        /// 空数据状态
        /// </summary>
        public static int NULL_DATA = 250;
        /// <summary>
        /// 未登录状态
        /// </summary>
        public static int NO_LOGIN = 0;
        /// <summary>
        /// DataTable转JSON
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="total">总页数</param>
        /// <param name="page">当前页</param>
        /// <param name="records">总数</param>
        /// <returns></returns>
        public static string DataToJson(DataTable dt, int total, int page, int records = 0)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");
            jsonBuilder.Append("\"status\":" + SUCCESS + ",\"msg\":\"success\",\"data\":{\"items\": [");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i > 0)
                {
                    jsonBuilder.Append(",");
                }
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (j > 0)
                    {
                        jsonBuilder.Append(",");
                    }
                    if (dt.Columns[j].DataType.Equals(typeof(DateTime)) && dt.Rows[i][j].ToString() != "")
                    {
                        jsonBuilder.Append("\"" + dt.Columns[j].ColumnName + "\": \"" + Convert.ToDateTime(dt.Rows[i][j].ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "\"");
                    }
                    else if (dt.Columns[j].DataType.Equals(typeof(String)))
                    {
                        jsonBuilder.Append("\"" + dt.Columns[j].ColumnName + "\": \"" + dt.Rows[i][j].ToString().Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\t", " ").Replace("\r", " ").Replace("\n", "<br/>") + "\"");
                    }
                    else
                    {
                        jsonBuilder.Append("\"" + dt.Columns[j].ColumnName + "\": \"" + dt.Rows[i][j].ToString() + "\"");
                    }
                }
                jsonBuilder.Append("}");
            }
            jsonBuilder.Append("]");
            jsonBuilder.Append(",\"page\":" + page + ",");
            if (records == 0)
            {
                jsonBuilder.Append("\"records\":" + dt.Rows.Count + ",");
            }
            else
            {
                jsonBuilder.Append("\"records\":" + records + ",");
            }

            jsonBuilder.Append("\"total\":" + total);
            jsonBuilder.Append("}}");
            return jsonBuilder.ToString();
        }

        /// <summary>
        /// DataTable转JSON
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="total">总页数</param>
        /// <param name="page">当前页</param>
        /// <param name="records">总数</param>
        /// <returns></returns>
        public static string SuccessJson(JArray items, int total = 0, int page = 0, int records = 0)
        {
            JObject data = new JObject();
            data["items"] = items;
            data["page"] = page;
            data["total"] = total;
            if (records == 0)
            {
                data["records"] = items.Count;
            }
            else
            {
                data["records"] = records;
            }
            JObject result = new JObject();
            result["status"] = SUCCESS;
            result["msg"] = "success";
            result["data"] = data;
            return result.ToString();
        }
        /// <summary>
        /// DataTable转JSON
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="total">总页数</param>
        /// <param name="page">当前页</param>
        /// <param name="records">总数</param>
        /// <returns></returns>
        public static string DataToJson(DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i > 0)
                {
                    jsonBuilder.Append(",");
                }
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (j > 0)
                    {
                        jsonBuilder.Append(",");
                    }
                    if (dt.Columns[j].DataType.Equals(typeof(DateTime)) && dt.Rows[i][j].ToString() != "")
                    {
                        jsonBuilder.Append("\"" + dt.Columns[j].ColumnName + "\": \"" + Convert.ToDateTime(dt.Rows[i][j].ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "\"");
                    }
                    else if (dt.Columns[j].DataType.Equals(typeof(String)))
                    {
                        jsonBuilder.Append("\"" + dt.Columns[j].ColumnName + "\": \"" + dt.Rows[i][j].ToString().Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\t", " ").Replace("\r", " ").Replace("\n", "<br/>") + "\"");
                    }
                    else
                    {
                        jsonBuilder.Append("\"" + dt.Columns[j].ColumnName + "\": \"" + dt.Rows[i][j].ToString() + "\"");
                    }
                }
                jsonBuilder.Append("}");
            }
            jsonBuilder.Append("]");
            return jsonBuilder.ToString();
        }
        /// <summary>
        /// 正确结果的JSON字符串
        /// </summary>
        /// <returns></returns>
        public static string SuccessJson()
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");
            jsonBuilder.Append("\"status\":" + SUCCESS + ",\"msg\":\"success\"}");
            return jsonBuilder.ToString();
        }

        /// <summary>
        /// 正确结果的JSON字符串
        /// </summary>
        /// <returns></returns>
        public static string SuccessJson(JObject obj)
        {
            JObject result = new JObject();
            result["status"] = SUCCESS;
            result["msg"] = "success";
            result["data"] = obj;

            return result.ToString();
        }
        /// <summary>
        /// 带错误信息的JSON字符串
        /// </summary>
        /// <returns></returns>
        public static string SuccessJson(int status, string successMsg, string errMsg)
        {
            JObject result = new JObject();
            result["status"] = status;
            result["msg"] = successMsg;
            result["errMsg"] = errMsg;

            return result.ToString();
        }
        /// <summary>
        /// 正确结果的JSON字符串
        /// </summary>
        /// <returns></returns>
        public static string SuccessJson(string msg)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");
            jsonBuilder.Append("\"status\":" + SUCCESS + ",\"msg\":\"" + msg + "\"}");
            return jsonBuilder.ToString();
        }
        /// <summary>
        /// 正确结果的JSON字符串
        /// </summary>
        /// <returns></returns>
        public static string SuccessJsonId(string id)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");
            jsonBuilder.Append("\"status\":" + SUCCESS + ",\"msg\":\"success\",\"data\":{\"id\":[" + id + "]}}");
            return jsonBuilder.ToString();
        }

        /// <summary>
        /// 正确结果的JSON字符串
        /// </summary>
        /// <returns></returns>
        public static string SuccessJsonIds(string[] ids)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            string id = string.Join(",", ids);
            jsonBuilder.Append("{");
            jsonBuilder.Append("\"status\":" + SUCCESS + ",\"msg\":\"success\",\"data\":{\"id\":[" + id + "]}}");
            return jsonBuilder.ToString();
        }

        /// <summary>
        /// 正确结果的JSON字符串
        /// </summary>
        /// <returns></returns>
        public static string SuccessJson(object obj)
        {
            string json = ObjectToJSON(obj);
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");
            jsonBuilder.Append("\"status\":" + SUCCESS + ",\"msg\":\"success\",\"data\":" + json + "}");
            return jsonBuilder.ToString();
        }


        /// <summary>
        /// 错误结果的JSON字符串
        /// </summary>
        /// <returns></returns>
        public static string ErrorJson()
        {
            return ErrorJson("操作失败，稍后再试");
        }
        /// <summary>
        /// 自定义输出错误结果JSON
        /// </summary>
        /// <returns></returns>
        public static string ErrorJson(string msg)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");
            jsonBuilder.Append("\"status\":" + ERROR + ",\"msg\":\"" + msg + "\"}");
            return jsonBuilder.ToString();
        }
        /// <summary>
        /// 返回空结果JSON
        /// </summary>
        /// <returns></returns>
        public static string NullJson()
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");
            jsonBuilder.Append("\"status\":" + NULL_DATA + ",\"msg\":\"无数据\"}");
            return jsonBuilder.ToString();
        }

        /// <summary> 
        /// 对象转JSON 
        /// </summary> 
        /// <param name="obj">对象</param> 
        /// <returns>JSON格式的字符串</returns> 
        public static string ObjectToJSON(object obj)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            try
            {
                byte[] b = Encoding.UTF8.GetBytes(jss.Serialize(obj));
                return Encoding.UTF8.GetString(b);
            }
            catch (Exception ex)
            {

                throw new Exception("JSONHelper.ObjectToJSON(): " + ex.Message);
            }
        }

        /// <summary> 
        /// 数据表转键值对集合
        /// 把DataTable转成 List集合, 存每一行 
        /// 集合中放的是键值对字典,存每一列 
        /// </summary> 
        /// <param name="dt">数据表</param> 
        /// <returns>哈希表数组</returns> 
        public static List<Dictionary<string, object>> DataTableToList(DataTable dt)
        {
            List<Dictionary<string, object>> list
                 = new List<Dictionary<string, object>>();

            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    dic.Add(dc.ColumnName, dr[dc.ColumnName]);
                }

                list.Add(dic);
            }
            return list;
        }

        /// <summary> 
        /// 数据集转键值对数组字典 
        /// </summary> 
        /// <param name="dataSet">数据集</param> 
        /// <returns>键值对数组字典</returns> 
        public static Dictionary<string, List<Dictionary<string, object>>> DataSetToDic(DataSet ds)
        {
            Dictionary<string, List<Dictionary<string, object>>> result = new Dictionary<string, List<Dictionary<string, object>>>();

            foreach (DataTable dt in ds.Tables)
                result.Add(dt.TableName, DataTableToList(dt));

            return result;
        }

        /// <summary> 
        /// 数据表转JSON 
        /// </summary> 
        /// <param name="dataTable">数据表</param> 
        /// <returns>JSON字符串</returns> 
        public static string DataTableToJSON(DataTable dt)
        {
            return ObjectToJSON(DataTableToList(dt));
        }

        /// <summary> 
        /// JSON文本转对象,泛型方法 
        /// </summary> 
        /// <typeparam name="T">类型</typeparam> 
        /// <param name="jsonText">JSON文本</param> 
        /// <returns>指定类型的对象</returns> 
        public static T JSONToObject<T>(string jsonText)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            try
            {
                return jss.Deserialize<T>(jsonText);
            }
            catch (Exception ex)
            {
                throw new Exception("JSONHelper.JSONToObject(): " + ex.Message);
            }
        }

        /// <summary> 
        /// 将JSON文本转换为数据表数据 
        /// </summary> 
        /// <param name="jsonText">JSON文本</param> 
        /// <returns>数据表字典</returns> 
        public static Dictionary<string, List<Dictionary<string, object>>> TablesDataFromJSON(string jsonText)
        {
            return JSONToObject<Dictionary<string, List<Dictionary<string, object>>>>(jsonText);
        }

        /// <summary> 
        /// 将JSON文本转换成数据行 
        /// </summary> 
        /// <param name="jsonText">JSON文本</param> 
        /// <returns>数据行的字典</returns>
        public static Dictionary<string, object> DataRowFromJSON(string jsonText)
        {
            return JSONToObject<Dictionary<string, object>>(jsonText);
        }
    }
}