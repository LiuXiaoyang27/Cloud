using System.Collections.Generic;

namespace Model
{
    /// <summary>
    /// 用户角色管理类
    /// </summary>
    public class Role
    {
        /// <summary>
        /// 自增主键ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 用户角色名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 是否是系统角色
        /// </summary>
        public int isAdmin { get; set; }
        /// <summary>
        /// 用户角色类型
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 目前状态
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public int isDelete { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public List<RoleValue> roleValues { get; set; }
    }
}
