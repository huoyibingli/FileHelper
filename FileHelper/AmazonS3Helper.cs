
//using Amazon.S3;
//using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileHelper
{
    public class AmazonS3Helper
    {
        private string _endpoint;
        private string _accessKeyId;
        private string _secretAccessKeyId;
        private string _bucketName;
        private string _uri;
        //private IAmazonS3 _client;
        //private AmazonS3Config _config = new AmazonS3Config();

        public AmazonS3Helper(string endpoint, string accessKeyId, string secretAccessKeyId, string bucketName)
        {
            _endpoint = endpoint;
            _accessKeyId = accessKeyId;
            _secretAccessKeyId = secretAccessKeyId;
            _bucketName = bucketName;
            _uri = endpoint.IndexOf('/') > 0 ? "http://" + string.Format("{0}.{1}", bucketName, endpoint.Substring(endpoint.IndexOf('/') + 2)) : "http://" + string.Format("{0}.{1}", bucketName, endpoint);
            //_uri = endpoint.IndexOf('/') > 0 ? "http://" + endpoint.Substring(endpoint.IndexOf('/') + 2) : "http://" + endpoint;
            //if (endpoint.IndexOf('/') > 0) _config.ServiceURL = endpoint.Substring(endpoint.IndexOf('/') + 2);
            //else _config.ServiceURL = _uri;
            //_config.ForcePathStyle = true;
            //_config.RegionEndpoint = Amazon.RegionEndpoint.CNNorthWest1;
            //_uri = endpoint;
        }

        //public IAmazonS3 Init()
        //{
        //    return new AmazonS3Client(_accessKeyId, _secretAccessKeyId, _config);
        //}

        //public async Task Upload(string fileName, string contentType, Stream stream)
        //{
        //    try
        //    {
        //        //using (var client = Init())
        //        //{
        //            //var putRequest = new PutObjectRequest
        //            //{
        //            //    BucketName = _bucketName,
        //            //    Key = fileName,
        //            //    InputStream = stream,
        //            //    ContentType = contentType
        //            //};

        //            //PutObjectResponse response = await client.PutObjectAsync(putRequest);
        //        //}
        //    }
        //    //catch (AmazonS3Exception e)
        //    //{
        //    //    Console.WriteLine(
        //    //            "Error encountered ***. Message:'{0}' when writing an object"
        //    //            , e.Message);
        //    //}
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(
        //            "Unknown encountered on server. Message:'{0}' when writing an object"
        //            , e.Message);
        //    }
        //}

        public bool Upload(string fileName, string contentType, Stream stream, ref string reMsg)
        {
            try
            {
                //using (var client = Init())
                //{
                    //PutObjectRequest putRequest = new PutObjectRequest
                    //{
                    //    BucketName = _bucketName,
                    //    Key = fileName,
                    //    InputStream = stream,
                    //    ContentType = contentType
                    //};

                    //PutObjectResponse response = client.PutObject(putRequest);
                    return true;
                //}
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_uri);
                //request.Method = WebRequestMethods.Http.Put;
                //request.ContentType = contentType;
                //request.KeepAlive = true;
                //DateTime date = DateTime.Now;
                //request.Date = DateTime.Now;
                //request.Host = _endpoint.IndexOf('/') > 0 ? _endpoint.Substring(_endpoint.IndexOf('/') + 2) : _endpoint;
                //request.Headers.Add("Authorization", GetAuth("PUT", contentType, date.ToString("r"), null, fileName));

                //Stream rs = request.GetRequestStream();
                //byte[] buffer = new byte[4096];
                //int bytesRead = 0;
                //while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                //{
                //    rs.Write(buffer, 0, bytesRead);
                //}
                //rs.Close();
                //stream.Close();

                //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //if (response == null) throw new Exception(string.Format("请求文件服务器错误！"));
                //using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                //{
                //    reMsg = sr.ReadToEnd();
                //    sr.Close();
                //}
                //response.Close();
                //return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public Stream Download(string key)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_uri);
                request.Method = "GET";
                request.KeepAlive = true;
                string date = DateTime.Now.ToString("r");
                request.Headers.Add("Date", date);
                request.Headers.Add("Authorization", GetAuth("PUT", string.Empty, date, null, key));
                
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response == null) throw new Exception(string.Format("请求文件服务器下载错误！"));
                Stream fileStream = response.GetResponseStream();
                MemoryStream memoryStream = new MemoryStream();
                const int bufferLength = 1024;
                int actual;
                byte[] buffer = new byte[bufferLength];
                while ((actual = fileStream.Read(buffer, 0, bufferLength)) > 0)
                {
                    memoryStream.Write(buffer, 0, actual);
                }
                memoryStream.Position = 0;
                fileStream.Close();
                response.Close();
                return memoryStream;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(string key)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_uri);
                request.Method = "DELETE";
                request.KeepAlive = true;
                string date = DateTime.Now.ToString("r");
                request.Headers.Add("Date", date);
                request.Headers.Add("Authorization", GetAuth("DELETE", string.Empty, date, null, key));

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response == null) throw new Exception(string.Format("请求文件服务器删除错误！"));
                response.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private HttpWebResponse TestWebClient(string method, Dictionary<string, string> param, string fileName, string contentType, Stream stream = null)
        {
            //switch (method)
            //{
            //    //case "PUT":
            //    case "PUT": // 上传
            //        break;
            //    case "GET": // 下载
            //        break;
            //    case "DELETE": // 删除
            //        break;
            //    default:
            //        return null;
            //}
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            //实例化
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(_uri);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = method;
            wr.KeepAlive = true;
            Stream rs = wr.GetRequestStream();

            #region "请求参数"
            // 请求参数
            string textTemplate = "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
            foreach (var item in param)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string text = String.Format(textTemplate, item.Key, item.Value);
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                rs.Write(bytes, 0, bytes.Length);
            }

            string date = DateTime.Now.ToString("r");
            wr.Headers.Add("Date", date);
            wr.Headers.Add("Authorization", GetAuth(method, contentType, date, null, fileName));
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    rs.CopyTo(ms);
            //    byte[] md5Byte = ms.ToArray();
            //}

            // 请求文件
            if (stream != null)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, "file", fileName, contentType);
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);
                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, bytesRead);
                }
                stream.Close();
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);
            string type = "Content-Disposition: form-data; name=\"submit\" \r\n Upload to OBS ";
            byte[] typebytes = System.Text.Encoding.UTF8.GetBytes(type);
            rs.Write(typebytes, 0, typebytes.Length);
            #endregion

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();
            


            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
            }
            catch (WebException ex)
            {
                if (wresp == null && ex.Response != null)
                {
                    wresp = ex.Response;
                }
            }
            finally
            {
                wr = null;
            }

            if (wresp != null)
            {
                return wresp as HttpWebResponse;
            }
            return null;

        }

        #region "Authorization"
        private string GetAuth(string httpVerb, string contentType, string date, byte[] md5Byte, string key)
        {
            string md5 = md5Byte == null ? string.Empty : MD5HashString(md5Byte);
            string canonicalizedAmzHeaders = "";
            string canonicalizedResource = "/" + _bucketName + "/" + key;

            string stringToSign = httpVerb + "\n" + md5 + "\n" + contentType + "\n"
                + date + "\n" + canonicalizedAmzHeaders + "\n" + canonicalizedResource;
            string signature = ToBase64hmac(stringToSign);
            string authorization = "AWS" + " " + _accessKeyId + ":" + signature;
            return authorization;
        }

        // Base64(HMAC-SHA1(accessKeySecret,clearText))
        private string ToBase64hmac(string clearText)
        {
            HMACSHA1 myHMACSHA1 = new HMACSHA1(Encoding.UTF8.GetBytes(_secretAccessKeyId));
            byte[] byteText = myHMACSHA1.ComputeHash(Encoding.UTF8.GetBytes(clearText));
            return System.Convert.ToBase64String(byteText);
        }

        private string MD5HashString(byte[] clearText)
        {
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            byte[] byteHash = MD5.ComputeHash(clearText);
            MD5.Clear();
            return Convert.ToBase64String(byteHash);
        }
        #endregion
    }
}
