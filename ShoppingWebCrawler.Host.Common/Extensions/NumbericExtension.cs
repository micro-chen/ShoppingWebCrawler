using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class NumbericExtension
    {
        /// <summary>
        /// 四舍五入法
        /// </summary>
        /// <param name="sourceNum">要进行处理的数据</param>
        /// <param name="toRemainIndex">保留的小数位数</param>
        /// <returns>四舍五入后的结果</returns>
        public static decimal ChineseRound(this decimal sourceNum, int toRemainIndex)
        {
            decimal result = sourceNum;
            string sourceString = sourceNum.ToString();
            //没有小数点,则返回原数据+"."+"保留小数位数个0"
            if (!sourceString.Contains("."))
            {
                result = Convert.ToDecimal(sourceString + "." + CreateZeros(toRemainIndex));
                return result;
            }
            //小数点的位数没有超过要保留的位数,则返回原数据+"保留小数位数 - 已有的小数位"个0
            if ((sourceString.Length - sourceString.IndexOf(".") - 1) <= toRemainIndex)
            {
                result = Convert.ToDecimal(sourceString + CreateZeros(toRemainIndex - (sourceString.Length - sourceString.IndexOf(".") - 1)));
                return result;
            }
            string beforeAbandon_String = string.Empty;
            beforeAbandon_String = sourceString.Substring(0, sourceString.IndexOf(".") + toRemainIndex + 1);
            //取得如3.1415926保小数点后4位(原始的,还没开始取舍)，中的3.1415
            decimal beforeAbandon_Decial = Convert.ToDecimal(beforeAbandon_String);

            //如果保留小数点后N位，则判断N+1位是否大于等于5，大于，则进一，否则舍弃。
            if (int.Parse(sourceString.Substring(sourceString.IndexOf(".") + toRemainIndex + 1, 1)) >= 5)
            {
                string toAddAfterPoint = "0." + CreateZeros(toRemainIndex - 1) + "1";
                result = beforeAbandon_Decial + Convert.ToDecimal(toAddAfterPoint);
            }
            else
            {
                result = beforeAbandon_Decial;
            }
            return result;
        }

        public static bool IsNumeric(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            int len = value.Length;
            if ('-' != value[0] && '+' != value[0] && !char.IsNumber(value[0]))
            {
                return false;
            }
            for (int i = 1; i < len; i++)
            {
                if (!char.IsNumber(value[i]))
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// 补 "0"方法.
        /// </summary>
        /// <param name="zeroCounts">生成个数.</param>
        /// <returns></returns>
        private static string CreateZeros(int zeroCounts)
        {
            string Result = string.Empty;
            if (zeroCounts == 0)
            {
                Result = "";
                return Result;
            }
            for (int i = 0; i < zeroCounts; i++)
            {
                Result += "0";
            }
            return Result;
        }
    }
}
