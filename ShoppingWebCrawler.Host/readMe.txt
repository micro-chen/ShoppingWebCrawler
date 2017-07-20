    HeadLessWebBrowerApp  实现了无头浏览器的操作

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

