# GenshenCharactorApp

## 一、界面演示

### 1、首页

!["首页1"](/Assets/Home_1.png)

!["首页2"](/Assets/Home_2.png)

!["首页3"](/Assets/Home_3.png)

### 2、新闻

!["新闻1"](/Assets/New_1.png)

!["新闻2"](/Assets/New_2.png)

### 3、角色

!["角色"](/Assets/Charactor.png)

### 4、世界

!["世界"](/Assets/World.png)

## 二、项目展示

本项目使用 WPF 桌面开发工具(C#)进行开发。

### 1、项目框架

采用了 WPF 开发中非常热门的 MVVM 模式框架 Prism。

![](project.png)

将界面分成了 HomeView，NewsView，CharactorView，WorldView 四个导航界面。
通过 Prism 提供的 RegionManager 实现界面的导航功能。

暂时只做了四个子界面，点击其他界面会转跳至对应的 Web 界面。

### 2、后端数据获取

对原神官网的网页进行后端接口的分析，获得后端数据的 Http 请求方式。

下面是请求原神区域角色数据的 WebApi：
***https://api-takumi-static.mihoyo.com/content_v2_user/app/16471662a82d418a/getContentList?iAppId=43&iChanId=742&iPageSize=50&iPage=1&sLangKey=zh-cn***

