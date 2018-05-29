# ShoppingWebCrawler
This Project is a WebCrawler build by .net framework .
<br/>
<img src="https://images2018.cnblogs.com/blog/371989/201805/371989-20180514183359583-1996553911.jpg" alt="" width="623" height="350" />
<br/>

本项目是一个基于使用微软.net framework 结合Google的webkit内核做的蜘蛛采集工具。
支持多进程的集群模式。实现高性能的蜘蛛采集！
# 项目概述
使用此工具，进行电商平台的数据采集。
本项目已经实现可以采集淘宝、天猫、京东、拼多多、一号店、国美、苏宁等主流电商平台的网页数据。
# 实现核心
1、使用基于 Xilium.CefGlue 的libcef绑定，实现C#操作webkit。进行可视化的登陆授权。不定时刷新，进行登陆状态的模拟和守护。  

2、使用Topshelf+libcef的Headless 模式（无头模式），进行windows 服务承载。对蜘蛛进程进行挂载守护。  

3、使用log4net进行日志记录  

4、使用Quartz.Net 进行定时任务Schduler。  

5、服务进程使用自定义高性能Socket（NTCPMessage）进行网络通信。对来自服务Client的请求进行请求应答。  

6、集群模式，使用简易的多进程实现集群。开启不同的监听端口，实现采集任务的负载均衡，进而大幅度提升硬件服务器的使用效率。  

# libcef 简介
The Chromium Embedded Framework (CEF)－－－－Chromium嵌入式框架。CEF聚焦于使用第三方嵌入浏览器的应用程序。支持嵌入HTML5浏览器在现有的本地应用程序中。比如嵌入MFC窗口。创建一个轻量级sehll应用程序，渲染网页内容。
libcef为为我们提供了CEF的运行时接口。为我们开发基于cef 运行时，提供了API。基于libcef 轻松创建基于html javascript的web 应用。就像打开网页一样。
（注：本质就是浏览器）
libcef的运行时体积略大，不方便推送到项目中，在下面的网址，百度云盘上，分享了32位和64位的运行时。
https://pan.baidu.com/s/19WPAny7nqZR_UwINj9zfcQ  
  
下载后：解压到项目的根 libs目录            
ibs  
│  jint.dll  
│  libcef_runtime_3.2623.7z  
│  Newtonsoft.Json.dll  
│  System.Net.Http.dll  
│  
├─x64  
│  │  cef.pak  
│  │  cef_100_percent.pak  
│  │  cef_200_percent.pak  
│  │  cef_extensions.pak  
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
     │ en-GB.pak  
     │ en-US.pak  
     │ zh-CN.pak  
     │ zh-TW.pak  

# Xilium.CefGlue 简介

Xilium.CefGlue是对CEF项目的.net的包装，它是用P/Invoke的方式来调用CEF类库的，请参见：https://bitbucket.org/xilium/xilium.cefglue/wiki/Home。
使用Xilium.CefGlue 可以实现.net 操作Chrome浏览器内核。进而实现浏览器网页加载，js V8的实现。

# Topshelf简介
Topshelf是创建Windows服务的一种方法。Topshelf是一个开源的跨平台的宿主服务框架，支持Windows和Mono，只需要几行代码就可以构建一个很方便使用的服务宿主。

引用安装
1、官网：http://topshelf-project.com/  这里面有详细的文档及下载  


安装：TopshelfDemo.exe install  

启动：TopshelfDemo.exe start  

重启：TopshelfDemo.exe restart  

卸载：TopshelfDemo.exe uninstall  


# 什么是Headless浏览器？
简单的说就是一个没有UI界面的浏览器。使用命令行进行代码控制浏览器行为，常见于自动化单元测试。

# 如何使用？
1、下载源码到本地。比如：d:\src  

2、使用visual studio2015 打开项目并编译。  

3、配置Redis 环境。本项目使用redis 进行进程间的cookie共享，从而实现登录凭据cookie的跨进程共享。在UI进程和Heaadless进程间进行Cookie共享。  

4、运行 ShoppingWebCrawler.Host 项目，即可运行。  

# 如何使用界面工具进行请求的可视化？
编译 ShoppingWebCrawler.Host.DeskTop ，得到UI 工具，可以对打开一个网址。比如登录淘宝，就可以在本地进程间共享淘宝登录凭据。从而实现
特定的蜘蛛采集任务。比如：采集某个类目的商品。采集商品优惠券。
# 如何在Windows 服务进行承载？
编译项目ShoppingWebCrawler.Host.WindowService,然后去项目的输出目录，在cmd 、powershell 定位到此目录。执行：

ShoppingWebCrawler.Host.WindowService.exe install 即可。如果想卸载，那么执行指令：ShoppingWebCrawler.Host.WindowService.exe uninstall .  

参考topshelf的命令。
# 如何开启集群模式？
在项目ShoppingWebCrawler.Host 的app.config文件中  

    <!--是否开启集群模式-->
    <add key="ClusteringMode" value="true"/>
    <!--集群子节点数量-->
    <add key="ClusterNodeCount" value="3"/>
    
# 联系作者
MyBlog:http://www.cnblogs.com/micro-chen/
<br/>
DotNET Core技术群: 59196458
# 赞助作者
一个好的项目离不开大家的支持，您的赞助，将给我更加充沛的动力。
<br/>
<br/>
<img src="https://images2018.cnblogs.com/blog/371989/201805/371989-20180514183954632-2054296110.jpg" alt="" />

