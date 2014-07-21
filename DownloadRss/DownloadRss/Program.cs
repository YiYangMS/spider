using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

public class httpsever
{
    public void getcontent(string url, string path)
    {
        string str = "";
        try
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream resStream = response.GetResponseStream();
            Encoding encoding = Encoding.GetEncoding("utf-8");
            StreamReader sreader = new StreamReader(resStream, encoding);
            str = sreader.ReadToEnd();
            writetofile(str, path);
            resStream.Close();
            sreader.Close();

            //判断
            /*
            string contentType = response.Headers["Content-Type"];
            Encoding encoding = null;
            Regex contenttypeRegex = new Regex("charset\\s*=\\s*(\\S+)", RegexOptions.IgnoreCase);
            Match match = null;
            if (contentType != null)
            {
                match = contenttypeRegex.Match(contentType);
                if (match.Success)
                {
                    encoding = Encoding.GetEncoding(match.Groups[1].Value.Trim());
                }
            }
            if (contentType == null || !match.Success)
            {
                Encoding enc = Encoding.GetEncoding("ASCII");
                StreamReader asreader = new StreamReader(resStream, enc);
                string html = asreader.ReadToEnd();
                //Console.WriteLine("finish readtoend" + DateTime.Now);
                Regex reg_charset = new Regex(@"charset\b\s*=\s*(?<charset>[^""]*)");
                if (reg_charset.IsMatch(html))
                {
                    str = reg_charset.Match(html).Groups["charset"].Value;
                    encoding = Encoding.GetEncoding(str.Trim());
                }
                else
                {
                    str = Encoding.Default.BodyName;
                    encoding = Encoding.GetEncoding(str.Trim());
                }
                asreader.Close();
            }
            if (str == "gb2312") encoding = Encoding.GetEncoding("gb2312"); 
            StreamReader sreader = new StreamReader(resStream, encoding);
            str = sreader.ReadToEnd();
            writetofile(str);
            */

        }
        catch (Exception e)
        {
            str = e.Message.ToString();
            Console.WriteLine(str);
        }
    }

    public void writetofile(string result, string path)
    {
        //path = "d:\\1.txt";
        //Console.Write("path:");
        //path = Console.ReadLine();
        StreamWriter writer = new StreamWriter(path);
        writer.WriteLine(result);
        writer.Close();
    }
}


namespace DownloadRss
{

    class Program
    {
        static void Main(string[] args)
        {
            string url;
            url = args[0];
            //url = "http://rss.sina.com.cn/news/marquee/ddt.xml ";
            httpsever hs = new httpsever();
            hs.getcontent(url, args[1]);
            Console.WriteLine("finish!");
            Console.ReadLine();
        }
    }
}


//http://news.163.com/special/00011K6L/rss_newstop.xml 
//http://rss.sina.com.cn/news/marquee/ddt.xml 

