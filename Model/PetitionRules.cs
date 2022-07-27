namespace Model
{
    /// <summary>
    /// 信访条例类
    /// </summary>
    public class PetitionRules
    {
        /// <summary>
        /// 自增主键ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public string fileType { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public int fileSize { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string filePath { get; set; }
        /// <summary>
        /// 文件状态
        /// </summary>
        public string fileState { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public string modifyTime { get; set; }
    }
}
