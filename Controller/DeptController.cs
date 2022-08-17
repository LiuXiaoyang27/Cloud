using Business;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace Controller
{
    /// <summary>
    /// 部门操作类
    /// </summary>
    public class DeptController
    {
        private readonly DeptBll dal;

        public DeptController()
        {
            dal = new DeptBll();
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
        public List<Dept> GetListModel(string strWhere = "")
        {
            DataTable dt = GetList(strWhere);
            DeptBll bll = new DeptBll();
            List<Dept> list = new List<Dept>();
            foreach (DataRow dataRow in dt.Rows)
            {
                list.Add(bll.DataRowToModel(dataRow));
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
            obj["EnabledMark"] = dataRow["ENABLEDMARK"].ToString();
            obj["Description"] = dataRow["DESCRIPTION"].ToString();
            obj["mobile"] = dataRow["MOBILE"].ToString();
            obj["tel"] = dataRow["TEL"].ToString();
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
        /// 根据ID获得部门信息的实体类
        /// </summary>
        /// <param name="id">部门ID</param>
        /// <returns></returns>
        public Dept GetModel(string id)
        {

            return dal.GetModel(id);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        public bool Delete(Dept entity)
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
        public int Create(Dept entity)
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
        public bool Update(Dept entity)
        {
            entity.ISDELETE = 0;
            entity.LASTMODIFYTIME = DateTime.Now;
            return dal.Update(entity) > 0;
        }

        /// <summary>
        /// 上移
        /// </summary>
        /// <param name="id">主键值</param>
        public bool First(Dept entity)
        {
            return dal.First(entity) > 0;
        }
        /// <summary>
        /// 下移
        /// </summary>
        /// <param name="id">主键值</param>
        public bool Next(Dept entity)
        {
            return dal.Next(entity) > 0;
        }

        /// <summary>
        /// 下拉框树形结构
        /// </summary>
        /// <param name="listData">部门列表数据</param>
        /// <returns></returns>
        public JArray ListToTreeJson(List<Dept> listData, string type = "")
        {
            JArray result = new JArray();
            List<Dept> list = GetParentNodes(listData, type);
            JArray children = new JArray();
            JObject parent;
            //获取父节点信息
            foreach (Dept model in list)
            {
                children = GetChilds(listData, model.ID);
                parent = new JObject();
                parent["parentId"] = model.PARENTID;
                parent["text"] = model.FULLNAME;
                parent["img"] = "fa fa-sitemap";
                parent["hasChildren"] = children.Count > 0 ? true : false;
                parent["ChildNodes"] = children;
                parent["isexpand"] = false;
                parent["id"] = model.ID.ToString();
                parent["complete"] = true;
                result.Add(parent);
            }
            return result;
        }

        /// <summary>
        /// 子节点数据
        /// </summary>
        /// <param name="list">部门列表数据</param>
        /// <param name="id">id</param>
        /// <returns></returns>
        public JArray GetChilds(List<Dept> list, string ParentId)
        {
            JArray result = new JArray();
            foreach (Dept model in list)
            {
                if (model.PARENTID.Equals(ParentId))
                {
                    JObject obj = new JObject();
                    JArray children = GetChilds(list, model.ID);
                    obj["parentId"] = model.PARENTID;
                    obj["text"] = model.FULLNAME;
                    obj["img"] = "fa fa-sitemap";
                    obj["hasChildren"] = children.Count > 0 ? true : false; ;
                    obj["ChildNodes"] = children;
                    obj["isexpand"] = false;
                    obj["title"] = model.ENCODE;
                    obj["id"] = model.ID.ToString();
                    obj["complete"] = true;
                    result.Add(obj);
                }
            }
            return result;
        }
        /// <summary>
        /// 获得gridtree 数据结构
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public JArray ListToGridTreeJson(List<Dept> list)
        {
            //重新组织List数据
            List<Dept> deptList = DeptList(list);
            JArray result = new JArray();
            foreach (Dept model in deptList)
            {
                JObject parent = ModelToJson(model);
                parent["loaded"] = true;
                parent["expanded"] = false;
                result.Add(parent);
            }
            return result;
        }
        /// <summary>
        /// 重新组织List数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<Dept> DeptList(List<Dept> list)
        {
            List<Dept> deptList = new List<Dept>();
            List<Dept> parents = GetParentNodes(list, "");
            foreach (Dept model in parents)
            {
                model.LEVEL = 0;
                List<Dept> childNodes = GetChildsList(model.ID, list, model.LEVEL);
                deptList.Add(model);
                if (childNodes.Count == 0)
                {
                    model.ISLEAF = true;
                }
                else
                {
                    model.ISLEAF = false;
                    deptList.AddRange(childNodes);
                }

            }
            return deptList;
        }
        /// <summary>
        /// Model 转 Json
        /// </summary>
        /// <param name="model">数据行</param>
        /// <returns></returns>
        public JObject ModelToJson(Dept model)
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
            obj["mobile"] = model.MOBILE;
            obj["tel"] = model.TEL;
            return obj;
        }

        /// <summary>
        /// 获取父节点信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<Dept> GetParentNodes(List<Dept> list, string type)
        {
            List<Dept> result = new List<Dept>();
            Dept dept = new Dept();
            if (type == "dept")
            {
                dept.ID = "-1";
                dept.PARENTID = "0";
                dept.FULLNAME = "顶级节点";
                result.Add(dept);
            }
            result.AddRange(list.FindAll(a => a.PARENTID.Equals("0")));
            return result;
        }

        /// <summary>
        /// 获取自节点集合
        /// </summary>
        /// <param name="ParentId"></param>
        /// <param name="list"></param>
        /// <returns>实体对象集合</returns>
        public List<Dept> GetChildsList(string ParentId, List<Dept> list, int Level)
        {
            List<Dept> deptList = new List<Dept>();
            foreach (Dept model in list)
            {
                if (model.PARENTID.Equals(ParentId))
                {
                    model.LEVEL = Level + 1;
                    List<Dept> childNodes = GetChildsList(model.ID, list, model.LEVEL);
                    deptList.Add(model);
                    if (childNodes.Count == 0)
                    {
                        model.ISLEAF = true;
                    }
                    else
                    {
                        model.ISLEAF = false;
                        deptList.AddRange(childNodes);
                    }

                }
            }
            return deptList;
        }

    }
}

