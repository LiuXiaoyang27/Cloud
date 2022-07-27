using System;

namespace Model
{
    /// <summary>
    /// 日志表
    /// </summary>
    public class Logs
    {
        /// <summary>
        /// 自增主键ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 操作人ID
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// 操作人用户名
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// 详细信息
        /// </summary>
        public string detail { get; set; }
        /// <summary>
        /// 操作人姓名
        /// </summary>
        public string nickName { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public int typeNum { get; set; }
        /// <summary>
        /// 操作类型名称
        /// </summary>
        public string typeName { get; set; }

        /// <summary>
        /// 级别
        /// </summary>
        public int infoLevel { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime modifyTime { get; set; }
    }
}
