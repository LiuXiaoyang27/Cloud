using Business;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;

namespace Controller
{
    /// <summary>
    /// 系统设置操作类
    /// </summary>
    public class OptionsController
    {
        private readonly OptionsBll dal;

        public OptionsController()
        {
            dal = new OptionsBll();
        }

        /// <summary>
        /// 通过关键字获得系统参数信息
        /// </summary>
        /// <param name="optionName">参数名称</param>
        /// <returns>参数内容</returns>
        public JObject GetOptions()
        {
            DataTable dt = dal.GetOptions();
            JObject obj = new JObject();
            foreach (DataRow dataRow in dt.Rows)
            {
                string op_name = dataRow["op_name"].ToString();
                string op_value = dataRow["op_value"].ToString();
                obj[op_name] = op_value;
            }
            return obj;
        }

        public string GetOptions(string name)
        {
            DataTable dt = dal.GetOptions(name);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["op_value"].ToString();
            }
            return "";
        }
        /// <summary>
        /// 更新系统参数信息
        /// </summary>
        /// <param name="optionValue">参数内容</param>
        /// <param name="optionName">参数名称</param>
        /// <returns></returns>
        public bool UpdateOptions(List<Model.Options> list)
        {
            return dal.UpdateOptions(list);
        }
    }
}
