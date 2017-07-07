    HeadLessWebBrowerApp  实现了无头浏览器的操作

	//取消了GPU的功能  在无头模式下 gp 加速会出现崩溃的问题 
	
	protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            //禁止使用gpu 加速 在headless 模式下  gpu 有问题
            //commandLine.AppendSwitch("disable-gpu", "1");
            commandLine.AppendSwitch("disable-gpu");
            commandLine.AppendSwitch("disable-gpu-compositing");
            commandLine.AppendSwitch("enable-begin-frame-scheduling");
            commandLine.AppendSwitch("disable-smooth-scrolling");
        }

