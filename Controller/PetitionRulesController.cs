using Business;
using Common;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

namespace Controller
{
    /// <summary>
    /// 通知公告操作类
    /// </summary>
    public class PetitionRulesController
    {
        private readonly PetitionRulesBll dal;

        public PetitionRulesController()
        {
            dal = new PetitionRulesBll();
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string strWhere)
        {
            return dal.Exists(strWhere);
        }

        /// <summary>
        /// 获得查询分页数据 
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
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
            item["FileName"] = dataRow["FileName"].ToString();
            item["FileType"] = dataRow["FileType"].ToString();
            item["FileSize"] = int.Parse(dataRow["FileSize"].ToString());
            item["FilePath"] = dataRow["FilePath"].ToString();
            item["FileState"] = dataRow["FileState"].ToString();
            item["modifyTime"] = dataRow["modifyTime"].ToString();
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
        /// 新增
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public string Add(PetitionRules Model)
        {
            return dal.Add(Model);
        }


   
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string id)
        {
            return dal.Delete(id);
        }


        /// <summary>
        /// 获得实体类
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public Petition GetTransferModel(DataRow row)
        {
            Petition model = new Petition();
            model.createDate = row["createDate"].ToString();
            model.pName = row["pName"].ToString();
            model.pIdCard = row["pIdCard"].ToString();
            model.pAddress = row["pAddress"].ToString();
            model.caseType = row["caseType"].ToString();
            model.caseName = row["caseName"].ToString();
            model.caseSource = row["caseSource"].ToString();
            model.channels = row["channels"].ToString();
            model.receiver = row["receiver"].ToString();
            model.rerm = int.Parse(row["rerm"].ToString());

            return model;
        }

    }
}
