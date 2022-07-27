using Business;
using Newtonsoft.Json.Linq;
using System;
using System.Data;

namespace Controller
{
    /// <summary>
    /// 计算单位操作类
    /// </summary>
    public class RoleController
    {
        private readonly RoleBll dal;

        public RoleController()
        {
            dal = new RoleBll();
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string roleId)
        {
            return dal.Exists(roleId);
        }

        /// <summary>
        /// 检查是否有权限
        /// </summary>
        public bool VerifyRight(string roleId, string module, string typeNumber)
        {
            //if (roleId == 0)
            //{
            //    return true;
            //}
            Model.Role model = dal.GetModel(roleId);
            if (model != null)
            {
                // 超级管理员
                if (model.type == 1)
                {
                    return true;
                }
                Model.RoleValue modelt = model.roleValues.Find(p => p.module.Equals(module) && p.typeNumber.Equals(typeNumber));
                if (modelt != null)
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(Model.Role model)
        {          
            return dal.Add(model);
        }

        /// <summary>
        /// 检查用户角色是否存在
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public bool CheckName(string name)
        {
            return dal.CheckName(name).Tables[0].Rows.Count > 0;
        }

        /// <summary>
        /// 检查用户角色是否存在
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public bool CheckName(string name, string roleId)
        {
            return dal.CheckName(name, roleId).Tables[0].Rows.Count > 0;
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.Role model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string roleId)
        {
            return dal.Delete(roleId);
        }

        /// <summary>
        /// 更新频道是否可用
        /// </summary>
        public bool UpdateStatus(string roleId, int status)
        {
            return dal.UpdateStatus(roleId, status);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.Role GetModel(string id)
        {
            return dal.GetModel(id);
        }
        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere = "")
        {
            return dal.GetList(strWhere);
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
            item["name"] = dataRow["name"].ToString();
            item["type"] = int.Parse(dataRow["typeId"].ToString());
            item["isAdmin"] = int.Parse(dataRow["isAdmin"].ToString());
            item["status"] = int.Parse(dataRow["status"].ToString());
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
    }
}
