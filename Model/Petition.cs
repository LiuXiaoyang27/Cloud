using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 信访案件类
    /// </summary>
    public class Petition
    {
        /// <summary>
        /// 自增主键ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string createDate { get; set; }
        /// <summary>
        /// 当事人姓名
        /// </summary>
        public string pName { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string pIdCard { get; set; }
        /// <summary>
        /// 家庭住址
        /// </summary>
        public string pAddress { get; set; }
        /// <summary>
        /// 案件类型
        /// </summary>
        public string caseType { get; set; }
        /// <summary>
        /// 案件名称
        /// </summary>
        public string caseName { get; set; }
        /// <summary>
        /// 案件种类
        /// </summary>
        public string caseCategory { get; set; }
        /// <summary>
        /// 案件来源
        /// </summary>
        public string caseSource { get; set; }
        /// <summary>
        /// 来访渠道
        /// </summary>
        public string channels { get; set; }
        /// <summary>
        /// 接访人
        /// </summary>
        public string receiver { get; set; }
        /// <summary>
        /// 期限
        /// </summary>
        public int rerm { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public string modifyTime { get; set; }
        /// <summary>
        /// 扩展字段1
        /// </summary>
        public string ext1 { get; set; }
        /// <summary>
        /// 扩展字段2
        /// </summary>
        public string ext2 { get; set; }
        /// <summary>
        /// 扩展字段3
        /// </summary>
        public string ext3 { get; set; }
        /// <summary>
        /// 扩展字段4
        /// </summary>
        public string ext4 { get; set; }
        /// <summary>
        /// 扩展字段5
        /// </summary>
        public string ext5 { get; set; }

    }
}
