using System;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;

namespace ShoppingWebCrawler.Host.Common
{
    /// <summary>
    /// 数据校验类
    /// </summary>
    public class InPutValidate
    {
        public static Regex RegPhone = new Regex("^[0-9]+[-]?[0-9]+[-]?[0-9]$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static Regex RegNumberSign = new Regex("^[+-]?[0-9]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static Regex RegDecimal = new Regex("^[0-9]+[.]?[0-9]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static Regex RegDecimalSign = new Regex("^[+-]?[0-9]+[.]?[0-9]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase); //等价于^[+-]?\d+[.]?\d+$
        public static Regex RegEmail = new Regex("^[\\w-]+@[\\w-]+\\.(com|net|org|edu|mil|tv|biz|info)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);//w 英文字母或数字的字符串，和 [a-zA-Z0-9] 语法一样 
        public static Regex RegCHZN = new Regex("[\u4e00-\u9fa5]");
        public static Regex RegexUrl = new Regex("((http://|https://|www\\.)([A-Z0-9.\\-]{1,})\\.[0-9A-Z?;~&\\(\\)#,=\\-_\\./\\+]{2,})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static Regex RegexJavaScriptPattern = new Regex(@"<script.*?>(.|\n)*?</script\s*>|(?<=<\w+.*?)\son\w+="".+?""(?=.*?>)|<\w{2,}\s+[^>]*?javascript:[^>]*?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);


        public InPutValidate()
        {
        }


        #region 数字字符串检查		
        public static bool IsPhone(string inputData)
        {
            Match m = RegPhone.Match(inputData);
            return m.Success;
        }


        /// <summary>
        /// 判断一个字符串是否是有效的布尔值
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsBooleanFormat(string text)
        {

            return Regex.IsMatch(text, "^(true|false)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        }

        /// <summary>
        /// 判断一个字符串是否是有效的整数值
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsIntegerFormat(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            int totalChars = text.Length;
            for (int i = 0; i < totalChars; i++)
            {
                if (char.IsNumber(text[i]) == false)
                    return false;
            }
            return true;
        }


        /// <summary>
        /// 是否只是包含数字
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static bool IsNumber(string txt)
        {
            var result = true;

            for (int i = 0; i < txt.Length; i++)
            {
                if (txt[i] < '0' || txt[i] > '9')
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
        /// <summary>
        /// 是否数字字符串 可带正负号
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsNumberSign(string inputData)
        {
            Match m = RegNumberSign.Match(inputData);
            return m.Success;
        }
        /// <summary>
        /// 是否是浮点数
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsDecimal(string inputData)
        {
            Match m = RegDecimal.Match(inputData);
            return m.Success;
        }
        /// <summary>
        /// 是否是浮点数 可带正负号
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsDecimalSign(string inputData)
        {
            Match m = RegDecimalSign.Match(inputData);
            return m.Success;
        }

        #endregion

        #region 中文检测

        /// <summary>
        /// 检测是否有中文字符
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static bool IsHasCHZN(string inputData)
        {
            Match m = RegCHZN.Match(inputData);
            return m.Success;
        }

        #endregion

        #region 邮件地址
        /// <summary>
        /// 是否是浮点数 可带正负号
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsEmail(string inputData)
        {
            Match m = RegEmail.Match(inputData);
            return m.Success;
        }

        #endregion

        #region 日期格式判断
        /// <summary>
        /// 日期格式字符串判断
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDateTime(string str)
        {
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    DateTime.Parse(str);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 是否是Url格式的字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsUrl(string str)
        {
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    return RegexUrl.IsMatch(str);
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion


        #region 是否由特定字符组成
        public static bool isContainSameChar(string strInput)
        {
            string charInput = string.Empty;
            if (!string.IsNullOrEmpty(strInput))
            {
                charInput = strInput.Substring(0, 1);
            }
            return isContainSameChar(strInput, charInput, strInput.Length);
        }

        public static bool isContainSameChar(string strInput, string charInput, int lenInput)
        {
            if (string.IsNullOrEmpty(charInput))
            {
                return false;
            }
            else
            {
                Regex RegNumber = new Regex(string.Format("^([{0}])+$", charInput));
                //Regex RegNumber = new Regex(string.Format("^([{0}]{{1}})+$", charInput,lenInput));
                Match m = RegNumber.Match(strInput);
                return m.Success;
            }
        }
        #endregion

        #region 检查输入的参数是不是某些定义好的特殊字符：这个方法目前用于密码输入的安全检查
        /// <summary>
        /// 检查输入的参数是不是某些定义好的特殊字符：这个方法目前用于密码输入的安全检查
        /// </summary>
        public static bool isContainSpecChar(string strInput)
        {
            string[] list = new string[] { "123456", "654321" };
            bool result = new bool();
            for (int i = 0; i < list.Length; i++)
            {
                if (strInput == list[i])
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        #endregion
    }
}
