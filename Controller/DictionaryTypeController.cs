using Business;
using Model;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;

namespace Controller
{
    /// <summary>
    /// 类别操作类
    /// </summary>
    public class DictionaryTypeController
    {
        private readonly DictionaryTypeBll dal;

        public DictionaryTypeController()
        {
            dal = new DictionaryTypeBll();
        }
        /// <summary>
        /// 查询数据是否存在
        /// </summary>
        /// <param name="strWhere">查询条件</param>
        /// <returns></returns>
        public bool IsExist(string strWhere)
        {
            return dal.IsExist(strWhere);
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="strWhere">查询条件</param>
        /// <returns></returns>
        public DataTable GetList(string strWhere)
        {
            return dal.GetList(strWhere).Tables[0];
        }
        public List<DictionaryType> GetListModel(string strWhere = "")
        {
            DataTable dt = GetList(strWhere);
            DictionaryTypeBll bll = new DictionaryTypeBll();
            List<DictionaryType> list = new List<DictionaryType>();
            foreach (DataRow dataRow in dt.Rows)
            {
                list.Add(bll.DataRowToModel(dataRow));
            }
            return list;
        }
        /// <summary>
        /// 下拉框树形结构
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public JArray GetTreeView(List<DictionaryType> list,string type = "")
        {
            JArray result = new JArray();
            //获取父节点信息
            List<DictionaryType> parent = ParentNode(list,type);
            foreach (DictionaryType model in parent)
            {
                JArray childs = GetChilds(model.ID, list);
                JObject parents = new JObject();
                parents["parentId"] = model.PARENTID;
                parents["text"] = model.FULLNAME;
                parents["hasChildren"] = childs.Count > 0 ? true : false;
                parents["ChildNodes"] = childs;
                parents["isexpand"] = true;
                parents["id"] = model.ID.ToString();
                parents["complete"] = true;
                result.Add(parents);
            }
            return result;
        }
        /// <summary>
        /// 获取父节点信息
        /// </summary>
        /// <param name="list">字典分类数据</param>
        /// <returns></returns>
        public List<DictionaryType> ParentNode(List<DictionaryType> list,string type)
        {
            List<DictionaryType> result = new List<DictionaryType>();
            if(type == "edit")
            {
                //添加顶级节点
                DictionaryType modle = new DictionaryType();
                modle.ID = "-1";
                modle.PARENTID = "0";
                modle.FULLNAME = "父级节点";
                result.Add(modle);
            }
            result.AddRange(list.FindAll(a => a.PARENTID.Equals("0")));
            return result;
        }
        /// <summary>
        /// 获取子节点
        /// </summary>
        /// <param name="parentId">父级ID</param>
        /// <param name="list">字典分类数据</param>
        /// <returns></returns>
        public JArray GetChilds(string ParentId, List<DictionaryType> list)
        {
            JArray result = new JArray();
            foreach (DictionaryType model in list)
            {
                if (model.PARENTID.Equals(ParentId))
                {
                    JObject obj = new JObject();
                    JArray children = GetChilds(model.ID, list);
                    obj["parentId"] = model.PARENTID;
                    obj["text"] = model.FULLNAME;
                    obj["hasChildren"] = children.Count > 0 ? true : false;
                    obj["ChildNodes"] = children;
                    obj["isexpand"] = true;
                    obj["id"] = model.ID.ToString();
                    obj["complete"] = true;
                    result.Add(obj);
                }
            }
            return result;
        }
        /// <summary>
        /// DataRow 转 Json
        /// </summary>
        /// <param name="dataRow">数据行</param>
        /// <returns></returns>
        public JObject RowToJson(DataRow dataRow)
        {
            JObject obj = new JObject();
            obj["id"] = dataRow["ID"].ToString();
            obj["ParentId"] = dataRow["PARENTID"].ToString();
            obj["FullName"] = dataRow["FULLNAME"].ToString();
            obj["EnCode"] = dataRow["ENCODE"].ToString();
            obj["SimpleSpellIng"] = dataRow["SIMPLESPELLING"].ToString();
            obj["IsTree"] = dataRow["ISTREE"].ToString();
            obj["Description"] = dataRow["DESCRIPTION"].ToString();
            obj["SortCode"] = dataRow["SORTCODE"].ToString();
            return obj;
        }
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        public DataTable GetInfo(string id)
        {
            return dal.GetInfo(id).Tables[0];
        }
        /// <summary>
        /// 根据ID获得类别信息的实体类
        /// </summary>
        /// <param name="id">类别ID</param>
        /// <returns></returns>
        public Model.DictionaryType GetModel(string id)
        {

            return dal.GetModel(id);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        public bool Delete(DictionaryType entity)
        {
            return dal.Delete(entity) > 0;
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public int Create(DictionaryType entity)
        {
            entity.ISDELETE = 0;
            return dal.Create(entity);
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public bool Update(DictionaryType entity)
        {
            return dal.Update(entity) > 0;
        }

        /// <summary>
        /// 上移
        /// </summary>
        /// <param name="id">主键值</param>
        public bool First(DictionaryType entity)
        {
            return dal.First(entity) > 0;
        }
        /// <summary>
        /// 下移
        /// </summary>
        /// <param name="id">主键值</param>
        public bool Next(DictionaryType entity)
        {
            return dal.Next(entity) > 0;
        }

    }
}

