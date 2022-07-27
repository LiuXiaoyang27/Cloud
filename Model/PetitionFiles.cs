
namespace Model
{
    /// <summary>
    /// 设备图片表
    /// </summary>
    public class PetitionFiles
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public string petitionId { get; set; }
        /// <summary>
        /// 图片类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 图片URL
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 缩略图URL
        /// </summary>
        public string thumbnailUrl { get; set; }
        /// <summary>
        /// 删除图片URL
        /// </summary>
        public string deleteUrl { get; set; }
        /// <summary>
        /// 删除图片方式
        /// </summary>
        public string deleteType { get; set; }
        /// <summary>
        /// 图片状态
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 图片大小
        /// </summary>
        public int size { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public int isDelete { get; set; }

    }
}
