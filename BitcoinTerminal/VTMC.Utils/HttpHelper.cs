/*************************************************
*Author:YangBing
*Date:2016/12/7 15:30:09
*Des:
************************************************/
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using VTMC.Utils.Models;

namespace VTMC.Utils
{
    /// <summary>
    /// 文件下载和上传
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// Http访问接口
        /// </summary>
        /// <param name="reqMod"></param>
        /// <returns></returns>
        public static HttpResponseModel HttpRequest(HttpRequestModel reqMod)
        {
            LogHelper.WriteMethodLog(true);

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            HttpResponseModel refMod = new HttpResponseModel();

            try
            {
                request = (HttpWebRequest)WebRequest.Create(reqMod.Url);
                LogHelper.WriteInfoLog(string.Format("访问Url:{0}.", reqMod.Url));

                #region 追加证书文件
                //AddCertificateFile(request);
                #endregion

                if (reqMod.Headers != null)
                {
                    foreach (var kv in reqMod.Headers)
                    {
                        request.Headers.Add(kv.Key, kv.Value);
                        LogHelper.WriteInfoLog(string.Format("Key:{0}, Value:{1}\r\n", kv.Key, kv.Value));
                    }
                }

                request.Method = reqMod.Method.Method;
                request.KeepAlive = false;
                request.ContentType = reqMod.ContentType;

                //把参数等写入到Request中
                if (!string.IsNullOrEmpty(reqMod.BodyContent))
                {
                    using (StreamWriter dataStream = new StreamWriter(request.GetRequestStream()))
                    {
                        dataStream.Write(reqMod.BodyContent);
                        dataStream.Close();
                    }
                }

                //开始访问HttpUrl
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    refMod.StatusCode = response.StatusCode;

                    //获取Response返回的内容
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        //读取Response返回内容
                        refMod.HttpResponseInfo = reader.ReadToEnd();
                    }
                }

                return refMod;
            }
            catch (HttpRequestException webEx)
            {
                LogHelper.WriteErrorInfoLog(string.Format("{0}\r\n{1}", reqMod.Url, reqMod.BodyContent), webEx);

                return new HttpResponseModel
                {
                    StatusCode = HttpStatusCode.PreconditionFailed,
                    HttpResponseInfo = webEx.Message
                };
            }
            catch (WebException netEx)
            {
                LogHelper.WriteErrorInfoLog(string.Format("{0}\r\n{1}", reqMod.Url, reqMod.BodyContent), netEx);
                HttpWebResponse webResponse = (HttpWebResponse)netEx.Response;

                if (webResponse != null)
                {
                    using (var tokenStreamReader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        string ResponseInfo = tokenStreamReader.ReadToEnd();
                        LogHelper.WriteErrorInfoLog(ResponseInfo, netEx);
                        return new HttpResponseModel
                        {
                            StatusCode = webResponse.StatusCode,
                            HttpResponseInfo = ResponseInfo
                        };
                    }
                }
                else
                {
                    return new HttpResponseModel
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        HttpResponseInfo = netEx.Message
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog(string.Format("{0}\r\n{1}", reqMod.Url, reqMod.BodyContent), ex);
                return new HttpResponseModel
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    HttpResponseInfo = ex.Message
                };
            }
            finally
            {
                request?.Abort();
                LogHelper.WriteMethodLog(false);
            }
        }

        /// <summary>
        ///  文件上传
        /// </summary>
        /// <param name="reqMod"></param>
        /// <returns></returns>
        public static HttpResponseModel HttpUploadFiles(HttpUploadModel reqMod)
        {
            try
            {
                LogHelper.WriteMethodLog(true);
                LogHelper.WriteInfoLog(string.Format("访问Url:{0}.", reqMod.Url));

                using (WebRequestHandler webRequestHandler = new WebRequestHandler())
                {
                    #region 追加证书文件
                    AddCertificateFile(webRequestHandler);
                    #endregion

                    using (var client = new HttpClient(webRequestHandler))
                    {

                        if (reqMod.Headers != null)
                        {
                            reqMod.Headers.ToList().ForEach((k) =>
                            {
                                client.DefaultRequestHeaders.Add(k.Key, k.Value);
                                LogHelper.WriteInfoLog(string.Format("Key:{0}, Value:{1}\r\n", k.Key, k.Value));
                            });
                        }

                        client.Timeout = TimeSpan.FromMilliseconds(60000);
                        //定义请求的消息体
                        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
                        var content = new MultipartFormDataContent(boundary);
                        int i = 0;
                        reqMod.Files.ForEach(file =>
                        {
                            var fileStream = new FileStream(file.Path, FileMode.Open);
                            content.Add(new StreamContent(fileStream), string.Format("multipartFileInformations[{0}].multipartFile", i), file.FileName);
                            if (file.Attachments != null)
                            {
                                file.Attachments.ToList().ForEach((x) =>
                                {
                                    content.Add(new StringContent(x.Value), string.Format("multipartFileInformations[{0}]." + x.Key, i));
                                });
                            }
                            i++;
                        });

                        //在请求头中设置token
                        var message = new HttpRequestMessage(HttpMethod.Post, reqMod.Url);
                        message.Headers.Add("KeepActive", "False");
                        message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
                        message.Content = content;
                        var response = client.PostAsync(reqMod.Url, content).Result;
                        var retMessage = response.Content.ReadAsStringAsync().Result;

                        return new HttpResponseModel()
                        {
                            StatusCode = response.StatusCode,
                            HttpResponseInfo = retMessage
                        };
                    }
                }
            }
            catch (HttpRequestException webEx)
            {
                LogHelper.WriteErrorInfoLog(string.Format("{0}\r\n{1}", reqMod.Url, reqMod.BodyContent), webEx);

                return new HttpResponseModel
                {
                    StatusCode = HttpStatusCode.PreconditionFailed,
                    HttpResponseInfo = webEx.Message
                };
            }
            catch (WebException netEx)
            {
                LogHelper.WriteErrorInfoLog(string.Format("{0}\r\n{1}", reqMod.Url, reqMod.BodyContent), netEx);
                HttpWebResponse webResponse = (HttpWebResponse)netEx.Response;

                if (webResponse != null)
                {
                    using (var tokenStreamReader = new StreamReader(webResponse.GetResponseStream(), Encoding.GetEncoding("utf-8")))
                    {
                        string ResponseInfo = tokenStreamReader.ReadToEnd();
                        LogHelper.WriteErrorInfoLog("从服务器上传文件异常发生,服务器返回：" + ResponseInfo, netEx);
                        return new HttpResponseModel
                        {
                            StatusCode = webResponse.StatusCode,
                            HttpResponseInfo = ResponseInfo
                        };
                    }
                }
                else
                {
                    return new HttpResponseModel
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        HttpResponseInfo = netEx.Message
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog(string.Format("{0}\r\n{1}", reqMod.Url, reqMod.BodyContent), ex);
                return new HttpResponseModel
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    HttpResponseInfo = ex.Message
                };
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }

        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="reqMod"></param>
        /// <returns></returns>
        public static HttpResponseModel HttpDownloadFile(HttpDownloadModel reqMod)
        {
            LogHelper.WriteMethodLog(true);

            HttpResponseModel refMod = new HttpResponseModel();
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream myResponseStream = null;
            Stream m_localStream = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(reqMod.Url);
                LogHelper.WriteInfoLog(string.Format("访问Url:{0}.", reqMod.Url));

                if (reqMod.Headers != null)
                {
                    reqMod.Headers.ToList().ForEach((k) =>
                    {
                        request.Headers.Add(k.Key, k.Value);
                        LogHelper.WriteInfoLog(string.Format("Key:{0}, Value:{1}\r\n", k.Key, k.Value));
                    });
                }

                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";

                response = (HttpWebResponse)request.GetResponse();
                myResponseStream = response.GetResponseStream();

                //Fix - 修复当目录不存在时引发异常的Bug
                var folder = Path.GetDirectoryName(reqMod.Path);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                m_localStream = new FileStream(reqMod.Path, FileMode.Create);
                byte[] bArr = new byte[1024];
                int size = myResponseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    m_localStream.Write(bArr, 0, size);
                    size = myResponseStream.Read(bArr, 0, (int)bArr.Length);
                }

                refMod.StatusCode = response.StatusCode;
                refMod.HttpResponseInfo = string.Empty;

                return refMod;
            }
            catch (HttpRequestException webEx)
            {
                LogHelper.WriteErrorInfoLog(string.Format("{0}\r\n{1}", reqMod.Url, reqMod.BodyContent), webEx);

                return new HttpResponseModel
                {
                    StatusCode = HttpStatusCode.PreconditionFailed,
                    HttpResponseInfo = webEx.Message
                };
            }
            catch (WebException netEx)
            {
                LogHelper.WriteErrorInfoLog(string.Format("{0}\r\n{1}", reqMod.Url, reqMod.BodyContent), netEx);

                HttpWebResponse webResponse = (HttpWebResponse)netEx.Response;

                if (webResponse != null)
                {
                    using (var tokenStreamReader = new StreamReader(webResponse.GetResponseStream(), Encoding.GetEncoding("utf-8")))
                    {
                        string ResponseInfo = tokenStreamReader.ReadToEnd();
                        LogHelper.WriteErrorInfoLog("从服务器下载文件异常发生,服务器返回：" + ResponseInfo, netEx);
                        return new HttpResponseModel
                        {
                            StatusCode = webResponse.StatusCode,
                            HttpResponseInfo = ResponseInfo
                        };
                    }
                }
                else
                {
                    return new HttpResponseModel
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        HttpResponseInfo = netEx.Message
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog(string.Format("{0}\r\n{1}", reqMod.Url, reqMod.BodyContent), ex);
                return new HttpResponseModel
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    HttpResponseInfo = ex.Message
                };
            }
            finally
            {
                LogHelper.WriteMethodLog(false);

                response.Close();
                myResponseStream.Close();
                m_localStream.Close();
                request = null;
                response = null;
                myResponseStream = null;
                m_localStream = null;
            }
        }

        #region 追加https证书
        /// <summary>
        /// 追加https证书
        /// </summary>
        private static void AddCertificateFile(HttpWebRequest request)
        {
            ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
            //System.Security.Cryptography.X509Certificates.X509Certificate cer = new System.Security.Cryptography.X509Certificates.X509Certificate(ConstHelper.CERTIFICATE_DIR + AppSettings.HTTPSCertFileName);
            //request.ClientCertificates.Add(cer);
        }
        /// <summary>
        /// 追加https证书
        /// </summary>
        public static void AddCertificateFile(WebRequestHandler webRequestHandler)
        {
            ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
            System.Security.Cryptography.X509Certificates.X509Certificate cer = new System.Security.Cryptography.X509Certificates.X509Certificate(ConstHelper.CERTIFICATE_DIR + AppSettings.HTTPSCertFileName);
            webRequestHandler?.ClientCertificates.Add(cer);
        }


        /// <summary>
        /// 服务器询问，回答true
        /// </summary>
        private static bool RemoteCertificateValidationCallback(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}