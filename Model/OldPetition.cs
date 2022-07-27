namespace Model
{
    /// <summary>
    /// 信访条例类
    /// </summary>
    public class OldPetition
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
        /// <summary>
        /// 案件名
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 案件时间
        /// </summary>
        public string pDate { get; set; }
        /// <summary>
        /// 当事人
        /// </summary>
        public string pName { get; set; }
        /// <summary>
        /// 办理人
        /// </summary>
        public string attendName { get; set; }
    }
}
