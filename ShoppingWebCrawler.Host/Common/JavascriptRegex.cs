
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace ShoppingWebCrawler.Host.Common
{

    /// <summary>
    /// 匹配HTML代码里的脚本
    /// </summary>
    public class JavascriptRegex:Regex
    {
        private const string scriptPattern = @"<script.*?>(.|\n)*?</script\s*>|(?<=<\w+.*?)\son\w+="".+?""(?=.*?>)|<\w{2,}\s+[^>]*?javascript:[^>]*?>";

        public JavascriptRegex()
        :base( scriptPattern, RegexOptions.IgnoreCase| RegexOptions.Compiled)
        {

        }
    }
}