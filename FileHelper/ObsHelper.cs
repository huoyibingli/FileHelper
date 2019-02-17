using OBS;
using OBS.Model;
using System;
using System.IO;

namespace FileHelper
{
    public class ObsHelper
    {
        private ObsClient _obsClient = null;
        private string _endpoint;
        private string _accessKey;
        private string _secretKey;
        private string _bucketName;

        public ObsHelper(string endpoint, string accessKey, string secretKey, string bucketName)
        {
            _endpoint = endpoint;
            _accessKey = accessKey;
            _secretKey = secretKey;
            _bucketName = bucketName;
            CreateObsClient();
        }

        private void CreateObsClient()
        {
            try
            {
                //初始化配置参数
                ObsConfig config = new ObsConfig();
                config.Endpoint = _endpoint;
                config.AuthType = AuthTypeEnum.OBS;
                // 创建ObsClient实例
                _obsClient = new ObsClient(_accessKey, _secretKey, config);
                // 使用访问OBS

                HeadBucketRequest request = new HeadBucketRequest
                {
                    BucketName = _bucketName,
                };
                _obsClient.HeadBucket(request);
                //if (_obsClient.HeadBucket(request) == false)
                //{
                //    CreateBucketRequest requestCreate = new CreateBucketRequest();
                //    requestCreate.BucketName = _bucketName;
                //    _obsClient.CreateBucket(requestCreate);
                //}
            }
            catch (ObsException ex)
            {
                throw new Exception("上传Obs错误：" + ex.ErrorCode + ":" + ex.Message);
            }
        }

        public bool Upload(string key, Stream fileStream)
        {
            try
            {
                PutObjectRequest request = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    ObjectKey = key,
                    InputStream = fileStream//new MemoryStream(Encoding.UTF8.GetBytes("Hello OBS"))
                };
                _obsClient.PutObject(request);
                return true;
            }
            catch (ObsException ex)
            {
                throw new Exception("上传Obs错误：" + ex.ErrorCode + ":" + ex.Message);
            }
        }

        public Stream Download(string key)
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest()
                {
                    BucketName = _bucketName,
                    ObjectKey = key,
                };
                using (GetObjectResponse response = _obsClient.GetObject(request))
                {
                    Stream requestStream = response.OutputStream;
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
            }
            catch (ObsException ex)
            {
                throw new Exception("上传Obs错误：" + ex.ErrorCode + ":" + ex.Message);
            }

        }

        public void Delete(string key)
        {
            try
            {
                DeleteObjectRequest request = new DeleteObjectRequest()
                {
                    BucketName = _bucketName,
                    ObjectKey = key,
                };
                DeleteObjectResponse response = _obsClient.DeleteObject(request);
            }
            catch (ObsException ex)
            {
                throw new Exception("上传Obs错误：" + ex.ErrorCode + ":" + ex.Message);
            }
        }

        //public bool CheckIfFileExist(string key)
        //{
        //    try
        //    {
        //        bool exist = _obsClient.DoesObjectExist(_bucketName, key);
        //        return exist;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.ToString());
        //    }
        //}
    }
}
