using System;

namespace Model
{
    /// <summary>
    /// 菜单类
    /// </summary>
    public class Menu
    {
        /// <summary>
        /// 自增主键ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 类别名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 父类ID
        /// </summary>
        public string parentId { get; set; }
        /// <summary>
        /// 类别级别
        /// </summary>
        public int level { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int ordnum { get; set; }
        /// <summary>
        /// 类别分类，关键字段
        /// </summary>
        public string typeNumber { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 调用别名
        /// </summary>
        public string module { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string fontIcon { get; set; }
        /// <summary>
        /// 目前状态
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 排序ID
        /// </summary>
        public int sortIndex { get; set; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public string linkUrl { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public int isDelete { get; set; }
        /// <summary>
        /// 调整时间
        /// </summary>
        public DateTime modifyTime { get; set; }
        /// <summary>
        /// 链接目标
        /// </summary>
        public string LinkTarget { get; set; }
        /// <summary>
        /// 导航类型
        /// </summary>
        public string navType { get; set; }
        /// <summary>
        /// 是否为叶子节点
        /// </summary>
        public bool isLeaf { get; set; }

    }
}
