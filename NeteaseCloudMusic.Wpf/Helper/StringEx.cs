using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;

namespace NeteaseCloudMusic.Wpf.Helper
{
    public static class StringEx
    {
        /// <summary>
        /// 从下载链接获取文件名
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetRealName(this string url)
        {
            try
            {
                HttpWebRequest req = HttpWebRequest.CreateHttp(url);
                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                string header_contentDisposition = resp.Headers["content-disposition"];

                if (!string.IsNullOrEmpty(header_contentDisposition))
                {
                    return new ContentDisposition(header_contentDisposition).FileName;

                }
                else
                {
                    return Path.GetFileName(resp.ResponseUri.AbsolutePath);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 剔除字符串中不合法的文件名
        /// </summary>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        public static string RemoveInvalidFileNameChars(this string strFileName)
        {
            StringBuilder rBuilder = new StringBuilder(strFileName);
            foreach (char rInvalidChar in Path.GetInvalidFileNameChars())
                rBuilder.Replace(rInvalidChar.ToString(), string.Empty);
            return rBuilder.ToString();
        }

        /// <summary>
        /// 剔除字符串中不合法的文件路径字符
        /// </summary>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        public static string RemoveInvalidPathChars(this string strPath)
        {
            StringBuilder rBuilder = new StringBuilder(strPath);
            foreach (char rInvalidChar in Path.GetInvalidPathChars())
                rBuilder.Replace(rInvalidChar.ToString(), string.Empty);
            return rBuilder.ToString();
        }
    }
}
