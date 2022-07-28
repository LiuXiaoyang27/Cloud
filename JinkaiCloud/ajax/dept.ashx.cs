using Business;
using Common;
using Controller;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.SessionState;

namespace JinkaiCloud.ajax
{
    /// <summary>
    /// 部门信息的摘要说明
    /// </summary>
    public class dept : IHttpHandler, IRequiresSessionState
    {
        // 频道别名
        private static string module = "dept";
        // 保存用户信息
        private Model.Admin admin = null;
        public void ProcessRequest(HttpContext context)
        {
            //检查管理员是否登录
            ManagePage page = new ManagePage();
            if (!page.IsAdminLogin())
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"尚未登录或已超时，请登录后操作！\"}");
                return;
            }
            else
            {
                admin = page.GetAdminInfo();
            }

            string action = context.Request["action"];
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            try
            {
                switch (action)
                {
                    // 部门信息
                    case "list":
                        context.Response.Write(GetList(context));
                        break;
                    // 树形结构
                    case "treeView":
                        context.Response.Write(GetTreeView(context));
                        break;
                    // 信息
                    case "info":
                        context.Response.Write(GetInfo(context));
                        break;
                    // 删除
                    case "delete":
                        context.Response.Write(Delete(context));
                        break;
                    // 新增
                    case "add":
                        context.Response.Write(Add(context));
                        break;
                    // 修改
                    case "update":
                        context.Response.Write(Update(context));
                        break;
                    // 检查名称是否存在
                    case "checkFullName":
                        context.Response.Write(CheckFullName(context));
                        break;
                    // 检查编码是否存在
                    case "checkEnCode":
                        context.Response.Write(CheckEnCode(context));
                        break;
                    // 上移
                    case "first":
                        context.Response.Write(First(context));
                        break;
                    // 下移
                    case "next":
                        context.Response.Write(Next(context));
                        break;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                context.Response.Write(JsonHelp.ErrorJson(msg));
            }
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        public string GetList(HttpContext context)
        {
            DeptController controller = new DeptController();
            DeptBll bll = new DeptBll();
            string skey = context.Request["skey"];
            string strWhere = "";
            if (!string.IsNullOrEmpty(skey))
            {
                strWhere += " AND FULLNAME like '%" + skey + "%' or ENCODE like '%" + skey + "%'";
            }
            DataTable data = controller.GetList(strWhere);
            List<Dept> list = new List<Dept>();
            foreach (DataRow dataRow in data.Rows)
            {
                list.Add(bll.DataRowToModel(dataRow));
            }
            return JsonHelp.SuccessJson(controller.ListToGridTreeJson(list));
        }
        /// <summary>
        /// 树形结构
        /// </summary>
        /// <returns></returns>
        public string GetTreeView(HttpContext context)
        {
            string category = context.Request["Category"];
            string strWhere = "";
            if (!string.IsNullOrEmpty(category) && category.Equals("city"))
            {
                strWhere += " AND Category = '" + category + "'";
            }
            DeptController controller = new DeptController();
            //DeptBll bll = new DeptBll();
            //DataTable data = controller.GetList("");
            //List<Dept> list = new List<Dept>();
            //foreach (DataRow dataRow in data.Rows)
            //{
            //    list.Add(bll.DataRowToModel(dataRow));
            //}
            List<Dept> list = controller.GetListModel(strWhere);
            //string type = context.Request["type"];

            //if (string.IsNullOrEmpty(type))
            //{
            //    type = "";
            //}

            return JsonHelp.SuccessJson(controller.ListToTreeJson(list, category));

        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <returns></returns>
        public string GetInfo(HttpContext context)
        {
            DeptController controller = new DeptController();
            string id = context.Request["id"];
            DataTable data = controller.GetInfo(id);
            return JsonHelp.SuccessJson(controller.RowToJson(data.Rows[0]));
        }
        /// <summary>
        /// 检查名称是否存在
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string CheckFullName(HttpContext context)
        {
            string FullName = Utils.UrlDecode(context.Request["FullName"]);
            string id = context.Request["id"];
            string parentId = context.Request["parentId"];
            if (!string.IsNullOrEmpty(parentId))
            {
                parentId = parentId == "-1" ? "0" : parentId;
            }
            else
            {
                parentId = "-1";
            }
            if (string.IsNullOrEmpty(FullName))
            {
                return JsonHelp.ErrorJson("参数错误");
            }
            string strWhere = "";
            DeptController controller = new DeptController();
            if (string.IsNullOrEmpty(id))
            {
                strWhere = " FULLNAME='" + FullName + "' and PARENTID = '" + parentId + "'";
            }
            else
            {
                strWhere = " FULLNAME='" + FullName + "' and ID <> '" + id + "' and PARENTID = '" + parentId + "'";
            }
            bool hasName = controller.IsExist(strWhere);
            return JsonHelp.SuccessJson(hasName);
        }
        /// <summary>
        /// 检查编码是否存在。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string CheckEnCode(HttpContext context)
        {
            string EnCode = context.Request["EnCode"];
            string id = context.Request["id"];
            string parentId = context.Request["parentId"];
            if (!string.IsNullOrEmpty(parentId))
            {
                parentId = parentId == "-1" ? "0" : parentId;
            }
            else
            {
                parentId = "-1";
            }
            if (string.IsNullOrEmpty(EnCode))
            {
                return JsonHelp.ErrorJson("参数错误");
            }
            string strWhere = "";
            DeptController controller = new DeptController();
            if (string.IsNullOrEmpty(id))
            {
                strWhere = " ENCODE='" + EnCode + "' and PARENTID = '" + parentId + "'";
            }
            else
            {
                strWhere = " ENCODE='" + EnCode + "' and ID <> '" + id + "' and PARENTID = '" + parentId + "'";
            }

            bool isExists = controller.IsExist(strWhere);
            return JsonHelp.SuccessJson(isExists);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Delete(HttpContext context)
        {
            bool right = new ManagePage().VerifyRight(module, "Delete");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            DeptController controller = new DeptController();
            // 判断请求参数是否正确
            string id = context.Request["id"];
            if (string.IsNullOrEmpty(id))
            {
                return JsonHelp.ErrorJson("请求参数错误");
            }
            // 判断部门是否存在
            Dept model = controller.GetModel(id);
            if (model == null)
            {
                return JsonHelp.ErrorJson("部门不存在，刷新重试");
            }
            model.DELETEUSERID = admin.id;
            model.ID = id;
            if (controller.Delete(model))
            {
                new ManagePage().AddAdminLog("删除部门 部门名称:" + model.FULLNAME, Constant.ActionEnum.Delete);
                return JsonHelp.SuccessJson("删除部门成功");
            }
            else
            {
                return JsonHelp.ErrorJson("删除部门失败");
            }

        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Add(HttpContext context)
        {
            bool right = new ManagePage().VerifyRight(module, "Add");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            DeptController controller = new DeptController();
            Dept model;
            string errMsg = "";
            model = GetRequestModel(context, ref errMsg);
            if (model == null)
            {
                return JsonHelp.ErrorJson(errMsg);
            }
            //转换为拼音
            model.SIMPLESPELLING = ChineseSpell.MakeSpellCode(model.FULLNAME, "", SpellOptions.FirstLetterOnly).ToUpper();
            model.CREATORUSERID = admin.id;
            //执行添加操作
            int result = controller.Create(model);
            if (result > 0)
            {
                new ManagePage().AddAdminLog("新增部门 部门名称：" + model.FULLNAME, Constant.ActionEnum.Add);
                return JsonHelp.SuccessJson(model);
            }
            else
            {
                return JsonHelp.ErrorJson("新增部门失败");
            }

        }
        /// <summary>
        /// 获得请求参数
        /// </summary>
        /// <param name="context"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public Dept GetRequestModel(HttpContext context, ref string errMsg)
        {
            Dept model = new Dept();
            string parentId = Utils.UrlDecode(context.Request["parentId"]);
            string fullName = Utils.UrlDecode(context.Request["fullName"]);
            string enCode = Utils.UrlDecode(context.Request["enCode"]);
            string enabledMark = Utils.UrlDecode(context.Request["EnabledMark"]);
            string description = Utils.UrlDecode(context.Request["Description"]);
            string mobile = context.Request["mobile"];
            string tel = Utils.UrlDecode(context.Request["tel"]);
            //新添加字段
            string category = Utils.UrlDecode(context.Request["Category"]);
            string propertyJson = Utils.UrlDecode(context.Request["PropertyJson"]);
            string id = context.Request["id"];
            if (string.IsNullOrEmpty(parentId))
            {
                errMsg = "上级不能为空";
                return null;
            }

            if (string.IsNullOrEmpty(fullName))
            {
                errMsg = "名称不能为空";
                return null;
            }

            if (string.IsNullOrEmpty(enCode))
            {
                errMsg = "编码不能为空";
                return null;
            }
            DeptController controller = new DeptController();
            string strCheckFullName;
            string strCheckEnCode;
            if (!string.IsNullOrEmpty(id))
            {
                strCheckFullName = " FULLNAME='" + fullName + "' and ID <> '" + id + "' and PARENTID = '" + parentId + "'";
                strCheckEnCode = " ENCODE='" + enCode + "' and ID <> '" + id + "' and PARENTID = '" + parentId + "'";
            }
            else
            {
                strCheckFullName = " FULLNAME='" + fullName + "' and PARENTID = '" + parentId + "'";
                strCheckEnCode = " ENCODE='" + fullName + "' and PARENTID = '" + parentId + "'";
            }
            // 验证名称是否重复
            bool hasFullName = controller.IsExist(strCheckFullName);

            if (hasFullName)
            {
                errMsg = "该名称已经被使用。";
                return null;
            }
            //验证编码是否重复
            bool hasEnCode = controller.IsExist(strCheckEnCode);

            if (hasEnCode)
            {
                errMsg = "该编码已经被使用。";
                return null;
            }

            //验证手机号码
            if (!string.IsNullOrEmpty(mobile))
            {
                if (!Utils.IsMobile(mobile))
                {
                    errMsg = "手机号格式不正确。";
                    return null;
                }
            }

            //验证固话
            if (!string.IsNullOrEmpty(tel))
            {
                if (!Utils.IsTelephone(tel))
                {
                    errMsg = "固话格式不正确。";
                    return null;
                }
            }


            //给类赋值
            model.PARENTID = parentId;
            model.FULLNAME = fullName;
            model.ENCODE = enCode;
            model.DESCRIPTION = description;
            model.ENABLEDMARK = int.Parse(enabledMark);
            model.MOBILE = mobile;
            model.TEL = tel;
            model.CATEGORY = category;
            model.PROPERTYJSON = propertyJson;
            return model;

        }
        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Update(HttpContext context)
        {
            bool right = new ManagePage().VerifyRight(module, "Edit");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }

            // 获得请求参数
            string id = context.Request["id"];
            string TypeId = context.Request["TypeId"];
            if (string.IsNullOrEmpty(id))
            {
                return JsonHelp.ErrorJson("参数错误");
            }
            DeptController controller = new DeptController();
            string errMsg = "";
            Dept model = GetRequestModel(context, ref errMsg);
            if (model == null)
            {
                return JsonHelp.ErrorJson(errMsg);
            }
            Dept temp = controller.GetModel(id);
            if (temp == null)
            {
                return JsonHelp.ErrorJson("不存在该部门信息");
            }
            //转换为拼音
            model.SIMPLESPELLING = ChineseSpell.MakeSpellCode(model.FULLNAME, "", SpellOptions.FirstLetterOnly).ToUpper();
            model.ID = id;
            model.LASTMODIFYUSERID = admin.id;
            if (controller.Update(model))
            {
                new ManagePage().AddAdminLog("更新部门 部门名称：" + model.FULLNAME, Constant.ActionEnum.Edit);
                return JsonHelp.SuccessJson(model);
            }
            else
            {
                return JsonHelp.ErrorJson("更新部门失败");
            }

        }
        /// <summary>
        /// 上移
        /// </summary>
        /// <returns></returns>
        public string First(HttpContext context)
        {
            DeptController controller = new DeptController();
            Dept model = new Dept();
            string id = context.Request["id"];
            model.ID = id;
            model.LASTMODIFYUSERID = admin.id;
            bool result = controller.First(model);
            return result ? JsonHelp.SuccessJson("操作成功") : JsonHelp.ErrorJson("操作失败");
        }
        /// <summary>
        /// 下移
        /// </summary>
        /// <returns></returns>
        public string Next(HttpContext context)
        {
            DeptController controller = new DeptController();
            Dept model = new Dept();
            string id = context.Request["id"];
            model.ID = id;
            model.LASTMODIFYUSERID = admin.id;
            bool result = controller.Next(model);
            return result ? JsonHelp.SuccessJson("操作成功") : JsonHelp.ErrorJson("操作失败");

        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}