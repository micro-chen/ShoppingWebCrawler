    HeadLessWebBrowerApp  实现了无头浏览器的操作
	  // Instruct CEF to not render to a window at all.
            CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
            cefWindowInfo.SetAsWindowless(IntPtr.Zero, true);

            // Settings for the browser window itself (e.g. should JavaScript be enabled?).
            var cefBrowserSettings = new CefBrowserSettings();

            // Initialize some the cust interactions with the browser process.
            // The browser window will be 1280 x 720 (pixels).
            var cefClient = new HeadLessCefClient(1, 1);
            var loader = cefClient.GetCurrentLoadHandler();
            loader.BrowserCreated += (s, e) =>
            {

                //事件通知 当cef  browser 创建完毕
                //创建完毕后 保存 browser 对象的实例
                var brw = e.Browser;
                var etaoBrowser = new CookiedCefBrowser { CefBrowser = brw, CefLoader = loader, CefClient = cefClient };

                tcs.TrySetResult(etaoBrowser);
            };
            ////注册  加载完毕事件handler
            //loader.LoadEnd += this.OnWebBrowserLoadEnd;
            // Start up the browser instance.
            string url = "about:blank";
            CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings, url);
             

	在部署的时候 一定要部署N>1 个实例  一个程序实例 在运行基于CEF内核操作数据请求的时候 ，效果不理想
并且在单个程序进程中，用多个线程去处理CEF内核 会出现 Cookie 抢用的情况；比如操作同一个网址  出现设置同名Cookie
所以  用多个进程实例，使用端口注册的方式 ，将请求路由到不同的程序实例上。还可以减轻 实例的压力！


	//取消了GPU的功能  在无头模式下 gp 加速会出现崩溃的问题 
	https://bitbucket.org/xilium/xilium.cefglue/commits/4146c2b46923593f55d28c7435f017631f86dca0
	protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            //禁止使用gpu 加速 在headless 模式下  gpu 有问题
            //commandLine.AppendSwitch("disable-gpu", "1");
            commandLine.AppendSwitch("disable-gpu");
            commandLine.AppendSwitch("disable-gpu-compositing");
            commandLine.AppendSwitch("enable-begin-frame-scheduling");
            commandLine.AppendSwitch("disable-smooth-scrolling");
        }


 //1 将其他的同名残留进程杀死，基于多进程的CEF
          
            //try
            //{   
            //    Process mainProcess = Process.GetCurrentProcess();
            //    var appName = Assembly.GetExecutingAssembly().GetName().Name;
            //    var psArray = Process.GetProcessesByName(appName);
            //    foreach (var ps in psArray)
            //    {
            //        if (ps.Id != mainProcess.Id)
            //        {
            //            ps.Kill();//终止同名的 
            //        }


            //    }

            //}
            //catch { }
***********必须将 项目根目录下的libs 文件夹 拷贝对应的 libcef文件
libs 文件目录：
卷序列号为 00000235 4010:61AA
. \LIBS
│  jint.dll
│  Newtonsoft.Json.dll
│  System.Net.Http.dll
│
├─x64
│  │  cef.pak
│  │  cef_100_percent.pak
│  │  cef_200_percent.pak
│  │  cef_extensions.pak
│  │  cef_sandbox.lib
│  │  d3dcompiler_43.dll
│  │  d3dcompiler_47.dll
│  │  devtools_resources.pak
│  │  icudtl.dat
│  │  libcef.dll
│  │  libcef.lib
│  │  libEGL.dll
│  │  libGLESv2.dll
│  │  natives_blob.bin
│  │  snapshot_blob.bin
│  │  widevinecdmadapter.dll
│  │
│  └─locales
│          en-GB.pak
│          en-US.pak
│          zh-CN.pak
│          zh-TW.pak
│
└─x86
    │  cef.pak
    │  cef_100_percent.pak
    │  cef_200_percent.pak
    │  cef_extensions.pak
    │  d3dcompiler_43.dll
    │  d3dcompiler_47.dll
    │  devtools_resources.pak
    │  icudtl.dat
    │  libcef.dll
    │  libcef.lib
    │  libEGL.dll
    │  libGLESv2.dll
    │  natives_blob.bin
    │  snapshot_blob.bin
    │  widevinecdmadapter.dll
    │  wow_helper.exe
    │
    └─locales
            en-GB.pak
            en-US.pak
            zh-CN.pak
            zh-TW.pak
