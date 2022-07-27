using System;

namespace Model
{
    /// <summary>
    /// 单据规则类
    /// </summary>
	public class BillRule
    {
        /// <summary>
        /// 自增Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 业务名称
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 业务编码
        /// </summary>
        public string EnCode { get; set; }
        /// <summary>
        /// 流水前缀
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// 流水日期
        /// </summary>
        public string DateFormat { get; set; }
        /// <summary>
        /// 流水位数
        /// </summary>
        public int? Digit { get; set; }
        /// <summary>
        /// 流水起始
        /// </summary>
        public string StartNumber { get; set; }
        /// <summary>
        /// 流水示例
        /// </summary>
        public string Example { get; set; }
        /// <summary>
        /// 流水数量
        /// </summary>
        public int? ThisNumber { get; set; }
        /// <summary>
        /// 当前流水号
        /// </summary>
        public string OutputNumber { get; set; }
        /// <summary>
        /// 流水说明
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        public string SortCode { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? EnabledMark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatorTime { get; set; }
        /// <summary>
        /// 创建人员
        /// </summary>
        public int? CreatorUserId { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? LastModifyTime { get; set; }
        /// <summary>
        /// 修改人员Id
        /// </summary>
        public int? LastModifyUserId { get; set; }
        /// <summary>
        /// 删除数据的时间
        /// </summary>
        public DateTime? DeleteTime { get; set; }
        /// <summary>
        /// 删除的人员Id
        /// </summary>
        public int? DeleteUserId { get; set; }
        /// <summary>
        /// 删除状态：0未删除，1已删除
        /// </summary>
        public int? DeleteMark { get; set; }
    }

}
