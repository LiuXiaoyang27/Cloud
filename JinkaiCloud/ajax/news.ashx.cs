using Common;
using Controller;
using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.SessionState;

namespace Cloud.ajax
{
    /// <summary>
    /// Summary description for news
    /// </summary>
    public class news : IHttpHandler, IRequiresSessionState
    {

        // 频道别名
        private static string module = "newsData";

        public void ProcessRequest(HttpContext context)
        {
            //检查管理员是否登录
            if (!new ManagePage().IsAdminLogin())
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"尚未登录或已超时，请登录后操作！\"}");
                return;
            }

            string action = context.Request["action"];
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            try
            {
                switch (action)
                {
                    // 显示通知公告
                    case "list":
                        context.Response.Write(GetListJson(context));
                        break;
                    // 添加通知公告
                    case "add":
                        context.Response.Write(Add(context));
                        break;
                    // 初始化修改数据
                    case "query":
                        context.Response.Write(Query(context));
                        break;
                    // 修改
                    case "update":
                        context.Response.Write(Update(context));
                        break;
                    // 删除
                    case "delete":
                        context.Response.Write(Delete(context));
                        break;
                    // 批量删除
                    case "batchDelete":
                        context.Response.Write(DeleteBatch(context));
                        break;
                    // 修改状态
                    case "release":
                        context.Response.Write(Release(context));
                        break;
                    // 获得最新的10条公告信息
                    case "latest":
                        context.Response.Write(LatestNews(context));
                        break;
                    // 获得通知信息
                    case "message":
                        context.Response.Write(GetMessage(context));
                        break;
                    // 已读
                    case "msgStatus":
                        context.Response.Write(UpdateMsgStatus(context));
                        break;
                    // 
                    case "isClosed":
                        context.Response.Write(UpdateIsClosed());
                        break;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                context.Response.Write(JsonHelp.ErrorJson(msg));
            }
        }

        #region GetListJson:将获得的结果转换为json数据
        /// <summary>
        /// 将获得的结果转换为json数据
        /// </summary>
        /// <returns></returns>
        public string GetListJson(HttpContext context)
        {
            string page = context.Request["page"];
            string rows = context.Request["rows"];
            string matchCon = context.Request["matchCon"];
            string beginDate = context.Request["beginDate"];
            string endDate = context.Request["endDate"];

            string strWhere = "";
            if (!string.IsNullOrEmpty(matchCon))
            {
                strWhere += "and (t1.title like '%" + matchCon + "%' or t2.name like '%" + matchCon + "%')";
            }
            strWhere += !string.IsNullOrEmpty(beginDate) ? " and t1.newsDate >= '" + beginDate + " 00:00:00" + "'" : "";
            strWhere += !string.IsNullOrEmpty(endDate) ? " and t1.newsDate <= '" + endDate + " 23:59:59 " + "'" : "";

            NewsController controller = new NewsController();
            int records;
            DataSet data = controller.GetList(int.Parse(rows), int.Parse(page), strWhere, "t1.modifyTime desc", out records);
            int total = Utils.GetPageCount(int.Parse(rows), records);
            string json = JsonHelp.SuccessJson(controller.GetJsonList(data.Tables[0]), total, int.Parse(page), records);
            return json;
        }
        #endregion

        /// <summary>
        /// 获得最新的10条公告信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string LatestNews(HttpContext context)
        {
            string userId = new ManagePage().GetAdminInfo().id;
            string type = context.Request["type"];
            NewsController controller = new NewsController();
            DataSet data = controller.GetLatest();
            return JsonHelp.SuccessJson(controller.GetLatestJsonList(data.Tables[0]));
        }

        /// <summary>
        /// 获得推送通知信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetMessage(HttpContext context)
        {
            string userId = new ManagePage().GetAdminInfo().id;
            string type = context.Request["type"];
            NewsController controller = new NewsController();
            DataSet data = controller.GetMessage(userId);
            return JsonHelp.SuccessJson(controller.GetMessageJsonList(data.Tables[0]));
        }
        #region Add:新增通知公告
        /// <summary>
        /// 新增通知公告
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Add(HttpContext context)
        {
            bool right = new ManagePage().VerifyRight(module, "Add");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            string errMsg = "";
            News model = GetRequestModel(context, ref errMsg);

            if (model == null)
            {
                return JsonHelp.ErrorJson(errMsg);
            }
            Admin userInfo = new ManagePage().GetAdminInfo();
            string authorId = userInfo.id;
            model.authorId = authorId;
            model.author = userInfo.name;
            model.creatorAvatar = userInfo.avatar;
            if (model.status == 1)
            {
                model.newsDate = DateTime.Now.ToString();
            }
            else
            {
                model.newsDate = null;
            }
            NewsController controller = new NewsController();
            string result = controller.Add(model);
            if (!string.IsNullOrEmpty(result))
            {
                model.id = result;
                if (model.status == 1 && model.newsType == "1")
                {
                    string uids = context.Request["userIds"];
                    string[] ids = uids.Split(',');
                    List<string> userIds = new List<string>(ids);
                    List<Message> messageList = GetMessageList(model, userIds);
                    controller.AddList(messageList);
                }

                new ManagePage().AddAdminLog("新增通知公告 标题：" + model.title, Constant.ActionEnum.Add);
                return SuccessJson(model);
            }
            else
            {
                return JsonHelp.ErrorJson("新增通知公告失败");
            }

        }
        #endregion

        #region 添加/修改之后，返回的JSON数据
        /// <summary>
        /// 添加/修改之后，返回的JSON数据
        /// </summary>
        public string SuccessJson(News model)
        {
            JObject data = JObject.Parse(JsonConvert.SerializeObject(model));
            JObject result = new JObject();
            result["status"] = 200;
            result["msg"] = "success";
            result["data"] = data;
            return result.ToString();
        }
        #endregion

        #region Query:根据id 查询信息
        /// <summary>
        ///  根据id 查询信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Query(HttpContext context)
        {
            string id = context.Request["id"];
            if (string.IsNullOrEmpty(id))
            {
                return JsonHelp.ErrorJson("参数Id错误");
            }
            NewsController controller = new NewsController();
            News model = controller.GetModel(id);
            if (model != null)
            {
                JObject result = new JObject();
                result["status"] = 200;
                result["msg"] = "success";
                JObject obj = controller.GetJsonObj(model);
                result["data"] = obj;
                return result.ToString();
            }

            return JsonHelp.ErrorJson("该单据不存在,刷新后重试");

        }
        #endregion

        #region Update:修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Update(HttpContext context)
        {
            bool right = new ManagePage().VerifyRight(module, "Edit");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            string id = context.Request["id"];

            if (string.IsNullOrEmpty(id))
            {
                return JsonHelp.ErrorJson("id不能为空");
            }
            NewsController controller = new NewsController();
            News temp = controller.GetModel(id);
            if (temp == null)
            {
                return JsonHelp.ErrorJson("不存在该通告信息");
            }
            string errMsg = "";
            News model = GetRequestModel(context, ref errMsg);
            if (model == null)
            {
                return JsonHelp.ErrorJson(errMsg);
            }

            Admin userInfo = new ManagePage().GetAdminInfo();
            model.authorId = userInfo.id;
            model.author = userInfo.name;
            model.id = id;
            model.creatorAvatar = userInfo.avatar;
            if (model.status == 1)
            {
                model.newsDate = DateTime.Now.ToString();
            }
            else
            {
                model.newsDate = null;
            }
            if (controller.Update(model))
            {
                if (model.status == 1 && model.newsType == "1")
                {
                    string uids = context.Request["userIds"];
                    string[] ids = uids.Split(',');
                    List<string> userIds = new List<string>(ids);
                    List<Message> messageList = GetMessageList(model, userIds);
                    controller.AddList(messageList);
                }
                new ManagePage().AddAdminLog("修改通知公告 标题：" + model.title, Constant.ActionEnum.Add);
                return SuccessJson(model);
            }
            else
            {
                return JsonHelp.ErrorJson("更新失败");
            }
        }
        #endregion

        #region GetRequestModel:获得表单信息
        /// <summary>
        /// 获得表单信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public News GetRequestModel(HttpContext context, ref string errMsg)
        {
            string title = Utils.UrlDecode(context.Request["title"]);
            string content = HttpUtility.UrlDecode(context.Request["content"]);
            string newsType = context.Request["newsType"];
            string status = context.Request["status"];
            News model = new Model.News();
            if (string.IsNullOrEmpty(title))
            {
                errMsg = "标题不能为空";
                return null;
            }
            if (string.IsNullOrEmpty(content))
            {
                errMsg = "内容不能为空";
                return null;
            }
            if (title.Length > 50)
            {
                errMsg = "标题字符超限（不超过50字）";
                return null;
            }
            if (string.IsNullOrEmpty(status))
            {
                status = "0";
            }

            model.title = title;
            model.content = content;
            model.newsType = newsType;
            model.status = int.Parse(status);
            model.modifyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return model;
        }
        #endregion

        #region Delete:删除一条数据
        /// <summary>
        /// 删除一条数据
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
            string id = context.Request["id"];
            if (string.IsNullOrEmpty(id))
            {
                return JsonHelp.ErrorJson("参数错误");
            }

            NewsController controller = new NewsController();
            News dataRow = controller.GetModel(id);
            if (dataRow != null)
            {
                bool result = controller.Delete("'" + id + "'");
                if (result)
                {
                    controller.DeleteMsg("'" + id + "'");
                    new ManagePage().AddAdminLog("删除通知公告 标题:" + dataRow.title, Constant.ActionEnum.Delete);
                    return JsonHelp.SuccessJson("删除成功");
                }
                else
                {
                    return JsonHelp.ErrorJson("删除失败");
                }
            }

            return JsonHelp.ErrorJson("该单据不存在,刷新后重试");
        }
        #endregion

        #region DeleteBatch:批量删除通告数据
        /// <summary>
        /// 批量删除通告数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string DeleteBatch(HttpContext context)
        {
            bool right = new ManagePage().VerifyRight(module, "Delete");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            string strId = context.Request["id"];
            if (string.IsNullOrEmpty(strId))
            {
                return JsonHelp.ErrorJson("参数错误");
            }

            NewsController controller = new NewsController();

            string[] ids = strId.Split(',');
            string delIds = "";
            for (int i = 0; i < ids.Length; i++)
            {
                delIds += ",'" + ids[i] + "'";
            }
            delIds = delIds.Substring(1);

            bool result = controller.Delete(delIds);
            if (result)
            {
                controller.DeleteMsg(delIds);
                new ManagePage().AddAdminLog("删除通告信息 id:" + delIds, Constant.ActionEnum.Delete);
                return JsonHelp.SuccessJson("删除成功");
            }
            else
            {
                return JsonHelp.ErrorJson("删除失败");
            }

        }
        #endregion

        #region Release:更改状态
        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Release(HttpContext context)
        {
            bool right = new ManagePage().VerifyRight(module, "Enabled");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            string strId = context.Request["newsId"];
            string status = context.Request["status"];
            if (string.IsNullOrEmpty(strId) || string.IsNullOrEmpty(status))
            {
                return JsonHelp.ErrorJson("参数错误");
            }
            NewsController controller = new NewsController();

            string newsid = "'" + strId + "'";
            int state = int.Parse(status);
            string time = "";
            if (state == 1)
            {
                time = DateTime.Now.ToString();
            }
            else
            {
                time = null;
            }
            if (controller.Release(newsid, state, time))
            {
                News news = controller.GetModel(strId);
                if (news != null && news.newsType == "1")
                {
                    if (state == 1)//发布
                    {
                        string uids = context.Request["userIds"];
                        string[] ids = uids.Split(',');
                        List<string> userIds = new List<string>(ids);
                        List<Message> messageList = GetMessageList(news, userIds);
                        if (controller.AddList(messageList))
                        {
                            // 添加日志记录
                            string action = "发布通知";
                            new ManagePage().AddAdminLog(action + " id:" + strId, Constant.ActionEnum.Enabled);
                            return JsonHelp.SuccessJson("发布成功");
                        }
                        else
                        {
                            return JsonHelp.ErrorJson("发布失败");
                        }
                    }
                    else //取消发布
                    {
                        if (controller.DeleteMsg(newsid))
                        {
                            // 添加日志记录
                            string action = "取消发布通知";
                            new ManagePage().AddAdminLog(action + " id:" + strId, Constant.ActionEnum.Enabled);
                            return JsonHelp.SuccessJson("已取消发布");
                        }
                        else
                        {
                            return JsonHelp.ErrorJson("取消发布失败");
                        }
                    }

                }
                else
                {
                    // 添加日志记录
                    string msg = int.Parse(status) == 1 ? "发布成功" : "已取消发布";
                    string action = int.Parse(status) == 1 ? "发布公告" : "取消发布公告";
                    new ManagePage().AddAdminLog(action + " id:" + strId, Constant.ActionEnum.Enabled);
                    return JsonHelp.SuccessJson(msg);
                }


            }
            else
            {
                return JsonHelp.ErrorJson("更改状态失败");
            }

        }
        #endregion

        #region Read:更改信息为已读
        /// <summary>
        /// 更改信息为已读
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string UpdateMsgStatus(HttpContext context)
        {
            string msgid = context.Request["msgid"];
            string field = context.Request["field"];
            string val = context.Request["value"];
            NewsController controller = new NewsController();

            if (controller.UpdateMsgStatus(msgid, field, val))
            {
                return JsonHelp.SuccessJson("修改成功");
            }
            else
            {
                return JsonHelp.ErrorJson("修改状态失败");
            }

        }
        public string UpdateIsClosed()
        {
            NewsController controller = new NewsController();
            string userId = new ManagePage().GetAdminInfo().id;
            if (controller.UpdateIsClosed(userId))
            {
                return JsonHelp.SuccessJson("修改成功");
            }
            else
            {
                return JsonHelp.ErrorJson("修改状态失败");
            }
        }
        #endregion

        public List<Message> GetMessageList(News news, List<string> userIds)
        {
            List<Message> modelList = new List<Message>();
            foreach (string item in userIds)
            {
                Message model = new Message();
                model.userId = item;
                model.newsId = news.id;
                model.creatorTime = news.newsDate;
                model.creatorUserId = news.authorId;
                model.creatorAvatar = news.creatorAvatar;
                modelList.Add(model);
            }
            return modelList;
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