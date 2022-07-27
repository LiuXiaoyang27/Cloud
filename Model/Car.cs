using System;

namespace Model
{
    /// <summary>
    /// 车辆信息
    /// </summary>
    public class Car
    {
        /// <summary>
        /// 车架号
        /// </summary>
        public string VIN { get; set; }
        /// <summary>
        /// 车辆信息
        /// </summary>
        public string CARDATA { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public string DEVID { get; set; }
        /// <summary>
        /// 激活密钥
        /// </summary>
        public string SECRETKEY { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string DESCRIPTION { get; set; }
        /// <summary>
        /// 创建时间/导入时间
        /// </summary>
        public DateTime CREATORTIME { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string CARTYPE { get; set; }
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


        public string WRITEUSERID { get; set; }

        public string ACTIVATEUSERID { get; set; }
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
        /// 状态
        /// </summary>
        public int STATUS { get; set; }

    }
}
