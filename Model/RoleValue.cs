namespace Model
{
    /// <summary>
    /// 用户角色管理类
    /// </summary>
    public class RoleValue
    {
        /// <summary>
        /// 自增主键ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 用户角色名称
        /// </summary>
        public string roleId { get; set; }
        /// <summary>
        /// 是否是系统角色
        /// </summary>
        public string menuId { get; set; } 
        /// <summary>
        /// 调用别名
        /// </summary>
        public string module { get; set; }
        /// <summary>
        /// 用户角色类型
        /// </summary>
        public string typeNumber { get; set; }

    }
}
