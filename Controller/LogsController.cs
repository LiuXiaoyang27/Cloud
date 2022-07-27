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
    public class LogsController
    {
        private readonly LogsBll dal;

        public LogsController()
        {
            dal = new LogsBll();
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        /// <summary>
        /// 将DataTable转为JSON格式数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public JArray GetJsonList(DataTable dt)
        {
            JArray items = new JArray();
            JObject item = null;
            DataRow dataRow;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                item = new JObject();
                dataRow = dt.Rows[i];
                item["id"] = dataRow["id"].ToString();
                item["userId"] = dataRow["userId"].ToString();
                item["userName"] = dataRow["userName"].ToString();
                item["ip"] = dataRow["ip"].ToString();
                item["detail"] = dataRow["detail"].ToString();
                item["typeName"] = dataRow["typeName"].ToString();
                item["modifyTime"] = DateTime.Parse(dataRow["modifyTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                item["nickName"] = dataRow["nickName"].ToString();
                item["infoLevel"] = dataRow["infoLevel"].ToString();
                items.Add(item);
            }
            return items;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public int Delete()
        {
            return dal.Delete();
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(string detail, Constant.ActionEnum action, Model.Admin admin, Constant.InfoEnum infoLevel = Constant.InfoEnum.Noraml)
        {
            Model.Logs model = new Model.Logs();
            model.userId = admin.id;
            model.ip = Utils.GetClientIP();
            model.userName = admin.username;
            model.detail = detail;
            model.typeNum = (int)action;
            model.typeName = action.ToString();
            model.infoLevel = (int)infoLevel;
            int result = dal.Add(model);
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}

