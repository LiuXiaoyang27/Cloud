using Business;
using Common;
using Controller;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.SessionState;

namespace Cloud.ajax
{
    /// <summary>
    /// 类别信息的摘要说明
    /// </summary>
    public class dictionaryType : IHttpHandler, IRequiresSessionState
    {
        // 频道别名
        private static string module = "dictionary";
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
                    // 树形结构排列显示信息
                    case "treeView":
                        context.Response.Write(GetListJson(context));
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
                    // 检查编号是否重复
                    case "checkFullName":
                        context.Response.Write(CheckFullName(context));
                        break;
                    // 检查编码是否重复
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
        /// 获得树形结构
        /// </summary>
        /// <returns></returns>
        public string GetListJson(HttpContext context)
        {
            string type = context.Request["type"];
            DictionaryTypeController controller = new DictionaryTypeController();
            DictionaryTypeBll bll = new DictionaryTypeBll();
            string strWhere = "";
            strWhere += " ORDER BY SORTCODE , LASTMODIFYTIME desc";
            DataTable data = controller.GetList(strWhere);
            List<DictionaryType> list = new List<DictionaryType>();
            foreach (DataRow dataRow in data.Rows)
            {
                list.Add(bll.DataRowToModel(dataRow));
            }
            return JsonHelp.SuccessJson(controller.GetTreeView(list,type));

        }
        /// <summary>
        /// 信息
        /// </summary>
        /// <returns></returns>
        public string GetInfo(HttpContext context)
        {
            DictionaryTypeController controller = new DictionaryTypeController();
            string id = context.Request["id"];
            DataTable data = controller.GetInfo(id);
            return JsonHelp.SuccessJson(controller.RowToJson(data.Rows[0]));
        }
        /// <summary>
        /// 检查名称是否重复
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string CheckFullName(HttpContext context)
        {
            string FullName = Utils.UrlDecode(context.Request["FullName"]);
            string id = context.Request["id"];
            if (string.IsNullOrEmpty(FullName))
            {
                return JsonHelp.ErrorJson("参数错误");
            }
            string strWhere = "";
            DictionaryTypeController controller = new DictionaryTypeController();
            if (string.IsNullOrEmpty(id))
            {
                strWhere = " FULLNAME='" + FullName + "' and ISDELETE = 0";
            }
            else
            {
                strWhere = " FULLNAME='" + FullName + "' and ISDELETE = 0 and ID <>  '" + id + "'";
            }
            bool hasName = controller.IsExist(strWhere);
            return JsonHelp.SuccessJson(hasName);
        }
        /// <summary>
        /// 检查编码是否重复。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string CheckEnCode(HttpContext context)
        {

            string EnCode = context.Request["EnCode"];
            string id = context.Request["id"];
            if (string.IsNullOrEmpty(EnCode))
            {
                return JsonHelp.ErrorJson("参数错误");
            }
            string strWhere = "";
            DictionaryTypeController controller = new DictionaryTypeController();
            if (string.IsNullOrEmpty(id))
            {
                strWhere = " ENCODE='" + EnCode + "' and ISDELETE = 0";
            }
            else
            {
                strWhere = " ENCODE='" + EnCode + "' and ISDELETE = 0 and ID <>  '" + id + "'";
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
            DictionaryTypeController controller = new DictionaryTypeController();
            // 判断请求参数是否正确
            string id = context.Request["id"];
            if (string.IsNullOrEmpty(id))
            {
                return JsonHelp.ErrorJson("请求参数错误");
            }
            // 判断类别是否存在
            Model.DictionaryType model = controller.GetModel(id);
            if (model == null)
            {
                return JsonHelp.ErrorJson("字典分类不存在，刷新重试");
            }
            model.ID = id;
            model.DELETEUSERID = admin.id;
            if (controller.Delete(model))
            {
                new ManagePage().AddAdminLog("删除字典分类 名称:" + model.FULLNAME, Constant.ActionEnum.Delete);
                return JsonHelp.SuccessJson("删除成功");
            }
            else
            {
                return JsonHelp.ErrorJson("删除失败");
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
            DictionaryTypeController controller = new DictionaryTypeController();
            DictionaryType model = new DictionaryType();
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
                new ManagePage().AddAdminLog("新增字典分类 名称：" + model.FULLNAME, Constant.ActionEnum.Add);
                return JsonHelp.SuccessJson(model);
            }
            else
            {
                return JsonHelp.ErrorJson("新增失败");
            }

        }
        /// <summary>
        /// 获得请求参数
        /// </summary>
        /// <param name="context"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public DictionaryType GetRequestModel(HttpContext context, ref string errMsg)
        {
            DictionaryType model = new DictionaryType();
            string parentId = Utils.UrlDecode(context.Request["parentId"]);
            string fullName = Utils.UrlDecode(context.Request["fullName"]);
            string isTree = Utils.UrlDecode(context.Request["IsTree"]);
            string enCode = Utils.UrlDecode(context.Request["enCode"]);
            string description = Utils.UrlDecode(context.Request["Description"]);
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
            DictionaryTypeController controller = new DictionaryTypeController();
            string strCheckFullName = "";
            string strCheckEnCode = "";
            if (!string.IsNullOrEmpty(id))
            {
                strCheckFullName = " FULLNAME='" + fullName + "' and id <> '" + id + "'";
                strCheckEnCode = " ENCODE='" + enCode + "' and id <>  '" + id + "'";
            }
            else
            {
                strCheckFullName = " FULLNAME='" + fullName + "'";
                strCheckEnCode = " ENCODE='" + fullName + "'";
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
            //给类赋值
            model.PARENTID = parentId;
            model.FULLNAME = fullName;
            model.ENCODE = enCode;
            model.DESCRIPTION = description;
            model.ISTREE = int.Parse(isTree);
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
            if (string.IsNullOrEmpty(id))
            {
                return JsonHelp.ErrorJson("参数错误");
            }
            DictionaryTypeController controller = new DictionaryTypeController();
            string errMsg = "";
            Model.DictionaryType model = GetRequestModel(context, ref errMsg);
            if (model == null)
            {
                return JsonHelp.ErrorJson(errMsg);
            }
            Model.DictionaryType temp = controller.GetModel(id);
            if (temp == null)
            {
                return JsonHelp.ErrorJson("不存在该信息");
            }
            //转换为拼音
            model.SIMPLESPELLING = ChineseSpell.MakeSpellCode(model.FULLNAME, "", SpellOptions.FirstLetterOnly).ToUpper();
            model.ID = id;
            model.LASTMODIFYUSERID = admin.id;
            if (controller.Update(model))
            {
                new ManagePage().AddAdminLog("更新字典分类 名称：" + model.FULLNAME, Constant.ActionEnum.Edit);
                return JsonHelp.SuccessJson(model);
            }
            else
            {
                return JsonHelp.ErrorJson("更新失败");
            }

        }
        /// <summary>
        /// 上移
        /// </summary>
        /// <returns></returns>
        public string First(HttpContext context)
        {
            DictionaryTypeController controller = new DictionaryTypeController();
            DictionaryType model = new DictionaryType();
            string id = context.Request["id"];
            model.ID = id;
            model.LASTMODIFYUSERID = admin.id;
            bool result = controller.First(model);
            return result ? JsonHelp.SuccessJson("操作成功") : JsonHelp.ErrorJson("操作失败，稍后重试！");


        }
        /// <summary>
        /// 下移
        /// </summary>
        /// <returns></returns>
        public string Next(HttpContext context)
        {
            DictionaryTypeController controller = new DictionaryTypeController();
            DictionaryType model = new DictionaryType();
            string id = context.Request["id"];
            model.ID = id;
            model.LASTMODIFYUSERID = admin.id;
            bool result = controller.Next(model);
            return result ? JsonHelp.SuccessJson("操作成功") : JsonHelp.ErrorJson("操作失败，稍后重试！");
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