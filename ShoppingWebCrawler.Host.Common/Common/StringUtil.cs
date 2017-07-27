using System;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Security.Cryptography;
using System.Globalization;



namespace ShoppingWebCrawler.Host.Common.Common
{

    /// <summary>
    /// 字符串助手类
    /// </summary>
    public static class StringUtil
    {


        /// <summary>
        /// 得到对象的 DateTime 类型的值，默认值为DateTime.MinValue
        /// </summary>
        /// <param name="Value">要转换的值</param>
        /// <param name="defaultValue">如果转换失败，返回默认值为DateTime.MinValue</param>
        /// <returns>如果对象的值可正确返回， 返回对象转换的值 ，否则， 返回的默认值为DateTime.MinValue</returns>
        public static DateTime GetDateTime(this object Value, DateTime defaultValue)
        {
            if (Value == null) return defaultValue;

            if (Value is DBNull) return defaultValue;

            string strValue = Value as string;
            if (strValue == null && (Value is IConvertible))
            {
                return (Value as IConvertible).ToDateTime(CultureInfo.CurrentCulture);
            }
            if (strValue != null)
            {
                strValue = strValue
                    .Replace("年", "-")
                    .Replace("月", "-")
                    .Replace("日", "-")
                    .Replace("点", ":")
                    .Replace("时", ":")
                    .Replace("分", ":")
                    .Replace("秒", ":")
                      ;
            }
            DateTime dt = defaultValue;
            if (DateTime.TryParse(Value.ToString(), out dt))
            {
                return dt;
            }

            return defaultValue;
        }

        /// <summary>
        /// 将String转换为Dictionary类型，过滤掉为空的值，首先 6 分割，再 7 分割
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Dictionary<string, string> StringToDictionary(this string input)
        {
            Dictionary<string, string> queryDictionary = new Dictionary<string, string>();
            string[] s = input.Split('^');
            for (int i = 0; i < s.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(s[i]) && !s[i].Contains("undefined") && !s[i].Contains("请选择"))
                {
                    var ss = s[i].Split('&');
                    if ((!string.IsNullOrEmpty(ss[0])) && (!string.IsNullOrEmpty(ss[1])))
                    {
                        queryDictionary.Add(ss[0], ss[1]);
                    }
                }

            }
            return queryDictionary;
        }
        /// <summary>
        /// 获取一个字符串的MD5值
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetStringMD5(this string input)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bs = Encoding.Default.GetBytes(input);
            byte[] output = md5.ComputeHash(bs);

            return BitConverter.ToString(output).Replace("-", "");//25-F9-E7-94-32-3B-45-38-85-F5-18-1F-1B-62-4D-0B 去掉-符号
        }

        public static string Replace(string text, string oldValue, string newValue)
        {
            return Regex.Replace(text, oldValue, newValue, RegexOptions.IgnoreCase);
        }



        /// <summary>
        /// 生成随机码
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string BuiderRandomString(int length)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder buider = new StringBuilder(length);

            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                buider.Append(chars[(int)rnd.Next(0, chars.Length)]);
            }
            return buider.ToString();
        }


        /// <summary>
        /// string转int
        /// </summary>
        /// <param name="defaultValue">转换失败,返回此值</param>
        public static int GetInt(string input, int defaultValue)
        {
            int result;
            if (!int.TryParse(input, out result))
                result = defaultValue;
            return result;
        }

        /// <summary>
        /// 将一个字符串转为SQL字符串的内容。
        /// 注意返回结果不包含SQL字符串的开始和结束部分的单引号。
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToSqlString(string text)
        {
            return text.Replace("'", "''");
        }


        /// <summary>
        /// 将字符串列表按固定分隔符合并
        /// </summary>
        /// <param name="array">所要合并的字符串列表</param>
        /// <param name="separator">字符串的分隔符</param>
        /// <returns>合并结果</returns>
        public static string Join(IEnumerable array, string separator)
        {
            if (array == null)
                return string.Empty;

            StringBuilder result = new StringBuilder();

            foreach (object value in array)
            {
                result.Append(value);
                result.Append(separator);
            }

            if (result.Length > 0)
                result.Remove(result.Length - separator.Length, separator.Length);

            return result.ToString();
        }


        /// <summary>
        /// 将字符串按,分割，并返回int类型的数组
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string[] Split(string input)
        {
            if (string.IsNullOrEmpty(input))
                return new string[0];
            return input.Split(',');
        }

        /// <summary>
        /// 将字符串按固定分隔符分割，并返回int类型的数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] Split(string input, char separator)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return input.Split(separator);
            }
            return new string[0];
        }






        private static Encoding s_EncodingCache = null;

        /// <summary>
        /// 尝试获取GB2312编码并缓存起来，如果运行环境不支持GB2312编码，将缓存系统默认编码
        /// </summary>
        private static Encoding EncodingCache
        {
            get
            {
                if (s_EncodingCache == null)
                {

                    try
                    {
                        s_EncodingCache = Encoding.GetEncoding(936);

                    }
                    catch { }

                    if (s_EncodingCache == null)
                        s_EncodingCache = Encoding.UTF8;

                }

                return s_EncodingCache;
            }
        }

        /// <summary>
        /// 获取字符串的字节长度，默认自动尝试用GB2312编码获取，
        /// 如果当前运行环境支持GB2312编码，英文字母将被按1字节计算，中文字符将被按2字节计算
        /// 如果尝试使用GB2312编码失败，将采用当前系统的默认编码，此时得到的字节长度根据具体运行环境默认编码而定
        /// </summary>
        /// <param name="text">字符串</param>
        /// <returns>字符串的字节长度</returns>
        public static int GetByteCount(string text)
        {
            return EncodingCache.GetByteCount(text);
        }

        /// <summary>
        /// 计算行号
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="endIndex">结束位置</param>
        /// <returns></returns>
        public static int LineCount(string text, int startIndex, int endIndex)
        {
            int num = 0;

            while (startIndex < endIndex)
            {
                if ((text[startIndex] == '\r') || ((text[startIndex] == '\n') && ((startIndex == 0) || (text[startIndex - 1] != '\r'))))
                {
                    num++;
                }

                startIndex++;
            }

            return num;
        }

        /// <summary>
        /// 忽略大小写的字符串比较
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2))
            {
                return true;
            }

            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
            {
                return false;
            }

            if (s2.Length != s1.Length)
            {
                return false;
            }

            return (0 == string.Compare(s1, 0, s2, 0, s2.Length, StringComparison.OrdinalIgnoreCase));
        }


        public static bool StartsWith(string text, char lookfor)
        {
            return (text.Length > 0 && text[0] == lookfor);
        }

        /// <summary>
        /// 快速判断字符串起始部分
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool StartsWith(string target, string lookfor)
        {
            if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(lookfor))
            {
                return false;
            }

            if (lookfor.Length > target.Length)
            {
                return false;
            }

            return (0 == string.Compare(target, 0, lookfor, 0, lookfor.Length, StringComparison.Ordinal));
        }

        /// <summary>
        /// 快速判断字符串起始部分
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool StartsWithIgnoreCase(string target, string lookfor)
        {
            if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(lookfor))
            {
                return false;
            }

            if (lookfor.Length > target.Length)
            {
                return false;
            }
            return (0 == string.Compare(target, 0, lookfor, 0, lookfor.Length, StringComparison.OrdinalIgnoreCase));
        }

        public static bool EndsWith(string text, char lookfor)
        {
            return (text.Length > 0 && text[text.Length - 1] == lookfor);
        }

        public static bool EndsWith(string target, string lookfor)
        {
            int indexA = target.Length - lookfor.Length;

            if (indexA < 0)
            {
                return false;
            }

            return (0 == string.Compare(target, indexA, lookfor, 0, lookfor.Length, StringComparison.Ordinal));
        }

        /// <summary>
        /// 快递判断字符串结束部分
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool EndsWithIgnoreCase(string target, string lookfor)
        {
            int indexA = target.Length - lookfor.Length;

            if (indexA < 0)
            {
                return false;
            }

            return (0 == string.Compare(target, indexA, lookfor, 0, lookfor.Length, StringComparison.OrdinalIgnoreCase));
        }

        public static bool Contains(string target, string lookfor)
        {
            if (target.Length < lookfor.Length)
                return false;

            return (0 <= target.IndexOf(lookfor));
        }

        /// <summary>
        /// 忽略大小写判断字符串是否包含
        /// </summary>
        /// <param name="target"></param>
        /// <param name="lookfor"></param>
        /// <returns></returns>
        public static bool ContainsIgnoreCase(string target, string lookfor)
        {
            if (target.Length < lookfor.Length)
                return false;

            return (0 <= target.IndexOf(lookfor, StringComparison.OrdinalIgnoreCase));
        }


        /// <summary>
        /// 截取指定长度字符串
        /// </summary>
        /// <param name="text"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CutString(string text, int length)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (length < 1)
                return text;

            byte[] buf = EncodingCache.GetBytes(text);

            if (buf.Length <= length)
            {
                return text;
            }

            int newLength = length;
            int[] numArray1 = new int[length];
            byte[] newBuf = null;
            int counter = 0;
            for (int i = 0; i < length; i++)
            {
                if (buf[i] > 0x7f)
                {
                    counter++;
                    if (counter == 3)
                    {
                        counter = 1;
                    }
                }
                else
                {
                    counter = 0;
                }
                numArray1[i] = counter;
            }

            if ((buf[length - 1] > 0x7f) && (numArray1[length - 1] == 1))
            {
                newLength = length + 1;
            }
            newBuf = new byte[newLength];
            Array.Copy(buf, newBuf, newLength);
            return EncodingCache.GetString(newBuf) + "...";

        }

        public static int FirstIndexOf(string source, int startIndex, int length, out string match, params string[] lookfors)
        {
            int index = -1;
            int itemIndex = -1;

            for (int i = 0; i < lookfors.Length; i++)
            {
                int temp = source.IndexOf(lookfors[i], startIndex, length);

                if (index < 0 || (temp >= 0 && temp < index))
                {
                    index = temp;
                    itemIndex = i;
                }
            }

            if (itemIndex >= 0)
                match = lookfors[itemIndex];
            else
                match = null;

            return index;
        }

        /// <summary>
        /// 友好大小
        /// </summary>
        public static string FriendlyCapacitySize(long value)
        {
            if (value < 1024 * 5 && value % 1024 != 0)
            {
                return value + " B";
            }
            else if (value < 1024 * 5 && value % 1024 == 0)
            {
                return (value / 1024) + " KB";
            }
            else if (value >= 1024 * 5 && value < 1024 * 1024)
            {
                return (value / 1024) + " KB";
            }
            else if (value < 1024 * 1024 * 5 && value % (1024 * 1024) != 0)
            {
                return (value / 1024) + " KB";
            }
            else if (value < 1024 * 1024 * 5 && value % (1024 * 1024) == 0)
            {
                return (value / (1024 * 1024)) + " MB";
            }
            else if (value >= 1024 * 1024 * 5 && value < 1024 * 1024 * 1024)
            {
                return (value / (1024 * 1024)) + " MB";
            }
            else
            {
                return (value / (1024 * 1024 * 1024)) + " GB";
            }
        }

        public static string GetSafeFormText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            StringBuilder result = new StringBuilder(text);
            result.Replace("\"", "&quot;");
            result.Replace("<", "&lt;");
            result.Replace(">", "&gt;");

            return result.ToString();
        }

        private static Regex scriptReg = null;

        /// <summary>
        /// 过滤HTML内容里的脚本
        /// </summary>
        /// <param name="sourceHtml">HTML内容</param>
        /// <returns>返回过滤后的</returns>
        public static string FilterScript(string sourceHtml)
        {
            if (scriptReg == null) scriptReg = new JavascriptRegex();
            sourceHtml = scriptReg.Replace(sourceHtml, string.Empty);
            return sourceHtml;
        }



        /// <summary>
        /// 清除末尾的换行和空格(性能差 用于发表的时候)
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string ClearEndLineFeedAndBlank(string content)
        {
            if (string.IsNullOrEmpty(content))
                return content;
            content = Regex.Replace(content, "<br>", "<br />", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, "<br/>", "<br />", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, "<br />", "<br />", RegexOptions.IgnoreCase);//主要作用是把大写转为小写 如"<Br />" 转成 "<br />"
            content = content.Replace("\n", "<br />");
            content = content.Replace("\r\n", "<br />");

            string[] contents = Regex.Split(content, "<br />");
            if (contents.Length > 1)
                content = ClearEndLineFeedAndBlank(contents, "<br />");

            //contents = content.Split('\n');

            //content = a(contents, "\n");


            return content.TrimEnd();
        }

        private static string ClearEndLineFeedAndBlank(string[] contents, string spliter)
        {
            StringBuilder result = new StringBuilder();

            bool hasContent = false;
            for (int i = contents.Length - 1; i > -1; i--)
            {
                if (hasContent == false)
                {
                    string temp = contents[i].Replace("&nbsp;", " ");

                    if (temp.Trim() != string.Empty)
                    {
                        result.Insert(0, contents[i].TrimEnd().Replace(" ", "&nbsp;"));
                        hasContent = true;
                    }
                }
                else
                {
                    result.Insert(0, spliter);
                    result.Insert(0, contents[i]);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// 如果内容只有换行和空格  返回空字符串   否则返回原内容
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Trim(string content)
        {
            if (string.IsNullOrEmpty(content) == false)
            {
                string tempContent = Regex.Replace(content, "(<br />)|(<br>)|(<br/>)|(&nbsp;)", string.Empty, RegexOptions.IgnoreCase);
                if (tempContent == string.Empty)
                    return string.Empty;
                else
                    return content;
            }

            return content;
        }


        //static readonly string[] excludeHtmlTags = new string[] { "body", "frame", "frameset", "html", "iframe", "style", "ilayer", "layer", "link", "meta", "applet", "form", "input", "select", "textarea" };//, "embed", "object","script"};


        ///// <summary>
        ///// 过滤HTML标签
        ///// </summary>
        ///// <param name="html"></param>
        ///// <returns></returns>
        //public static string ConvertHtmlToSafety(string html)
        //{
        //    string pattern="";
        //    html = FilterScript(html);//脚本
        //    foreach (string s in excludeHtmlTags)
        //    {
        //        pattern=string.Format("</?{0}.*?/?>",s);
        //        if (Regex.IsMatch(html, pattern, RegexOptions.IgnoreCase))
        //        {
        //            html = Regex.Replace(html, pattern, string.Empty, RegexOptions.IgnoreCase);
        //        }
        //    }
        //    return html;
        //}

        /// <summary>
        /// 对字符串进行Html解码
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string HtmlDecode(string content)
        {
            return HttpUtility.HtmlDecode(content);
        }

        /// <summary>
        /// 对字符串进行Html编码
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string HtmlEncode(string content)
        {
            return HttpUtility.HtmlEncode(content);
        }


        /// <summary>
        /// <函数：Decode>
        ///作用：将16进制数据编码转化为字符串，是Encode的逆过程
        /// </summary>
        /// <param name="strDecode"></param>
        /// <returns></returns>
        public static string HexDecode(string strDecode)
        {
            if (strDecode.IndexOf(@"\u") == -1)
                return strDecode;

            int startIndex = 0;
            if (strDecode.StartsWith(@"\u") == false)
            {
                startIndex = 1;
            }

            string[] codes = Regex.Split(strDecode, @"\\u");

            StringBuilder result = new StringBuilder();
            if (startIndex == 1)
                result.Append(codes[0]);
            for (int i = startIndex; i < codes.Length; i++)
            {
                try
                {
                    if (codes[i].Length > 4)
                    {
                        result.Append((char)short.Parse(codes[i].Substring(0, 4), global::System.Globalization.NumberStyles.HexNumber));
                        result.Append(codes[i].Substring(4));
                    }
                    else
                    {
                        result.Append((char)short.Parse(codes[i].Substring(0, 4), global::System.Globalization.NumberStyles.HexNumber));
                    }
                }
                catch
                {
                    result.Append(codes[i]);
                }
            }

            return result.ToString();
        }



        public static List<string> GetStrArray(string str, char speater, bool toLower)
        {
            List<string> list = new List<string>();
            string[] ss = str.Split(speater);
            foreach (string s in ss)
            {
                if (!string.IsNullOrEmpty(s) && s != speater.ToString())
                {
                    string strVal = s;
                    if (toLower)
                    {
                        strVal = s.ToLower();
                    }
                    list.Add(strVal);
                }
            }
            return list;
        }
        public static string[] GetStrArray(string str)
        {
            return str.Split(new char[',']);
        }
        public static string GetArrayStr(List<string> list, string speater)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == list.Count - 1)
                {
                    sb.Append(list[i]);
                }
                else
                {
                    sb.Append(list[i]);
                    sb.Append(speater);
                }
            }
            return sb.ToString();
        }


        #region 删除最后一个字符之后的字符

        /// <summary>
        /// 删除最后结尾的一个逗号
        /// </summary>
        public static string DelLastComma(string str)
        {
            return str.Substring(0, str.LastIndexOf(","));
        }

        /// <summary>
        /// 删除最后结尾的指定字符后的字符
        /// </summary>
        public static string DelLastChar(string str, string strchar)
        {
            return str.Substring(0, str.LastIndexOf(strchar));
        }

        #endregion




        /// <summary>
        /// 转全角的函数(SBC case)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSBC(string input)
        {
            //半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }

        /// <summary>
        ///  转半角的函数(SBC case)
        /// </summary>
        /// <param name="input">输入</param>
        /// <returns></returns>
        public static string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        public static List<string> GetSubStringList(string o_str, char sepeater)
        {
            List<string> list = new List<string>();
            string[] ss = o_str.Split(sepeater);
            foreach (string s in ss)
            {
                if (!string.IsNullOrEmpty(s) && s != sepeater.ToString())
                {
                    list.Add(s);
                }
            }
            return list;
        }


        #region 将字符串样式转换为纯字符串
        public static string GetCleanStyle(string StrList, string SplitString)
        {
            string RetrunValue = "";
            //如果为空，返回空值
            if (StrList == null)
            {
                RetrunValue = "";
            }
            else
            {
                //返回去掉分隔符
                string NewString = "";
                NewString = StrList.Replace(SplitString, "");
                RetrunValue = NewString;
            }
            return RetrunValue;
        }
        #endregion

        #region 将字符串转换为新样式
        public static string GetNewStyle(string StrList, string NewStyle, string SplitString, out string Error)
        {
            string ReturnValue = "";
            //如果输入空值，返回空，并给出错误提示
            if (StrList == null)
            {
                ReturnValue = "";
                Error = "请输入需要划分格式的字符串";
            }
            else
            {
                //检查传入的字符串长度和样式是否匹配,如果不匹配，则说明使用错误。给出错误信息并返回空值
                int strListLength = StrList.Length;
                int NewStyleLength = GetCleanStyle(NewStyle, SplitString).Length;
                if (strListLength != NewStyleLength)
                {
                    ReturnValue = "";
                    Error = "样式格式的长度与输入的字符长度不符，请重新输入";
                }
                else
                {
                    //检查新样式中分隔符的位置
                    string Lengstr = "";
                    for (int i = 0; i < NewStyle.Length; i++)
                    {
                        if (NewStyle.Substring(i, 1) == SplitString)
                        {
                            Lengstr = Lengstr + "," + i;
                        }
                    }
                    if (Lengstr != "")
                    {
                        Lengstr = Lengstr.Substring(1);
                    }
                    //将分隔符放在新样式中的位置
                    string[] str = Lengstr.Split(',');
                    foreach (string bb in str)
                    {
                        StrList = StrList.Insert(int.Parse(bb), SplitString);
                    }
                    //给出最后的结果
                    ReturnValue = StrList;
                    //因为是正常的输出，没有错误
                    Error = "";
                }
            }
            return ReturnValue;
        }
        #endregion
        /// <summary>
        /// 从字符串里随机得到，规定个数的字符串.
        /// </summary>
        /// <param name="allChar"></param>
        /// <param name="CodeCount"></param>
        /// <returns></returns>
        public static string GetRandomCode(string allChar, int CodeCount)
        {
            //string allChar = "1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,i,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z"; 
            string[] allCharArray = allChar.Split(',');
            string RandomCode = "";
            int temp = -1;
            Random rand = new Random();
            for (int i = 0; i < CodeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(temp * i * ((int)DateTime.Now.Ticks));
                }

                int t = rand.Next(allCharArray.Length - 1);

                while (temp == t)
                {
                    t = rand.Next(allCharArray.Length - 1);
                }

                temp = t;
                RandomCode += allCharArray[t];
            }
            return RandomCode;
        }




    }
}
