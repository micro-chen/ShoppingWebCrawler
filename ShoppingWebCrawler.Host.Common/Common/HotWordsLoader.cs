using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingWebCrawler.Host.Common
{
    /// <summary>
    /// 从文本文件读取热搜词
    /// https://top.taobao.com/index.php?spm=a1z5i.1.2.1.hUTg2J&topId=HOME
    /// 定期维护下这个静态文本文件 长期不更新也没事
    /// </summary>
    public static class HotWordsLoader
    {
        /// <summary>
        /// 静态字段
        /// </summary>
        private static string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "HotWords.txt");

        /// <summary>
        /// 固定搜索词
        /// </summary>
        private const string FIX_WORD = "面膜";
        /// <summary>
        /// 从 Configs\HotWords.txt 读取热搜词集合
        /// </summary>
        /// <returns></returns>
        public static List<string> LoadConfig()
        {

            var lstData = new List<string> { FIX_WORD };

            var allLines = File.ReadAllLines(configFilePath);
            if (null != allLines && allLines.Length > 0)
            {
                int pos = 0;
                while (pos < allLines.Length)
                {
                    string word = allLines[pos].Trim();
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        lstData.Add(word);
                    }
                    pos += 1;
                }
            }

            return lstData;
        }

        /// <summary>
        /// 获取一个随机的热搜词
        /// </summary>
        /// <returns></returns>
        public static string GetRandHotWord()
        {
            string result = string.Empty;
            int min = 0;
            int max = GlobalContext.HotWords.Count;

            int mixdPos = new Random(DateTime.Now.Millisecond).Next(min, max);

            result = GlobalContext.HotWords[mixdPos];

            if (string.IsNullOrEmpty(result))
            {
                result = FIX_WORD;
            }
            return result;
        }


    }
}
