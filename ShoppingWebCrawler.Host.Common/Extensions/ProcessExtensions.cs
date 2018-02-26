using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace System.Diagnostics
{
    public static class ProcessExtensions
    {
        /// <summary>
        /// 获取进程的命令行信息
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static string GetCommandLine(this Process process)
        {
            var commandLine = new StringBuilder();

            commandLine.Append(" ");
            using (var searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
            {
                foreach (var @object in searcher.Get())
                {
                    commandLine.Append(@object["CommandLine"]);
                    commandLine.Append(" ");
                }
            }

            return commandLine.ToString();
        }
    }
}
