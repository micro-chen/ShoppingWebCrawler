using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingWebCrawler.Host.WindowService.App_Start;

namespace ShoppingWebCrawler.Host.WindowService
{
    class Program
    {
        static void Main(string[] args)
        {
            WinServiceConfig.Init();
        }
    }
}
