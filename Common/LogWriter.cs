using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class LogWriter
    {
        /// <summary>
        /// 日志输出目标文件夹路径
        /// </summary>
        public static string _LogPath = "/temp/logs/";

        /// <summary>
        /// 日志输出类型
        /// </summary>
        public enum LogType
        {
            Err = 0,
            Evt = 1
        }
        #region 公开方法

        #region PutErrLog：错误日志输出
        /// <summary>
        /// 错误日志输出
        /// </summary>
        /// <param name="classNm">类或模块名称</param>
        /// <param name="methodNm">方法或函数名称</param>
        /// <param name="ex">异常对象</param>
        public static void PutErrLog(string classNm, string methodNm, Exception ex)
        {
            PutErrLog(classNm, methodNm, ex.Message, "");
        }
        public static void PutErrLog(string classNm, string methodNm, string ex)
        {
            PutErrLog(classNm, methodNm, ex, "");
        }
        /// <summary>
        /// 错误日志输出
        /// </summary>
        /// <param name="classNm">类或模块名称</param>
        /// <param name="methodNm">方法或函数名称</param>
        /// <param name="ex">异常对象</param>
        /// <param name="addMessage">添加信息</param>
        public static void PutErrLog(string classNm, string methodNm, string ex, string addMessage)
        {
            string setMessage = "";
            string filePath = "";

            try
            {
                // 编辑输出信息
                setMessage = "";

                if (ex != null)
                {
                    setMessage = setMessage + " <Err>=" + ex;
                }
                if (!string.IsNullOrEmpty(addMessage))
                {
                    setMessage = setMessage + " <Comment>=" + addMessage;
                }
                setMessage = classNm + "." + methodNm + " | " + setMessage;
                string LogPath = Utils.GetMapPath(_LogPath);
                //如果不存在就创建file文件夹
                if (!Directory.Exists(LogPath))
                {
                    Directory.CreateDirectory(LogPath);
                }
                // 输出文件路径
                filePath = LogPath + GetFileName(LogType.Err);
                // 写入日志信息
                PutFreeLog(filePath, setMessage);
            }
            catch (Exception mEx)
            {
                Console.WriteLine("异常信息：" + mEx.Message);
            }

        }
        #endregion

        #region PutEvtLog: 事件日志输出
        /// <summary>
        /// 事件日志输出
        /// </summary>
        /// <param name="classNm">类或模块名称</param>
        /// <param name="methodNm">方法或函数名称</param>
        /// <param name="ex">异常对象</param>
        /// <param name="addMessage">添加信息</param>
        public static void PutEvtLog(string classNm, string methodNm, string message)
        {
            string setMessage = "";
            string filePath = "";

            try
            {
                // 编辑输出信息
                setMessage = "";

                setMessage = classNm + "." + methodNm + " | " + message;
                string LogPath = Utils.GetMapPath(_LogPath);
                //如果不存在就创建file文件夹
                if (!Directory.Exists(LogPath))
                {
                    Directory.CreateDirectory(LogPath);
                }

                // 输出文件路径
                filePath = Path.Combine(LogPath, GetFileName(LogType.Evt));

                // 写入日志信息
                PutFreeLog(filePath, setMessage);
            }
            catch (Exception mEx)
            {
                Console.WriteLine("异常信息：" + mEx.Message);
            }
        }
        #endregion

        #region GetFileName: 获取指定日志类型的文件名(有扩展名)
        /// <summary>
        /// 获取指定日志类型的文件名(有扩展名)
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <returns></returns>
        public static string GetFileName(LogType type)
        {
            DateTime targetDate = DateTime.Now;
            string fileName = GetFileName(type, targetDate);
            return fileName + ".log";
        }
        #endregion

        #region  GetFileName:获取指定日志类型的文件名(无扩展名)
        /// <summary>
        /// 获取指定日志类型的文件名(无扩展名)
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="targetDate">日志日期</param>
        /// <returns></returns>
        public static string GetFileName(LogType type, DateTime targetDate)
        {

            string fileName = "";
            switch (type)
            {
                case LogType.Err:
                    fileName = "Err_" + targetDate.ToString("yyyyMMdd");
                    break;
                case LogType.Evt:
                    fileName = "Evt_" + targetDate.ToString("yyyyMMdd");
                    break;
            }
            return fileName;
        }
        #endregion

        #endregion

        #region 私有方法

        #region PutFreeLog:在附加模式下输出可变长度日志
        /// <summary>
        /// 在附加模式下输出可变长度日志。
        /// </summary>
        /// <param name="filePath">日志文件的绝对路径</param>
        /// <param name="kbn">区分</param>
        /// <param name="message">信息</param>
        /// <returns></returns>
        private static void PutFreeLog(string filePath, string message)
        {
            StringBuilder sbData;
            try
            {
                // 编辑输出信息
                sbData = new System.Text.StringBuilder();
                sbData.Append(DateTime.Now.ToString("HH:mm:ss") + " ");
                sbData.Append(message);

                // 以附加写入模式打开并输出文件
                FileStream fs = new FileStream(filePath, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(sbData.ToString());
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("异常信息：" + ex.Message);
            }
        }
        #endregion


        #endregion
    }
}
