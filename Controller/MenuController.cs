using Business;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace Controller
{
    /// <summary>
    /// 辅助资料频道操作类
    /// </summary>
    public class MenuController
    {
        private readonly MenuBll dal;
        public MenuController()
        {
            dal = new MenuBll();
        }

        /// <summary>
        /// 获得菜单列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        }

        /// <summary>
        /// 获得菜单列表
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <returns></returns>
        public DataSet GetRoleList(string roleId)
        {
            return dal.GetRoleList(roleId);
        }
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string strWhere)
        {
            return dal.Exists(strWhere);
        }
        /// <summary>
        /// 根据单位ID获得频道信息的实体类
        /// </summary>
        public Model.Menu GetModel(string menuId)
        {
            return dal.GetModel(menuId);
        }
        /// <summary>
        /// 将DataRow转为JSON格式数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public JObject GetJsonObj(DataRow dataRow)
        {
            JObject item = new JObject();
            item["id"] = int.Parse(dataRow["id"].ToString());
            item["name"] = dataRow["name"].ToString();
            item["parentId"] = int.Parse(dataRow["parentId"].ToString());
            item["path"] = dataRow["cPath"].ToString();
            item["level"] = int.Parse(dataRow["cLevel"].ToString());
            item["fontIcon"] = dataRow["fontIcon"].ToString();
            item["module"] = dataRow["module"].ToString();
            item["status"] = int.Parse(dataRow["status"].ToString());
            item["ordnum"] = int.Parse(dataRow["ordnum"].ToString());
            item["linkUrl"] = dataRow["linkUrl"].ToString();
            item["navType"] = dataRow["navType"].ToString();
            item["LinkTarget"] = dataRow["LinkTarget"].ToString();
            item["typeNumber"] = dataRow["typeNumber"].ToString();
            item["detail"] = dataRow["detail"].ToString();
            item["remark"] = dataRow["remark"].ToString();
            item["modifyTime"] = DateTime.Parse(dataRow["modifyTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
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
        /// 删除一条数据
        /// </summary>
        public bool Delete(string memuId)
        {
            return dal.Delete(memuId);
        }
        /// <summary>
        /// 更新频道是否可用
        /// </summary>
        public bool UpdateStatus(string memuId, int status)
        {
            return dal.UpdateStatus(memuId, status);
        }

        /// <summary>
        /// 检查调用别名是否存在
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public bool CheckModule(string module, string menuId)
        {
            return dal.CheckModule(module, menuId).Tables[0].Rows.Count > 0;
        }
        /// <summary>
        /// 增加一条数据
        /// </summary>     
        public Model.Menu Add(Model.Menu model)
        {
            // 默认信息
            model.status = 1;
            model.isDelete = 0;
            model.level = 1;

            if (string.IsNullOrEmpty(model.parentId) || model.parentId.Equals("0"))
            {
                  
            }
            else
            {
                Model.Menu temp = dal.GetModel(model.parentId);
                int level = temp.level + 1;
                model.level = level;
                
            }

            string result = dal.Add(model);

            if (!string.IsNullOrEmpty(result))
            {
                return model;
            }
            else
            {
                return null;
            }
          
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.Menu model)
        {
            int level = 1;
            if (string.IsNullOrEmpty(model.parentId) || model.parentId.Equals("0"))
            {
                
            }
            else
            {
                Model.Menu temp = dal.GetModel(model.parentId);
                level = temp.level + 1;
            }
            model.level = level;
            return dal.Update(model);

        }
        #region =============新增：获取树形结构数据方法  --liuxiaoyang 20210429===============      

        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<Menu> GetModelList(string strWhere = "")
        {
            DataTable dt = GetList(strWhere).Tables[0];
            List<Menu> list = new List<Menu>();
            foreach (DataRow dataRow in dt.Rows)
            {
                list.Add(dal.DataRowToModel(dataRow));
            }
            return list;
        }

        /// <summary>
        /// list 转树形 json
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public JArray ListToTreeJson(List<Menu> list)
        {
            JArray result = new JArray();
            List<Menu> parents = new List<Menu>();
            //获取父级节点
            parents = GetParentNodes(list, "comBoxTree");
            foreach (Menu model in parents)
            {
                JArray children = GetChilds(model.id, list);
                JObject parent = new JObject();
                parent["parentId"] = model.parentId;
                parent["text"] = model.name;
                parent["img"] = model.fontIcon == "" ? "fa fa-file-text" : "fa " + model.fontIcon;
                parent["hasChildren"] = children.Count > 0 ? true : false;
                parent["ChildNodes"] = children;
                parent["isexpand"] = true;
                parent["id"] = model.id.ToString();
                parent["complete"] = true;
                result.Add(parent);
            }

            return result;
        }
        /// <summary>
        /// 获取父节点信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<Menu> GetParentNodes(List<Menu> list, string type)
        {
            List<Menu> result = new List<Menu>();
            if (type == "comBoxTree")
            {
                //添加顶级节点
                Menu menu = new Menu();
                menu.id = "-1";
                menu.parentId = "0";
                menu.name = "父级导航";
                menu.fontIcon = "fa-folder-open";
                result.Add(menu);
            }         
            result.AddRange(list.FindAll(a => a.parentId.Equals("0")));
            return result;
        }
        /// <summary>
        /// 获取子节点
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="list"></param>
        /// <returns>JArray</returns>
        public JArray GetChilds(string ParentId, List<Menu> list)
        {
            JArray result = new JArray();
            foreach (Menu model in list)
            {
                if (model.parentId.Equals(ParentId))
                {
                    JObject obj = new JObject();
                    JArray children = GetChilds(model.id, list);
                    obj["parentId"] = model.parentId;
                    obj["text"] = model.name;
                    obj["img"] = model.fontIcon == "" ? "fa fa-file-text" : "fa " + model.fontIcon;
                    obj["hasChildren"] = children.Count > 0 ? true : false;
                    obj["ChildNodes"] = children;
                    obj["isexpand"] = true;
                    obj["id"] = model.id.ToString();
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
        public JArray ListToGridTreeJson(List<Menu> list)
        {
            //对list重新排序
            List<Menu> menuList = OrderList(list);
            JArray result = new JArray();
            foreach (Menu model in menuList)
            {
                JObject parent = ModelToJson(model);
                parent["loaded"] = true;
                parent["expanded"] = true;
                result.Add(parent);
            }
            return result;
        }
        /// <summary>
        /// 对list排序
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<Menu> OrderList(List<Menu> list)
        {
            List<Menu> menuList = new List<Menu>();
            List<Menu> parents = GetParentNodes(list, "gridTree");
            foreach (Menu model in parents)
            {
                model.level = 0;
                List<Menu> childNodes = GetChildsList(model.id, list, model.level);
                menuList.Add(model);
                if (childNodes.Count == 0)
                {
                    model.isLeaf = true;
                }
                else
                {
                    model.isLeaf = false;
                    menuList.AddRange(childNodes);
                }
            }
            return menuList;
        }
        /// <summary>
        /// 获取子节点集合
        /// </summary>
        /// <param name="ParentId"></param>
        /// <param name="list"></param>
        /// <returns>实体对象集合</returns>
        public List<Menu> GetChildsList(string ParentId, List<Menu> list, int Level)
        {
            List<Menu> menuList = new List<Menu>();
            foreach (Menu model in list)
            {
                if (model.parentId.Equals(ParentId))
                {
                    model.level = Level + 1;
                    List<Menu> childNodes = GetChildsList(model.id, list, model.level);
                    menuList.Add(model);
                    if (childNodes.Count == 0)
                    {
                        model.isLeaf = true;
                    }
                    else
                    {
                        model.isLeaf = false;
                        menuList.AddRange(childNodes);
                    }
                }
            }
            return menuList;
        }
        /// <summary>
        /// 实体类转 json
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JObject ModelToJson(Menu model)
        {
            JObject obj = new JObject();
            obj["id"] = model.id;
            obj["name"] = model.name;
            obj["module"] = model.module;
            obj["fontIcon"] = model.fontIcon == "" ? "fa fa-file-text" : "fa " + model.fontIcon;
            obj["ordnum"] = model.ordnum;
            obj["remark"] = model.remark;
            obj["status"] = model.status;
            //obj["cpath"] = model.path;
            obj["linkUrl"] = model.linkUrl;
            obj["typeNumber"] = model.typeNumber;
            obj["navType"] = model.navType;
            obj["LinkTarget"] = model.navType;
            //---
            obj["parent"] = model.parentId;
            obj["level"] = model.level;
            obj["isLeaf"] = model.isLeaf;
            return obj;
        }
        #endregion

    }
}
