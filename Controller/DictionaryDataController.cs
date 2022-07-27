using Business;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace Controller
{
    /// <summary>
    /// 类别操作类
    /// </summary>
    public class DictionaryDataController
    {
        private readonly DictionaryDataBll dal;

        public DictionaryDataController()
        {
            dal = new DictionaryDataBll();
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
        public List<DictionaryData> GetListModel(string strWhere = "")
        {
            DataTable dt = GetList(strWhere);
            List<DictionaryData> list = new List<DictionaryData>();
            foreach (DataRow dataRow in dt.Rows)
            {
                list.Add(dal.DataRowToModel(dataRow));
            }
            return list;
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
            obj["EnabledMark"] = dataRow["ENABLEDMARK"].ToString();
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
        public Model.DictionaryData GetModel(string id)
        {

            return dal.GetModel(id);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        public bool Delete(DictionaryData entity)
        {
            entity.ISDELETE = 1;
            entity.DELETETIME = DateTime.Now;
            return dal.Delete(entity) > 0;
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public int Create(DictionaryData entity)
        {
            entity.ISDELETE = 0;
            entity.CREATORTIME = DateTime.Now;
            return dal.Create(entity);
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public bool Update(DictionaryData entity)
        {
            entity.ISDELETE = 0;
            entity.LASTMODIFYTIME = DateTime.Now;
            return dal.Update(entity) > 0;
        }

        /// <summary>
        /// 上移
        /// </summary>
        /// <param name="id">主键值</param>
        public bool First(DictionaryData entity)
        {
            return dal.First(entity) > 0;
        }
        /// <summary>
        /// 下移
        /// </summary>
        /// <param name="id">主键值</param>
        public bool Next(DictionaryData entity)
        {
            return dal.Next(entity) > 0;
        }
        /// <summary>
        /// 下拉框树形结构
        /// </summary>
        /// <param name="listData">字典列表数据</param>
        /// <param name="typeId">字典分类id</param>
        /// <returns></returns>
        public JArray GetTreeView(List<DictionaryData> listData, string TypeId)
        {
            JArray result = new JArray();
            List<DictionaryType> listType = new DictionaryTypeController().GetListModel();
            List<DictionaryType> parents = listType.FindAll(a => !a.PARENTID.Equals("0") && a.ID.Equals(TypeId));
            JArray children = new JArray();
            JObject parent;
            //获取父节点信息
            foreach (DictionaryType model in parents)
            {
                if (model.ISTREE != 0)
                {
                    children = GetDictionaryData(listData, model.ID);
                }
                parent = new JObject();
                parent["parentId"] = model.PARENTID;
                parent["text"] = model.FULLNAME;
                parent["hasChildren"] = children.Count > 0 ? true : false;
                parent["ChildNodes"] = children;
                parent["isexpand"] = true;
                parent["id"] = model.ID.ToString();
                parent["complete"] = true;
                result.Add(parent);
            }
            return result;
        }
        /// <summary>
        /// 通过字典分类ID 获得字典列表
        /// </summary>
        /// <param name="list">字典列表数据</param>
        /// <param name="id">id</param>
        /// <returns></returns>
        public JArray GetDictionaryData(List<DictionaryData> list, string id)
        {
            List<DictionaryData> data = list.FindAll(a => a.PARENTID.Equals("0"));
            JArray result = new JArray();
            JObject obj;
            JArray children = new JArray();
            foreach (DictionaryData model in data)
            {
                children = DictionaryData(list, model.ID);
                obj = new JObject();
                obj["parentId"] = model.TYPEID;
                obj["text"] = model.FULLNAME;
                obj["hasChildren"] = children.Count > 0 ? true : false;
                obj["ChildNodes"] = children;
                obj["isexpand"] = true;
                obj["title"] = model.ENCODE;
                obj["id"] = model.ID.ToString();
                obj["complete"] = true;
                result.Add(obj);
            }
            return result;
        }
        /// <summary>
        /// 子节点数据
        /// </summary>
        /// <param name="list">字典列表数据</param>
        /// <param name="id">id</param>
        /// <returns></returns>
        public JArray DictionaryData(List<DictionaryData> list, string id)
        {
            JArray result = new JArray();
            JObject obj;
            JArray children = new JArray();
            List<DictionaryData> dicData = list.FindAll(a => a.PARENTID.Equals(id));
            foreach (DictionaryData model in dicData)
            {
                obj = new JObject();
                obj["parentId"] = model.PARENTID;
                obj["text"] = model.FULLNAME;
                obj["hasChildren"] = children.Count > 0 ? true : false; ;
                obj["ChildNodes"] = children;
                obj["isexpand"] = true;
                obj["title"] = model.ENCODE;
                obj["id"] = model.ID.ToString();
                obj["complete"] = true;
                result.Add(obj);
            }
            return result;
        }
        /// <summary>
        /// 获得gridtree 数据结构
        /// </summary>
        /// <param name="list">字典列表数据</param>
        /// <returns></returns>
        public JArray ListToGridTreeJson(List<DictionaryData> list)
        {
            //重新组织List数据
            List<DictionaryData> menuList = GetTreeList(list);
            JArray result = new JArray();
            foreach (DictionaryData model in menuList)
            {
                JObject parent = ProvinceJson(model);
                parent["loaded"] = true;
                parent["expanded"] = true;
                result.Add(parent);
            }
            return result;
        }
        /// <summary>
        /// 重新组织List数据
        /// </summary>
        /// <param name="list">字典列表数据</param>
        /// <returns></returns>
        public List<DictionaryData> GetTreeList(List<DictionaryData> list)
        {
            //对list重新排序
            List<DictionaryData> result = new List<DictionaryData>();
            List<DictionaryData> parents = ParentNode(list);
            foreach (DictionaryData province in parents)
            {
                province.LEVEL = 0;
                List<DictionaryData> childNodes = GetChildsList(province.ID, list, province.LEVEL);
                result.Add(province);
                if (childNodes.Count == 0)
                {
                    province.ISLEAF = true;
                }
                else
                {
                    province.ISLEAF = false;
                    result.AddRange(childNodes);
                }
            }
            return result;
        }
        /// <summary>
        /// 获取父节点信息
        /// </summary>
        ///  <param name="list">字典列表数据</param>
        /// <returns></returns>
        public List<DictionaryData> ParentNode(List<DictionaryData> list)
        {
            List<DictionaryData> result = new List<DictionaryData>();
            result.AddRange(list.FindAll(a => a.PARENTID.Equals("0")));
            return result;
        }
        /// <summary>
        /// 获取子节点集合
        /// </summary>
        /// <param name="ParentId">父节点id</param>
        /// <param name="list">字典列表数据</param>
        /// <param name="Level">是否有下拉图标</param>
        /// <returns></returns>
        public List<DictionaryData> GetChildsList(string ParentId, List<DictionaryData> list, int Level)
        {
            List<DictionaryData> province = new List<DictionaryData>();
            foreach (DictionaryData model in list)
            {
                if (model.PARENTID.Equals(ParentId))
                {
                    model.LEVEL = Level + 1;
                    List<DictionaryData> childNodes = GetChildsList(model.ID, list, model.LEVEL);
                    province.Add(model);
                    if (childNodes.Count == 0)
                    {
                        model.ISLEAF = true;
                    }
                    else
                    {
                        model.ISLEAF = false;
                        province.AddRange(childNodes);
                    }

                }
            }
            return province;
        }
        /// <summary>
        /// 转换数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JObject ProvinceJson(DictionaryData model)
        {
            JObject obj = new JObject();
            obj["Id"] = model.ID;
            obj["FullName"] = model.FULLNAME;
            obj["EnCode"] = model.ENCODE;
            obj["EnabledMark"] = model.ENABLEDMARK;
            obj["SimpleSpelling"] = model.SIMPLESPELLING;
            obj["ParentId"] = model.PARENTID;
            obj["LastModifyUserId"] = model.LASTMODIFYUSERID;
            obj["LastModifyTime"] = model.LASTMODIFYTIME;
            obj["CreatorTime"] = model.CREATORTIME;
            obj["CreatorUserId"] = model.CREATORUSERID;
            obj["IsDelete"] = model.ISDELETE;
            obj["DeleteTime"] = model.DELETETIME;
            obj["DeleteUserId"] = model.DELETEUSERID;
            obj["Description"] = model.DESCRIPTION;
            obj["SortCode"] = model.SORTCODE;
            obj["parent"] = model.PARENTID;
            obj["level"] = model.LEVEL;
            obj["isLeaf"] = model.ISLEAF;
            return obj;
        }
        /// <summary>
        /// 数据字典信息
        /// </summary>
        /// <returns></returns>
        public JArray BindDictionary()
        {
            JArray result = new JArray();
            List<DictionaryType> listType = new DictionaryTypeController().GetListModel(" And ParentId <> '0' ");
            List<DictionaryData> listData = new DictionaryDataController().GetListModel();
            JObject data = null;
            for (int i = 0; i < listType.Count; i++)
            {
                data = new JObject();
                data["key"] = listType[i].ENCODE;
                data["isTree"] = listType[i].ISTREE;
                data["value"] = GetDictionaryData(listType[i], listData);
                result.Add(data);
            }
            return result;
        }
        /// <summary>
        /// 查询数据返回JArray
        /// </summary>
        /// <param name="typeData">字典分类数据</param>
        /// <param name="listData">字典列表数据</param>
        /// <returns></returns>
        public JArray GetDictionaryData(DictionaryType typeData, List<DictionaryData> listData)
        {
            JArray result = new JArray();

            List<DictionaryData> list = listData.FindAll(a => a.TYPEID.Equals(typeData.ID));
            JObject obj = new JObject();
            if (typeData.ISTREE == 0)
            {
                foreach (DictionaryData data in list)
                {
                    obj = new JObject();
                    obj["text"] = data.FULLNAME;
                    //obj["id"] = data.ENCODE;
                    obj["code"] = data.ENCODE;
                    obj["id"] = data.ID.ToString();
                    result.Add(obj);
                }
            }
            else
            {
                foreach (DictionaryData data in list)
                {
                    obj = new JObject();
                    obj["text"] = data.FULLNAME;
                    //obj["id"] = data.ENCODE;
                    obj["id"] = data.ID.ToString();
                    obj["parentId"] = data.PARENTID;
                    result.Add(obj);
                }
            }
            return result;
        }



        /// <summary>
        /// 获取父节点信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<DictionaryData> GetParentNodes(List<DictionaryData> list, int typeId, bool noTop)
        {
            List<DictionaryType> listType = new DictionaryTypeController().GetListModel(" And ID = '" + typeId + "' ");
            int isTree = listType[0].ISTREE;
            List<DictionaryData> result = new List<DictionaryData>();
            if (!noTop && isTree == 1)
            {
                //添加顶级节点
                DictionaryData model = new DictionaryData();
                model.ID = "-1";
                model.PARENTID = "0";
                model.FULLNAME = "全部";
                result.Add(model);
            }
            result.AddRange(list.FindAll(a => a.PARENTID.Equals("0")));
            return result;
        }
        /// <summary>
        /// 资产类别树形
        /// </summary>
        public JArray BatchDictionary(List<DictionaryData> listData, int typeId, bool noTop = false)
        {
            List<DictionaryData> parents = GetParentNodes(listData, typeId, noTop);
            JArray result = new JArray();
            JArray children = new JArray();
            JObject parent;
            foreach (DictionaryData model in parents)
            {
                children = GetChilds(model.ID, listData);
                parent = new JObject();
                parent["parentId"] = model.PARENTID;
                parent["text"] = model.FULLNAME;
                parent["hasChildren"] = children.Count > 0 ? true : false;
                parent["ChildNodes"] = children;
                parent["isexpand"] = true;
                parent["id"] = model.ID.ToString();
                parent["complete"] = true;
                result.Add(parent);
            }
            return result;
        }
        /// <summary>
        /// 获取子节点
        /// </summary>
        /// <param name="ParentId">父节点id</param>
        /// <param name="listCate">资产类别数据</param>
        /// <returns>JArray</returns>
        public JArray GetChilds(string ParentId, List<DictionaryData> listCate)
        {
            JArray result = new JArray();
            JObject obj;
            JArray children = new JArray();
            foreach (DictionaryData model in listCate)
            {
                if (model.PARENTID.Equals(ParentId))
                {
                    obj = new JObject();
                    children = GetChilds(model.ID, listCate);
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

    }
}

