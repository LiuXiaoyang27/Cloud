using Business;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Data;

namespace Controller
{
    public class OldPetitionController
    {
        private readonly OldPetitionBll dal;

        public OldPetitionController()
        {
            dal = new OldPetitionBll();
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
            item["title"] = dataRow["TITLE"].ToString();
            item["pDate"] = dataRow["PDATE"].ToString();
            item["pName"] = dataRow["PNAME"].ToString();
            item["attendName"] = dataRow["ATTENDNAME"].ToString();
            return item;
        }

        public OldPetition GetModel(string id)
        {
            return dal.GetModel(id);
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
        public string Add(OldPetition Model)
        {
            return dal.Add(Model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(OldPetition model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string id)
        {
            return dal.Delete(id);
        }

        public JObject GetJsonObj(OldPetition dataRow)
        {
            JObject item = new JObject();
            item["id"] = dataRow.id;
            item["pDate"] = string.IsNullOrEmpty(dataRow.pDate) ? "" : DateTime.Parse(dataRow.pDate).ToString("yyyy-MM-dd HH:mm");
            item["pName"] = dataRow.pName;
            item["title"] = dataRow.title;
            item["attendName"] = dataRow.attendName;
            item["fileName"] = dataRow.fileName;
            item["fileType"] = dataRow.fileType;
            item["fileSize"] = dataRow.fileSize;
            item["filePath"] = dataRow.filePath;
            item["fileState"] = dataRow.fileState;
            item["modifyTime"] = dataRow.modifyTime.ToString();
            return item;
        }

    }
}
