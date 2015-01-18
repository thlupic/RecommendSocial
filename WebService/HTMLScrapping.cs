﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.IO.Compression;
using System.Threading;
using System.Web;
using System.Security.Cryptography.X509Certificates;

public class HTMLScrapping
{
    public class PostValue
    {
        public PostValue(String key, String value)
        {
            Key = key;
            Value = value;
        }


        public String Key { get; set; }

        public String Value { get; set; }
    }

    [Serializable]
    public class WebPage
    {
        public WebPage(String html)
        {
            Html = html;
        }

        public WebPage(String html, WebPage parent)
        {
            Html = html;
            Parent = parent;
        }

        public String Html { get; set; }
        public WebPage Parent { get; set; }
    }


    internal class AcceptAllCertificatePolicy : ICertificatePolicy
    {
        public AcceptAllCertificatePolicy()
        {
        }

        public bool CheckValidationResult(ServicePoint sPoint,
           X509Certificate cert, WebRequest wRequest, int certProb)
        {
            // Always accept
            return true;
        }
    }

    public class WebSession
    {
        public String BaseUrl { get; set; }
        public String LastUrl { get; set; }
        public String UserAgent { get; set; }

        public int PageReattempts { get; set; }
        public WebProxy Proxy { get; set; }
        public String CookieString { get; set; }
        public Dictionary<String, String> Cookies { get; set; }

        private static WebSession instance { get; set; }
        public static WebSession Instance { get { if (instance == null) instance = new WebSession(); return instance; } }

        public const String DefaultAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.0.8) Gecko/2009032609 Firefox/3.0.8";

        public WebSession()
            : this(DefaultAgent, null)
        {
        }


        public WebSession(String baseUrl)
            : this(DefaultAgent, null)
        {
            BaseUrl = baseUrl;
        }

        public WebSession(String userAgent, WebProxy proxy)
        {
            ServicePointManager.CertificatePolicy = new AcceptAllCertificatePolicy();
            CookieString = "";
            Cookies = new Dictionary<string, string>();

            if (userAgent == "")
                UserAgent = DefaultAgent;
            else
                UserAgent = userAgent;

            Proxy = proxy;
            LastUrl = "";
            PageReattempts = 4;
            ServicePointManager.Expect100Continue = false;
        }


        public WebPage RequestPage(string URL)
        {
            return RequestPage(new Uri(BaseUrl + URL));
        }

        public WebPage RequestPage(string URL, string Values, string Method)
        {
            return RequestPage(new Uri(BaseUrl + URL), Values, Method);
        }

        public WebPage RequestPage(string URL, string Values, string Method, string ContentType)
        {
            return RequestPage(new Uri(BaseUrl + URL), Values, Method, "application/x-www-form-urlencoded");
        }

        public WebPage RequestPage(Uri URL)
        {
            return RequestPage(URL, "", "GET");
        }


        public WebPage RequestPage(String URL, params PostValue[] postValues)
        {
            String totalString = "";

            if (postValues.Length > 0)
            {
                for (int count = 0; count < postValues.Length; count++)
                {
                    if (count > 0)
                        totalString += "&";

                    totalString += postValues[count].Key + "=" + HttpUtility.UrlEncode(postValues[count].Value);
                }
            }

            return RequestPage(URL, totalString);
        }


        public WebPage RequestPage(string URL, string Values)
        {
            return RequestPage(new Uri(BaseUrl + URL), Values);
        }


        public WebPage RequestPage(Uri URL, string Values)
        {
            return RequestPage(URL, Values, "POST");
        }


        public WebPage RequestPage(Uri URL, string Values, string Method)
        {
            return RequestPage(URL, Values, Method, "application/x-www-form-urlencoded");
        }


        public WebPage RequestPage(Uri url, string content, string method, string contentType)
        {
            string htmlResult;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse response = null;
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] contentData = encoding.GetBytes(content);

            request.Proxy = Proxy;
            request.Timeout = 60000;
            request.Method = method;
            request.AllowAutoRedirect = false;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Referer = LastUrl;
            request.KeepAlive = false;

            request.UserAgent = UserAgent;

            request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
            //request.Headers.Add("UA-CPU", "x86");
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("Accept-Encoding", "gzip,deflate");

            String cookieString = "";
            foreach (KeyValuePair<String, String> cookiePair in Cookies)
                cookieString += cookiePair.Key + "=" + cookiePair.Value + ";";

            if (cookieString.Length > 2)
            {
                String cookie = cookieString.Substring(0, cookieString.Length - 1);
                request.Headers.Add("Cookie", cookie);
            }

            if (method == "POST")
            {
                request.ContentLength = contentData.Length;
                request.ContentType = contentType;

                Stream contentWriter = request.GetRequestStream();
                contentWriter.Write(contentData, 0, contentData.Length);
                contentWriter.Close();
            }

            int attempts = 0;

            while (true)
            {
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                    if (response == null)
                        throw new WebException();

                    break;
                }
                catch (WebException)
                {
                    if (response != null)
                        response.Close();

                    if (attempts == PageReattempts)
                        throw;

                    // Wait three seconds before trying again
                    Thread.Sleep(3000);
                }

                attempts += 1;
            }

            // Tokenize cookies
            if (response.Headers["Set-Cookie"] != null)
            {
                String headers = response.Headers["Set-Cookie"].Replace("path=/,", ";").Replace("HttpOnly,", "");
                foreach (String cookie in headers.Split(';'))
                {
                    if (cookie.Contains("="))
                    {
                        String[] splitCookie = cookie.Split('=');
                        String cookieKey = splitCookie[0].Trim();
                        String cookieValue = splitCookie[1].Trim();

                        if (Cookies.ContainsKey(cookieKey))
                            Cookies[cookieKey] = cookieValue;
                        else
                            Cookies.Add(cookieKey, cookieValue);
                    }
                    else
                    {
                        if (Cookies.ContainsKey(cookie))
                            Cookies[cookie] = "";
                        else
                            Cookies.Add(cookie, "");
                    }
                }
            }

            htmlResult = ReadResponseStream(response);
            response.Close();

            if (response.Headers["Location"] != null)
            {
                response.Close();
                Thread.Sleep(1500);
                String newLocation = response.Headers["Location"];
                WebPage result = RequestPage(newLocation);
                return new WebPage(result.Html, new WebPage(htmlResult));
            }

            LastUrl = url.ToString();

            return new WebPage(htmlResult);
        }

        public string ReadResponseStream(HttpWebResponse response)
        {
            Stream responseStream = null;
            StreamReader reader = null;

            try
            {
                responseStream = response.GetResponseStream();
                responseStream.ReadTimeout = 5000;

                if (response.ContentEncoding.ToLower().Contains("gzip"))
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                    responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);

                reader = new StreamReader(responseStream);

                return reader.ReadToEnd();
            }
            finally
            {
                reader.Close();
                responseStream.Close();
            }
        }
    }
}