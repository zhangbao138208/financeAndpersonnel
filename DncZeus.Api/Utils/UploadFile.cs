using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.Utils
{
    /// <summary>
    /// 单文件上传类 (暂时不支持多文件上传)
    /// </summary>
    public class UploadFile
    {
        /// <summary>
        /// 上传文件信息 (动态数组)
        /// </summary>
        public Dictionary<string, dynamic> FileInfo = new Dictionary<string, dynamic>();

        /// <summary>
        /// 最大文件大小
        /// </summary>
        public int FileSize = 10240;

        /// <summary>
        /// 允许上传的文件类型, 逗号分割,必须全部小写!
        /// 
        /// 格式: ".gif,.exe" 或更多
        /// </summary>
        public string FileType = ".jpg,.gif,.png,.bmp,.ico,.jpeg,.flV,.f4v,.webm,.vob,.dat,.mkv,.swf,.lavt,.cpk,.dirac,.ram,.at,.fli,.flc,.mod,.div,.dv,.divx,.mpg,.mpeg,.mpe,.ts,.wmv,.avi,.asf,.rm,.rmvb,.m4v,.mov,.3gp,.3g2,.mp4,.xls,.xlsx,.zip,.rar";

        /// <summary>
        /// 上传错误
        /// </summary>
        public bool Error;

        /// <summary>
        /// 消息
        /// </summary>
        public string Message;

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="formFile">表单文件域名称</param>
        /// <param name="folder">自定义文件夹</param>
        /// <param name="appointFileName">指定文件名</param>
        public void Save(IFormFile formFile, string folder = "", string appointFileName = "")
        {
            // 验证格式
            this.CheckingType(formFile.FileName, appointFileName);
            if (Error)
            {
                return;
            }
            // 获取存储目录
            var path = this.GetPath(folder) + "/";
            var dir = path + this.FileInfo["Name"];

            // 注册文件信息
            this.FileInfo.Add("path", path + this.FileInfo["Name"]);
            this.FileInfo.Add("filepath", $"/{ CeyhConfiguration.TheUploadFileSettings.UploadFolder }/{ this.FileInfo["dir"]}{this.FileInfo["Name"]}");

            // 保存文件
            using (FileStream fs = File.Create(dir))
            {
                formFile.CopyTo(fs);
                fs.Flush();
            }
        }



        #region base64转图片
        /// <summary>
        /// 图片上传 Base64解码
        /// </summary>
        /// <param name="dataURL">Base64数据</param>
        /// <param name="imgName">指定文件名</param>
        /// <returns>返回一个相对路径</returns>
        public void decodeBase64ToImage(string dataURL, string folder = "", string fileName = "")
        {



            String base64 = dataURL.Substring(dataURL.IndexOf(",") + 1);      //将‘，’以前的多余字符串删除

            try//会有异常抛出，try，catch一下
            {

                byte[] arr = Convert.FromBase64String(base64);//将纯净资源Base64转换成等效的8位无符号整形数组

                System.IO.MemoryStream ms = new System.IO.MemoryStream(arr);//转换成无法调整大小的MemoryStream对象
                Save(ms, folder, fileName);
                // bitmap = new System.Drawing.Bitmap(ms);//将MemoryStream对象转换成Bitmap对象

                //filename = imgName + ".jpg";//所要保存的相对路径及名字
                //string url = HttpRuntime.AppDomainAppPath.ToString();
                //string tmpRootDir = System.Web.HttpContext.Current.Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath.ToString()); //获取程序根目录 
                //string imagesurl2 = tmpRootDir + filename; //转换成绝对路径 
                //bitmap.Save(imagesurl2, System.Drawing.Imaging.ImageFormat.Jpeg);//保存到服务器路径
                //bitmap.Save(filePath + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                //bitmap.Save(filePath + ".gif", System.Drawing.Imaging.ImageFormat.Gif);
                //bitmap.Save(filePath + ".png", System.Drawing.Imaging.ImageFormat.Png);
                ms.Close();//关闭当前流，并释放所有与之关联的资源

            }
            catch (Exception e)
            {
                string massage = e.Message;
            }
            // return filename;//返回相对路径
        }
        #endregion

        public void Save(MemoryStream formFile, string folder = "user", string fileName = "")
        {
            // 获取存储目录
            var path = this.GetPath(folder) + "/";
            var dir = path + fileName + ".jpg";
            // 注册文件信息
            this.FileInfo.Add("path", path + fileName);
            this.FileInfo.Add("filepath", $"/{ CeyhConfiguration.TheUploadFileSettings.UploadFolder }/{dir}");

            var bitmap = new System.Drawing.Bitmap(formFile);//将MemoryStream对象转换成Bitmap对象

            bitmap.Save(dir, System.Drawing.Imaging.ImageFormat.Jpeg);


            // 保存文件
            //using (FileStream fs = File.Create(dir))
            //{
            //    fs.Write(formFile.GetBuffer(), 0, Convert.ToInt32(formFile.Length));
            //    fs.Flush();
            //}
        }

        /// <summary>
        /// 获取目录
        /// </summary>
        /// <param name="folder">自定义文件夹</param>
        /// <returns></returns>
        private string GetPath(string folder = "")
        {
            // 目录格式
            DirectoryInfo dir;
            string rootDir = Directory.GetCurrentDirectory();
            if (string.IsNullOrEmpty(folder))
            {
                string date = DateTime.Now.ToString("yyyy-MM");
                dir = new DirectoryInfo($@"{rootDir}/{CeyhConfiguration.TheUploadFileSettings.UploadFolder}/{date}");
                // 注册文件信息
                this.FileInfo.Add("dir", date + "/");
            }
            else
            {
                dir = new DirectoryInfo($@"{rootDir}/{CeyhConfiguration.TheUploadFileSettings.UploadFolder}/{folder}");
                // 注册文件信息
                this.FileInfo.Add("dir", folder + "/");
            }

            // 创建目录
            if (!dir.Exists)
            {
                Directory.CreateDirectory(dir.FullName);
            }
            return dir.FullName;
        }

        /// <summary>
        /// 验证文件类型
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="appointFileName">指定文件名</param>
        private void CheckingType(string fileName, string appointFileName = "")
        {
            // 获取允许允许上传类型列表
            string[] typeList = this.FileType.Split(',');

            // 获取上传文件类型(小写)
            string type = Path.GetExtension(fileName).ToLowerInvariant();
            string name = Path.GetFileNameWithoutExtension(fileName);
            string nameHash = name.GetHashCode().ToString();

            // 注册文件信息
            if (!string.IsNullOrEmpty(appointFileName))
            {
                type = ".jpg";
                var saveFileName = $"{appointFileName}{type}";
                this.FileInfo.Add("Name", saveFileName);
            }
            else
            {
                this.FileInfo.Add("Name", CollectionHelper.MD5Hash16(nameHash) + type);
            }
            this.FileInfo.Add("type", type);

            // 验证类型
            if (typeList.Contains(type) == false)
                this.TryError("文件类型非法!");
        }

        /// <summary>
        /// 抛出错误
        /// </summary>
        /// <param name="msg"></param>
        public void TryError(string msg)
        {
            this.Error = true;
            this.Message = msg;
        }
    }
}
