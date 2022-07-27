namespace Model
{
    /// <summary>
    /// 通知公告类
    /// </summary>
    public class News
    {
        /// <summary>
        /// 自增主键ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 作者Id
        /// </summary>
        public string authorId { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string author { get; set; }
        /// <summary>
        /// 发布日期
        /// </summary>
        public string newsDate { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public string modifyTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int status { get; set; }


        public string newsType { get; set; }

        public string typeName { get; set; }

        public string creatorAvatar { get; set; }
    }
}
