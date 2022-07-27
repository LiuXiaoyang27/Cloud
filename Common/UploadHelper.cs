using System;
using System.Collections;
using System.Web;
using System.IO;
using Model;

namespace Common
{

    /// <summary>
    /// 上传帮助类
    /// </summary>
    /// <remarks>20170602 liuyan</remarks>
    public class UploadHelper
    {
        /// <summary>
        /// 原始图片上传文件路径
        /// </summary>
        public static string uploadPath = "/data/upfile/";
        /// <summary>
        /// 缩略图图片上传文件路径
        /// </summary>
        public static string thumbnailPath = "/data/upfile/goods/thumbnail/";

        /// <summary>
        /// 模版文件上传地址
        /// </summary>
        public static string excelPath = "/data/upfile/excel/";

        /// <summary>
        /// 用户头像图片上传文件路径
        /// </summary>
        public static string avatarPath = "/data/avatar/";
        /// <summary>
        /// 文件上传方法
        /// </summary>
        /// <param name="postedFile">文件流</param>
        /// <param name="isThumbnail">是否生成缩略图</param>
        /// <returns>上传后文件信息</returns>
        public static FileData FileSaveAs(HttpPostedFile postedFile, bool isThumbnail)
        {
            try
            {
                string fileType = postedFile.ContentType; // 文件类型
                string fileExt = Utils.GetFileExt(postedFile.FileName); //文件扩展名，不含“.”
                int fileSize = postedFile.ContentLength; //获得文件大小，以字节为单位
                string fileName = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf(@"\") + 1); //取得原文件名
                string newFileName = Utils.GetRamCode() + "." + fileExt; //随机生成新的文件名
                string newThumbnailFileName = "thumb_" + newFileName; //随机生成缩略图文件名
                string fullUpLoadPath = Utils.GetMapPath(uploadPath); //上传目录的物理路径
                string newFilePath = uploadPath + newFileName; //上传后的路径
                string newThumbnailPath = uploadPath + "thumbnail/" + newThumbnailFileName; //上传后的缩略图路径
                //检查上传的物理路径是否存在，不存在则创建
                if (!Directory.Exists(fullUpLoadPath))
                {
                    Directory.CreateDirectory(fullUpLoadPath);
                }
                //保存文件
                postedFile.SaveAs(fullUpLoadPath + newFileName);
                //如果是图片，检查是否需要生成缩略图，是则生成
                if (isThumbnail)
                {
                    //检查上传的物理路径是否存在，不存在则创建
                    string thumnailPath = fullUpLoadPath + "thumbnail/";
                    if (!Directory.Exists(thumnailPath))
                    {
                        Directory.CreateDirectory(thumnailPath);
                    }

                    Thumbnail.MakeThumbnailImage(fullUpLoadPath + newFileName, thumnailPath +  newThumbnailFileName);
                }
                else
                {
                    newThumbnailPath = newFilePath; //不生成缩略图则返回原图
                }
                // 将信息保存到图片类中
                FileData data = new FileData();
                data.name = newFileName;
                //data.type = fileType;
                data.size = fileSize;
                data.url = newFilePath;
                data.invId = 0;
                data.deleteType = "";
                data.deleteUrl = "";
                data.thumbnailUrl = newThumbnailPath; 

                return data;
            }
            catch(Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// 文件上传方法
        /// </summary>
        /// <param name="postedFile">文件流</param>
        /// <returns>上传后文件信息</returns>
        public static string FileSaveAs(HttpPostedFile postedFile)
        {
            try
            {
                // 文件扩展名，不含“.”
                string fileExt = Utils.GetFileExt(postedFile.FileName);
                // 随机生成新的文件名
                string newFileName = Utils.GetRamCode() + "." + fileExt;
                // 上传目录的物理路径
                string fullUpLoadPath = Utils.GetMapPath(excelPath);
                // 上传后的路径
                string newFilePath = excelPath + newFileName; 

                // 检查上传的物理路径是否存在，不存在则创建
                if (!Directory.Exists(fullUpLoadPath))
                {
                    Directory.CreateDirectory(fullUpLoadPath);
                }
                string fullPath = fullUpLoadPath + newFileName;
                // 保存文件
                postedFile.SaveAs(fullPath);

                // 上传后的路径
                return fullPath;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 文件上传方法
        /// </summary>
        /// <param name="postedFile">文件流</param>
        /// <param name="isThumbnail">是否生成缩略图</param>
        /// <returns>上传后文件信息</returns>
        public static bool AvatarSaveAs(HttpPostedFile postedFile, ref string filePath)
        {
            try
            {
                //取得原文件名
                string fileName = Utils.GetRamCode() + ".jpg";

                string fullUploadPath = Utils.GetMapPath(avatarPath); //上传目录的物理路径           

                //检查上传的物理路径是否存在，不存在则创建
                if (!Directory.Exists(fullUploadPath))
                {
                    Directory.CreateDirectory(fullUploadPath);
                }
                filePath = avatarPath + "" + fileName;
                //保存文件
                postedFile.SaveAs(fullUploadPath + fileName);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 是否为图片文件
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        private bool IsImage(string _fileExt)
        {
            ArrayList al = new ArrayList();
            al.Add("bmp");
            al.Add("jpeg");
            al.Add("jpg");
            al.Add("gif");
            al.Add("png");
            if (al.Contains(_fileExt.ToLower()))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 文件上传方法
        /// </summary>
        /// <param name="postedFile">文件流</param>
        /// <returns>上传后文件信息</returns>
        public static PetitionRules SaveFileAs(HttpPostedFile postedFile, string uploadPath )
        {
            try
            {
                int fileSize = postedFile.ContentLength; //获得文件大小，以字节为单位
                string fileName = postedFile.FileName; //随机生成新的文件名
                string fullUpLoadPath = Utils.GetMapPath(uploadPath); //上传目录的物理路径
                string newFilePath = uploadPath + fileName; //上传后的路径
                //检查上传的物理路径是否存在，不存在则创建
                if (!Directory.Exists(fullUpLoadPath))
                {
                    Directory.CreateDirectory(fullUpLoadPath);
                }
                //保存文件
                postedFile.SaveAs(fullUpLoadPath + fileName);
                // 将信息保存到图片类中
                PetitionRules data = new PetitionRules();
                data.fileName = fileName;
                data.fileSize = fileSize;
                data.filePath = newFilePath;
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 文件上传方法
        /// </summary>
        /// <param name="postedFile">文件流</param>
        /// <returns>上传后文件信息</returns>
        public static OldPetition SaveFile(HttpPostedFile postedFile, string uploadPath)
        {
            try
            {
                int fileSize = postedFile.ContentLength; //获得文件大小，以字节为单位
                string fileName = postedFile.FileName; //随机生成新的文件名
                string fullUpLoadPath = Utils.GetMapPath(uploadPath); //上传目录的物理路径
                string newFilePath = uploadPath + fileName; //上传后的路径
                //检查上传的物理路径是否存在，不存在则创建
                if (!Directory.Exists(fullUpLoadPath))
                {
                    Directory.CreateDirectory(fullUpLoadPath);
                }
                //保存文件
                postedFile.SaveAs(fullUpLoadPath + fileName);
                // 将信息保存到图片类中
                OldPetition data = new OldPetition();
                data.fileName = fileName;
                data.fileSize = fileSize;
                data.filePath = newFilePath;
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 文件上传方法
        /// </summary>
        /// <param name="postedFile">文件流</param>
        /// <returns>上传后文件信息</returns>
        public static PetitionFiles SaveFileDoc(HttpPostedFile postedFile,string uploadPath)
        {
            try
            {
                int fileSize = postedFile.ContentLength; //获得文件大小，以字节为单位
                string fileName = postedFile.FileName; //随机生成新的文件名
                string fullUpLoadPath = Utils.GetMapPath(uploadPath); //上传目录的物理路径
                string newFilePath = uploadPath + fileName; //上传后的路径
                //检查上传的物理路径是否存在，不存在则创建
                if (!Directory.Exists(fullUpLoadPath))
                {
                    Directory.CreateDirectory(fullUpLoadPath);
                }
                //保存文件
                postedFile.SaveAs(fullUpLoadPath + fileName);
                // 将信息保存到图片类中
                PetitionFiles data = new PetitionFiles();
                data.name = fileName;
                //data.type = fileType;
                data.size = fileSize;
                data.url = newFilePath;
                data.petitionId = "";
                data.deleteType = "";
                data.deleteUrl = "";
                data.thumbnailUrl = "";

                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }


    }
}
