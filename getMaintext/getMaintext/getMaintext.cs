using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace getMaintext
{
    public class Maintext
    {
        public struct SiteNode
        {
            public string SiteUrl;
            public string StartFlag;
            public string EndFlag;
        };
        static List<SiteNode> SiteNodeList = new List<SiteNode>();

        //获取页面数据
        public static string getData(string url)
        {
            string str = "";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.AllowAutoRedirect = true;
            request.AllowWriteStreamBuffering = true;
            request.Timeout = 10 * 1000;
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string characterset = response.CharacterSet;
                    Encoding encode;
                    if (characterset != null)
                    {
                        if (characterset == "ISO-8859-1") characterset = "gb2312";
                        encode = Encoding.GetEncoding(characterset);
                    }
                    else
                    {
                        encode = Encoding.Default;
                    }

                    //写入内存流
                    Stream stream = response.GetResponseStream();
                    MemoryStream mstream = new MemoryStream();
                    byte[] bf = new byte[255];
                    int cnt = stream.Read(bf, 0, 255);
                    while (cnt > 0)
                    {
                        mstream.Write(bf, 0, cnt);
                        cnt = stream.Read(bf, 0, 255);
                    }
                    stream.Close();
                    mstream.Seek(0, SeekOrigin.Begin);

                    //写入字符串
                    StreamReader reader = new StreamReader(mstream, encode);
                    str = reader.ReadToEnd();

                    Regex reg = new Regex(@"<meta[\s\S]+?charset=[""]?(.*?)""[\s\S]*?>",
                         RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    MatchCollection mc = reg.Matches(str);
                    if (mc.Count > 0)
                    {
                        string tempCharSet = mc[0].Result("$1");
                        if (string.Compare(tempCharSet, characterset, true) != 0)
                        {
                            encode = Encoding.GetEncoding(tempCharSet);
                            str = "";
                            mstream.Seek(0, SeekOrigin.Begin);
                            reader = new StreamReader(mstream, encode);
                            str = reader.ReadToEnd();
                        }
                    }
                    reader.Close();
                    mstream.Close();
                }
            }
            catch (Exception ex)
            {
                str = "E:" + ex.Message.ToString();
                return str;
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
            return getMain(str, url);
        }

        public static string getMain(string input, string url)
        {
            string maintext = "";
            int startpos = -1;
            int endpos = -1;

            //判断来源
            foreach (SiteNode nodetmp in SiteNodeList)
            {
                if (url.IndexOf(nodetmp.SiteUrl) != -1)
                {
                    endpos = input.IndexOf(nodetmp.EndFlag);
                    if (endpos != -1)
                    {
                        startpos = input.IndexOf(nodetmp.StartFlag);
                        maintext = input.Substring(0, endpos);
                        startpos = maintext.LastIndexOf(nodetmp.StartFlag);
                    }
                    break;
                }
            }

            if (startpos != -1 && endpos != -1) maintext = input.Substring(startpos, endpos - startpos);
            return getP(maintext);
        }

        public static string getP(string input)
        {
            if (input == "") return input;

            string mainp = "";
             
            int startp = input.IndexOf(@"<p>");
            int endp = -1;
            while (startp != -1)
            {
                endp = input.IndexOf(@"</p>", startp);
                if (endp == -1) break;
                mainp += @"[p]" + input.Substring(startp + 3, endp - startp - 3);
                startp = input.IndexOf(@"<p>", endp);
            }
            
             ////正则表达式去除html标签
            //string reg1 = @"<(p|br)>";

            string reg2 = @"(\[([^=]*)(=[^\]]*)?\][\s\S]*?\[/\1\])|(?<lj>(?=[^\u4E00-\u9FA5\uFE30-\uFFA0,."");])<a\s+[^>]*>[^<]{2,}</a>(?=[^\u4E00-\u9FA5\uFE30-\uFFA0,."");]))|(?<Style><style[\s\S]+?/style>)|(?<select><select[\s\S]+?/select>)|(?<Script><script[\s\S]*?/script>)|(?<Explein><\!\-\-[\s\S]*?\-\->)|(?<li><li(\s+[^>]+)?>[\s\S]*?/li>)|(?<Html></?\s*[^> ]+(\s*[^=>]+?=['""]?[^""']+?['""]?)*?[^\[<]*>)|(?<Other>&[a-zA-Z]+;)|(?<Other2>\#[a-z0-9]{6})|(?<Space>\s+)|(\&\#\d+\;)";

                    
            //@"(\[([^=]*)(=[^\]]*)?\][\s\S]*?\[/\1\])|
            // (?<lj>(?=[^\u4E00-\u9FA5\uFE30-\uFFA0,."");])<a\s+[^>]*>[^<]{2,}</a>(?=[^\u4E00-\u9FA5\uFE30-\uFFA0,."");]))|
            // (?<Style><style[\s\S]+?/style>)|
            // (?<select><select[\s\S]+?/select>)|
            // (?<Script><script[\s\S]*?/script>)|
            // (?<Explein><\!\-\-[\s\S]*?\-\->)|
            // (?<li><li(\s+[^>]+)?>[\s\S]*?/li>)|
            // (?<Html></?\s*[^> ]+(\s*[^=>]+?=['""]?[^""']+?['""]?)*?[^\[<]*>)|
            // (?<Other>&[a-zA-Z]+;)|
            // (?<Other2>\#[a-z0-9]{6})|
            // (?<Space>\s+)|
            // (\&\#\d+\;)";

            //maintext = new Regex(reg1, RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(maintext, "[$1]");
            mainp = new Regex(reg2, RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(mainp, "");

            return mainp;
        }

        public static string getNodeList(string path)
        {
            SiteNodeList.Clear();
            string str = "";
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(path);
                XmlElement rootElem = doc.DocumentElement;
                XmlNodeList TemplateList = rootElem.GetElementsByTagName("NewsTemplate");
                if (TemplateList.Count == 0)
                {
                    str = "Could not find NewsTemplate";
                    return str;
                }
                SiteNode SiteNodetmp = new SiteNode();
                foreach (XmlNode TemplateNode in TemplateList)
                {
                    SiteNodetmp.SiteUrl = TemplateNode.SelectSingleNode("SiteUrl").InnerText;
                    SiteNodetmp.StartFlag = TemplateNode.SelectSingleNode("StartFlag").InnerText;
                    SiteNodetmp.EndFlag = TemplateNode.SelectSingleNode("EndFlag").InnerText;
                    SiteNodeList.Add(SiteNodetmp);
                }
            }
            catch (Exception e)
            {
                str = e.Message.ToString();
            }
            return str;
        }
    }
}
