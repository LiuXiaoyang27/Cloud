using System;

namespace Model
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class Device
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 激活车架号
        /// </summary>
        public string VIN { get; set; }
        /// <summary>
        /// 激活次数
        /// </summary>
        public int TIMES { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string DESCRIPTION { get; set; }
        /// <summary>
        /// 创建时间/导入时间
        /// </summary>
        public DateTime CREATORTIME { get; set; }
        /// <summary>
        /// 创建/导入用户ID
        /// </summary>
        public string CREATORUSERID { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LASTMODIFYTIME { get; set; }
        /// <summary>
        /// 最后修改用户
        /// </summary>
        public string LASTMODIFYUSERID { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime DELETETIME { get; set; }
        /// <summary>
        /// 删除用户ID
        /// </summary>
        public string DELETEUSERID { get; set; }
        /// <summary>
        /// 删除标记 0：未删除 1： 已删除
        /// </summary>
        public int ISDELETE { get; set; }
        /// <summary>
        /// 激活时间
        /// </summary>
        public DateTime ACTIVATETIME { get; set; }
        /// <summary>
        /// 激活用户ID
        /// </summary>
        public string ACTIVATEUSERID { get; set; }

        public string SECRETKEY { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int STATUS { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string DEVTYPE { get; set; }

    }
}
