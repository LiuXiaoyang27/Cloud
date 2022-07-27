namespace Model
{
    public class Message
    {

        /// <summary>
        /// 自增主键ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// userId
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// 新闻Id
        /// </summary>
        public string newsId { get; set; }
        /// <summary>
        /// 是否已读
        /// </summary>
        public string isRead { get; set; }
        /// <summary>
        /// 发布日期
        /// </summary>
        public string creatorTime { get; set; }

        public string creatorUserId { get; set; }

        public string isClosed { get; set; }

        public string creatorAvatar { get; set; }

    }
}
