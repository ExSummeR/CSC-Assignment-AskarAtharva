using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Amazon.S3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NF.Samples.AWS_S3.MVC_Core.Models;

namespace NF.Samples.AWS_S3.MVC_Core.Controllers
{
    public class HomeController : Controller
    {
        public static LinkHolder LinkHolding=new LinkHolder();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult UploadFile()
        {
            return View();
        }
        #region Amazon
        // you must set your accessKey and secretKey
        // for getting your accesskey and secretKey go to your Aws amazon console
        string AWS_accessKey = "";
        string AWS_secretKey = "";
        string AWS_bucketName = "";
        string AWS_defaultFolder = "";

        [HttpPost]
        public async Task<IActionResult> UploadNewFileAsync(IFormFile myfile, string subFolder)
        {
            var result = await UploadFileToAWSAsync(myfile, subFolder);
            LinkHolding.link = result;
           
            return RedirectToAction(nameof(UploadFile));
        }

        protected async Task<string> UploadFileToAWSAsync(IFormFile myfile, string subFolder = "")
        {
            var result = "";
            try
            {
                var s3Client = new AmazonS3Client(AWS_accessKey, AWS_secretKey, Amazon.RegionEndpoint.APSoutheast1);
                var bucketName = AWS_bucketName;
                var keyName = AWS_defaultFolder;
                if (!string.IsNullOrEmpty(subFolder))
                    keyName = keyName + "/" + subFolder.Trim();
                keyName = keyName + "/" + myfile.FileName;

                var fs = myfile.OpenReadStream();
                var request = new Amazon.S3.Model.PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    InputStream = fs,
                    ContentType = myfile.ContentType,
                    CannedACL = S3CannedACL.PublicRead
                };
                await s3Client.PutObjectAsync(request);
                
                result = string.Format("http://{0}.s3.amazonaws.com/{1}", bucketName, keyName);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        public IActionResult BitlyLink()
        {
            ViewBag.message=LinkHolding.BitlyLink;
            return View(ViewBag);
        }
        public  ActionResult LinkGenerator() {
            
            BitlyAPI B = new BitlyAPI("");
                string result = B.Shorten(LinkHolding.link);
            LinkHolding.BitlyLink = result;
            return RedirectToAction(nameof(BitlyLink));
        }

        public class BitlyAPI
        {
            private string BitlyApi = @"https://api-ssl.bitly.com/shorten?access_token={0}&longUrl={1}";
            public string ACCESS_TOKEN { get; set; }
            public BitlyAPI()
            {
            }

            /// <summary>
            /// Create new Bitly object with access token
            /// </summary>
            /// <param name="access_token"></param>
            public BitlyAPI(string access_token)
            {
                ACCESS_TOKEN = access_token;
            }

            /// <summary>
            /// Check Access Token using synchronous method
            /// </summary>
            /// <returns></returns>
            public bool CheckAccessToken()
            {
                if (string.IsNullOrEmpty(ACCESS_TOKEN))
                    return false;
                string temp = string.Format(BitlyApi, ACCESS_TOKEN, "google.com");
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage res = client.GetAsync(temp).Result;
                    return res.IsSuccessStatusCode;
                }
            }

            /// <summary>
            /// Check Access Token using asynchronous
            /// </summary>
            /// <returns></returns>
            public async Task<bool> CheckAccessTokenAsync()
            {
                return await Task.Run(() => CheckAccessToken());
            }

            /// <summary>
            /// Shortern long URL using synchronous
            /// </summary>
            /// <param name="long_url"></param>
            /// <returns></returns>
            public string Shorten(string long_url)
            {
                if (CheckAccessToken())
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string temp = string.Format(BitlyApi, ACCESS_TOKEN, WebUtility.UrlEncode(long_url));
                        var res = client.GetAsync(temp).Result;
                        if (res.IsSuccessStatusCode)
                        {
                            var message = res.Content.ReadAsStringAsync().Result;
                            dynamic obj = JsonConvert.DeserializeObject(message);
                            return obj.results[long_url].shortUrl;
                        }
                        else
                        {
                            return "Can not short URL";
                        }
                    }
                }
                else
                {
                    return "Can not short URL";
                }
            }

            /// <summary>
            /// Shortern long URL using asynchronous
            /// </summary>
            /// <param name="long_url"></param>
            /// <returns></returns>
            public async Task<string> ShortenAsync(string long_url)
            {
                return await Task.Run(() => Shorten(long_url));
            }
        }
    }

    #endregion
}

