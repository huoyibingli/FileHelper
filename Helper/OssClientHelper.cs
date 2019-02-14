using Aliyun.OSS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eca.File
{
    public class OssClientHelper
    {
        private OssClient _ossClient = null;
        private string _endpoint;
        private string _accessKeyId;
        private string _accessKeySecret;
        private string _bucketName;

        public OssClientHelper(string endpoint, string accessKeyId, string accessKeySecret, string bucketName)
        {
            _endpoint = endpoint;
            _accessKeyId = accessKeyId;
            _accessKeySecret = accessKeySecret;
            _bucketName = bucketName;
            CreateOssClient();
        }

        private void CreateOssClient()
        {
            try
            {
                _ossClient = new OssClient(_endpoint, _accessKeyId, _accessKeySecret);
                //if (!_ossClient.DoesBucketExist(_bucketName))
                //{
                //    var bucket = _ossClient.CreateBucket(_bucketName);
                //}
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// mhd 2018-09-06 上传文件到OSS
        /// </summary>
        /// <param name="fileGuid">文件唯一标识，也是文件在OSS上的文件名，取文件时的key</param>
        /// <param name="fileStream">文件流</param>
        /// <returns></returns>
        public bool UploadOssCilent(string key, Stream fileStream, string contentType)
        {
            try
            {
                ObjectMetadata meta = new ObjectMetadata();
                meta.ContentType = contentType;
                var result = _ossClient.PutObject(_bucketName, key, fileStream, meta);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("上传Oss错误：" + ex.Message);
            }
        }

        public Stream DownloadOssClient(string key)
        {
            try
            {
                var obj = _ossClient.GetObject(_bucketName, key);
                Stream requestStream = obj.Content;
                MemoryStream memoryStream = new MemoryStream();
                const int bufferLength = 1024;
                int actual;
                byte[] buffer = new byte[bufferLength];
                while ((actual = requestStream.Read(buffer, 0, bufferLength)) > 0)
                {
                    memoryStream.Write(buffer, 0, actual);
                }
                memoryStream.Position = 0;
                return memoryStream;
            }
            catch (Exception e)
            {
                throw new Exception("Oss下载错误：" + e.Message);
            }
            
        }

        public void DeleteOssClient(string key)
        {
            try
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public bool CheckIfFileExist(string key)
        {
            try
            {
                bool exist = _ossClient.DoesObjectExist(_bucketName, key);
                return exist;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
