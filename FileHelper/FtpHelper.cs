using System;
using System.Net;
using System.IO;

namespace FileHelper
{
    public class FtpHelper
    {
        private string _ftpServerUrl;
        private string _ftpRemotePath;
        private string _ftpUserName;
        private string _ftpPassword;
        private string _ftpURI;

        /// <summary>
        /// 连接FTP
        /// </summary>
        /// <param name="ftpServerUrl">FTP连接地址</param>
        /// <param name="ftpUserName">用户名</param>
        /// <param name="ftpPassword">密码</param>
        /// <param name="ftpRemotePath">指定FTP连接成功后的当前目录, 如果不指定即默认为根目录</param>
        public FtpHelper(string ftpServerUrl, string ftpUserName, string ftpPassword, string ftpRemotePath = null)
        {
            _ftpServerUrl = ftpServerUrl;
            _ftpRemotePath = ftpRemotePath;
            _ftpUserName = ftpUserName;
            _ftpPassword = ftpPassword;
            _ftpURI = ftpServerUrl.TrimEnd('/') + "/" + ftpRemotePath + "/";
        }
        
        public bool Upload(string fileName, Stream fileStream)
        {
            CreateFtpDirectory();
            byte[] filebytes = FileBaseClass.StreamToBytes(fileStream);
            bool result = false;
            FtpWebRequest reqFtp;
            reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpURI + fileName));
            reqFtp.Credentials = new NetworkCredential(_ftpUserName, _ftpPassword);
            reqFtp.KeepAlive = false;
            reqFtp.Method = WebRequestMethods.Ftp.UploadFile;
            reqFtp.UseBinary = true;
            try
            {
                Stream ftpStream = reqFtp.GetRequestStream();
                ftpStream.Write(filebytes, 0, filebytes.Length);
                result = true;
                ftpStream.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("ftp文件上传失败：" + ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        public Stream Download(string fileName)
        {
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpURI + fileName));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = false;
                reqFTP.Credentials = new NetworkCredential(_ftpUserName, _ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                MemoryStream memoryStream = new MemoryStream();
                const int bufferLength = 1024;
                int actual;
                byte[] buffer = new byte[bufferLength];
                while ((actual = ftpStream.Read(buffer, 0, bufferLength)) > 0)
                {
                    memoryStream.Write(buffer, 0, actual);
                }
                memoryStream.Position = 0;
                ftpStream.Close();
                response.Close();
                return memoryStream;
            }
            catch (Exception ex)
            {
                throw new Exception("ftp文件下载失败：" + ex.Message);
            }
        }
        
        /// <summary>
        /// 检查FTP服务器上，指定的路径下文件是否存在
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public bool CheckIfFileExists(string fileName)
        {
            var request = (FtpWebRequest)WebRequest.Create(_ftpURI + fileName);
            request.Credentials = new NetworkCredential(_ftpUserName, _ftpPassword);
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    return false;
            }
            return false;
        }
        
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        public void Delete(string fileName)
        {
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(_ftpURI + fileName);
            req.Credentials = new NetworkCredential(_ftpUserName, _ftpPassword);
            req.KeepAlive = false;
            req.Method = WebRequestMethods.Ftp.DeleteFile;
            req.UsePassive = false;
            try
            {
                FtpWebResponse response = (FtpWebResponse)req.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                req.Abort();
                throw new Exception("ftp文件删除失败：" + fileName + ex.Message);
            }
            req.Abort();
        }


        /// <summary>
        /// 判断文件的目录是否存,不存则创建
        /// </summary>
        /// <param name="destFilePath">本地文件目录</param>
        private void CreateFtpDirectory()
        {
            string fullDir = _ftpURI.IndexOf(':') > 0 ? _ftpURI.Substring(_ftpURI.IndexOf(':') + 1) : _ftpURI;
            fullDir = fullDir.Replace('\\', '/');
            string[] dirs = fullDir.Split(new char[] { '/'},StringSplitOptions.RemoveEmptyEntries);//解析出路径上所有的文件名
            string curDir = "ftp://";
            for (int i = 0; i < dirs.Length; i++)//循环查询每一个文件夹
            {
                string dir = dirs[i];
                curDir += dir + "/";
                if (CheckIfDirectoryExists(curDir) == false)
                {
                    MakeDirectory(curDir);
                }
            }
            _ftpURI = curDir;
        }

        /// <summary>
        /// 检查FTP服务器上，指定的路径是否存在
        /// </summary>
        /// <param name="ftpPathDir">全路径</param>
        /// <returns></returns>
        private bool CheckIfDirectoryExists(string ftpPathDir)
        {
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(ftpPathDir);
            req.Credentials = new NetworkCredential(_ftpUserName, _ftpPassword);
            req.Method = WebRequestMethods.Ftp.ListDirectory;
            try
            {
                FtpWebResponse response = (FtpWebResponse)req.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                FileBaseClass.log.Error("FTP目录不存在！" + ftpPathDir + ex.Message);
                req.Abort();
                return false;
            }
            req.Abort();
            return true;
        }

        /// <summary>
        /// 在FTP服务器上创建指定的目录
        /// </summary>
        /// <param name="ftpPathDir">全路径</param>
        /// <returns></returns>
        private bool MakeDirectory(string ftpPathDir)
        {
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(ftpPathDir);
            req.Credentials = new NetworkCredential(_ftpUserName, _ftpPassword);
            req.Method = WebRequestMethods.Ftp.MakeDirectory;
            req.KeepAlive = false;
            req.UseBinary = true;
            req.UsePassive = false;
            try
            {
                FtpWebResponse response = (FtpWebResponse)req.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                FileBaseClass.log.Error("创建FTP目录：" + ftpPathDir + ex.Message);
                req.Abort();
                return false;
            }
            req.Abort();
            return true;
        }

        ///// <summary>
        ///// 改名
        ///// </summary>
        ///// <param name="currentFilename"></param>
        ///// <param name="newFilename"></param>
        //public void ReName(string currentFilename, string newFilename)
        //{
        //    FtpWebRequest reqFTP;
        //    try
        //    {
        //        reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpURI + currentFilename));
        //        reqFTP.Method = WebRequestMethods.Ftp.Rename;
        //        reqFTP.RenameTo = newFilename;
        //        reqFTP.UseBinary = true;
        //        reqFTP.UsePassive = false;
        //        reqFTP.Credentials = new NetworkCredential(_ftpUserName, _ftpPassword);
        //        FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
        //        Stream ftpStream = response.GetResponseStream();
        //        ftpStream.Close();
        //        response.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("FtpHelper ReName Error --> " + ex.Message);
        //    }
        //}
    }
}
