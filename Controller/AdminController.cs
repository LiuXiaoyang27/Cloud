using Business;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace Controller
{
    /// <summary>
    /// 管理员用户操作类
    /// </summary>
    public class AdminController
    {
        private readonly AdminBll dal;

        public AdminController()
        {
            dal = new AdminBll();
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string strWhere)
        {
            return dal.Exists(strWhere);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        }

        public Admin GetListModel(DataRow row)
        {
            return dal.DataRowToModel(row);
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
                item["username"] = dataRow["username"].ToString();
                item["status"] = int.Parse(dataRow["status"].ToString());
                item["name"] = dataRow["name"].ToString();
                item["mobile"] = dataRow["mobile"].ToString();
                item["deptName"] = dataRow["deptName"].ToString();
                item["gender"] = dataRow["gender"].ToString();
                item["roleId"] = dataRow["roleId"].ToString();
                item["roleName"] = dataRow["roleName"].ToString();
                item["roleType"] = dataRow["roleType"].ToString();
                item["description"] = dataRow["description"].ToString();
                item["isAdmin"] = string.IsNullOrEmpty(dataRow["isAdmin"].ToString()) ? 0 : int.Parse(dataRow["isadmin"].ToString());
                items.Add(item);
            }
            return items;
        }
        /// <summary>
        /// 将DataTable转为JSON格式数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<string> GetUserIDList(DataTable dt)
        {
            List<string> items = new List<string>();
            DataRow dataRow;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dataRow = dt.Rows[i];
                items.Add(dataRow["id"].ToString());
            }
            return items;
        }
        /// <summary>
        /// 根据条件查询用户信息
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public Admin UserLogin(string userName, string password)
        {
            //string strWhere = " and (t1.username='" + userName + "') or ( t1.mobile= '" + userName + "')";
            Admin model = dal.GetLoginModel(userName);
            if (model != null)
            {
                //string md5Password = Common.Utils.MD5(password);
                if (password.Equals(model.userpwd))
                {
                    string roleId = model.roleId;
                    List<string> rights = new List<string>();
                    if (roleId == "0" || model.roleType == 1)
                    {
                        model.rights = rights;
                        return model;
                    }
                    Role role = new RoleController().GetModel(roleId);
                    if (role == null)
                    {
                        return null;
                    }
                    var values = role.roleValues;
                    for (int i = 0; i < values.Count; i++)
                    {
                        rights.Add(values[i].module + "_" + values[i].typeNumber);
                    }
                    model.rights = rights;
                    return model;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据条件查询用户信息
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public Admin GetModel(string strWhere)
        {
            return dal.GetModel(strWhere);
        }

        public Admin GetUserModel(string userId)
        {
            return dal.GetUserModel(userId);
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(Admin model)
        {
            return dal.Delete(model);
        }
        /// <summary>
        /// 根据条件更新用户状态
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public bool Disable(string id, int status)
        {
            return dal.Disable(id, status);
        }


        /// <summary>
        /// 新增一条管理员信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Add(Admin model, string authId)
        {
            string userId = dal.Add(model);
            if (!string.IsNullOrEmpty(userId))
            {               
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 修改一条管理员信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateAdmin(Admin model)
        {
            return dal.UpdateUserinfo(model);
        }

        /// <summary>
        /// 修改一条管理员信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateLoginUser(Admin model)
        {
            return dal.UpdateLoginUser(model);
        }

        /// <summary>
        /// 更新用户登录时间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateLoginTime(string userId, string ip)
        {
            return dal.UpdateLoginTime(userId, ip);
        }

        public bool UpdateUserAvatar(string userId, string filePath)
        {
            return dal.UpdateUserAvatar(userId, filePath);
        }
        /// <summary>
        /// 修改管理员密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateUserPassword(Admin model)
        {
            return dal.UpdateUserPassword(model);
        }

        /// <summary>
        /// 根据用户角色获得用户权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public JObject GetUserRightsJson(string roleId)
        {
            Role model = new RoleController().GetModel(roleId);
            if (model == null)
            {
                return null;
            }

            JObject moduleJson = new JObject();
            List<RoleValue> values = model.roleValues;
            string moduleName = "";
            JArray btnList = new JArray();
            for (int i = 0; i < values.Count; i++)
            {

                if (i == 0)
                {
                    moduleName = values[i].module;
                }
                if (moduleName.Equals(values[i].module))
                {
                    btnList.Add(values[i].typeNumber);
                    if (i == values.Count - 1)
                    {
                        moduleJson[moduleName] = btnList;
                    }
                }
                else
                {
                    moduleJson[moduleName] = btnList;

                    btnList = new JArray();
                    btnList.Add(values[i].typeNumber);
                }
                moduleName = values[i].module;
            }
            return moduleJson;
        }

        /// <summary>
        /// 获得用户树形结构数据
        /// </summary>
        /// <param name="listOrg">部门</param>
        /// <param name="listUsers">用户</param>
        /// <returns></returns>
        public JArray GetUserTreeView(List<Dept> listOrg,List<Admin> listUsers)
        {
            JArray result = new JArray();
            JArray children = new JArray();
            JObject parent;
            List<Dept> parents = listOrg.FindAll(a => a.PARENTID == "0");
            foreach (Dept model in parents)
            {
                children = GetChilds(model.ID, listOrg, listUsers);
                MergeJArray(children, GetUsers(listUsers, model.ID));
                parent = new JObject();
                parent["parentId"] = model.PARENTID;
                parent["text"] = model.FULLNAME;
                parent["hasChildren"] = children.Count > 0 ? true : false;
                parent["ChildNodes"] = children;
                parent["isexpand"] = true;
                parent["click"] = false;
                parent["id"] = model.ID;
                parent["complete"] = true;
                result.Add(parent);
            }
            return result;
        }
        /// <summary>
        /// 获取子节点
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="list"></param>
        /// <returns>JArray</returns>
        public JArray GetChilds(string ParentId, List<Dept> listOrg, List<Admin> listUsers)
        {
            JArray result = new JArray();
            JObject obj;
            JArray children = new JArray();
            foreach (Dept model in listOrg)
            {
                if (model.PARENTID == ParentId)
                {
                    obj = new JObject();
                    children = GetChilds(model.ID, listOrg, listUsers);
                    MergeJArray(children, GetUsers(listUsers, model.ID));
                    obj["parentId"] = model.PARENTID;
                    obj["text"] = model.FULLNAME;
                    obj["hasChildren"] = children.Count > 0 ? true : false;
                    obj["ChildNodes"] = children;
                    obj["isexpand"] = true;
                    obj["click"] = false;
                    obj["id"] = model.ID;
                    obj["complete"] = true;
                    result.Add(obj);
                }
            }
            return result;
        }
        /// <summary>
        /// 通过组织ID 获得人员信息
        /// </summary>
        /// <param name="list"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public JArray GetUsers(List<Admin> list, string orgId)
        {
            List<Admin> data = list.FindAll(a => a.deptId == orgId);
            JArray result = new JArray();
            JObject obj;
            foreach (Admin model in data)
            {
                obj = new JObject();
                obj["parentId"] = model.deptId;
                obj["text"] = model.name + "/" + model.username; //todo
                obj["img"] = "fa fa-user";
                obj["hasChildren"] = false;
                obj["ChildNodes"] = new JArray();
                obj["isexpand"] = true;
                obj["id"] = model.id; //todo
                obj["complete"] = true;
                obj["showcheck"] = true;
                obj["checkstate"] = null;
                result.Add(obj);
            }
            return result;
        }
        /// <summary>
        /// 合并数据
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void MergeJArray(JArray parent, JArray child)
        {
            if (child.Count == 0)
            {
                return;
            }
            foreach (JObject obj in child)
            {
                parent.Add(obj);
            }
        }
    }
}

