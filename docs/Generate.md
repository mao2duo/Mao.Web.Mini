# 文字範本

## 功能簡介

透過自定義的介面輸入參數，將參數透過程式的字串處理產生一或多個文字檔案

## 圖形示意

透過介面輸入參數

![img1](https://raw.githubusercontent.com/mao2duo/Mao.Static/master/develop-tools/img1.png)

依照參數輸出對應的結果

![img2](https://raw.githubusercontent.com/mao2duo/Mao.Static/master/develop-tools/img2.png)

# 建置專案

1. 下載以下三個位置的專案
    * https://github.com/mao2duo/Mao.Web.Mini  
      請取得分支「develop-tools」標籤「demo-1」的版本，避免於開發階段的異動過大與該文件說明不符
    * https://github.com/mao2duo/Mao.Library  
      如果取得最新版本使得主專案無法編譯，請移至標籤「develop-tools-1」的版本
    * https://github.com/mao2duo/Mao.Repository  
      如果取得最新版本使得主專案無法編譯，請移至標籤「develop-tools-1」的版本  
      
      將這三個目錄放置於同一層  
      ![img3](https://raw.githubusercontent.com/mao2duo/Mao.Static/master/develop-tools/img3.png)
  
2. 開啟 Mao.Web.Mini\Mao.Web.sln
3. 進行建置
        
# 環境建置

1. 將 Mao.Web.Mini\Mao.Web 架設成站台
2. 建立資料庫
3. 打開 Mao.Web.Mini\Mao.Web\Database\Migrations\2021-07-26-01.txt
4. 將內容複製至建立的資料庫執行
5. 修改 Mao.Web.Mini\Mao.Web\Web.config 中的連接字串指向建立的資料庫
6. 使用瀏覽器開啟站台，點擊上方的文字範本，即可選擇功能使用

# 預防針

1. 使用文字範本時，建議先點右上角進行註冊並登入再開始使用，否則儲存畫面資料的功能都將無法使用
2. 目前開發工具尚處於開發階段，許多防呆及基底功能都並非完善  
   甚至在選單看到的文字範本，大部分都是未完成的
    
# 新增文字範本

## 流程概述

1. 開啟 Mao.Web.Mini\Mao.Web.sln
2. 建立輸入畫面
    1. 在專案 Mao.Web 的目錄 Areas\Generate\Views\Generate 之下建立兩層目錄
        * 第一層目錄為框架名稱
        * 第二層目錄為功能名稱
    2. 在第二層目錄內建立 Input.cshtml
        * 設置 Layout = "~/Areas/Generate/Views/Generate/Input.cshtml";
        * 內容放置文字範本需要的輸入框
    3. 建立一個不限定名稱的類別
        * 屬性對應 Input.cshtml 的輸入框
        * 建議與 Input.cshtml 放在同一個目錄下
3. 建立文字範本生成結果
    1. 在專案 Mao.Web 的目錄 Features\Generators 之下建立兩層目錄
        * 與上一個步驟的兩層目錄名稱相同
    2. 在第二層目錄之下可建立任意的目錄結構與類別
        * 需要繼承 IGenerator&lt;T&gt;
        * T 為輸入畫面對應的類別
        * 命名空間必須在 Mao.Web.Features.Generators.(框架名稱).(功能名稱) 之下
    3. 實作方法 Generate

## 使用文字範本建立文字範本

進到文字範本區塊，找到選單中的文字範本，即可輸入框架名稱與功能名稱來建立上述的檔案

![img4](https://raw.githubusercontent.com/mao2duo/Mao.Static/master/develop-tools/img4.png)

這個範本只會建立一個繼承 IGenerator&lt;T&gt; 的類別，如果需要多個直接複製修改名稱即可

![img5](https://raw.githubusercontent.com/mao2duo/Mao.Static/master/develop-tools/img5.png)

## 進階說明

為了方便說明，以下
  * 將流程概述中產生的 Input.cshtml 稱為 View
  * 將第一步驟建立的類別稱為 ViewModel
  * 將第二步驟建立的類別稱為 Generator

### View

設置 Layout = "~/Areas/Generate/Views/Generate/Input.cshtml"; 之後  
即可專注於放置 input，其名稱對應 ViewModel 的 PropertyName  
即可將畫面輸入的值傳遞至 ViewModel

### ViewModel

除了無繼承的類別以外，你也可以選擇繼承 Mao.Web.Features.Interfaces.IGeneratorRequest 這個介面來選擇是否套用以下功能
* 將 UseDefaultStringEmpty 設置為 true 可以確保所有 string 類型的屬性在 Generator 運行時不會是 null (以空字串取代)
* 將 UseDefaultInstance 設置為 true 可以確保所有 class 類型的屬性在 Generator 運行時不會是 null (以 new() 的物件來取代)
* 將 UseDefaultEmptyCollection 設置為 true 可以確保所有 Array/List 類型的屬性在 Generator 運行時不會是 null (以空集合取代)
* 將 UseUpdateModel 設置為 true，表示套用 MVC 原本 ModelBinding 機制 (若不清楚這個需求，設置為 true 即可)
* 實作 ReceiveRequestForm 可以接收 Request.Form 的 NameValueCollection 來自訂屬性獲取值的方式  
  (若不清楚這個需求，在方法內放入 throw new NotImplementedException(); 即可)
* 實作 OnAfterUpdateModel 可以在 Generator 運行之前，執行自定義的程式  
  (若不清楚這個需求，在方法內放入 throw new NotImplementedException(); 即可)
  
![img6](https://raw.githubusercontent.com/mao2duo/Mao.Static/master/develop-tools/img6.png)

### Generator
* 一個 ViewModel 可以對應多個 Generator
* IGenerator&lt;T&gt; 中唯一的方法 Generate
    * 回傳類型為 GenerateOutputFiles.Response.Files 是 GenerateOutputFiles.Response.File 的集合  
      這表示你可以在一個 Generator 產生多個文字檔案的結果
    * 可以回傳 GenerateOutputFiles.Response.File 類型的物件，表示只產生一個結果 (會透過隱含轉換為 GenerateOutputFiles.Response.Files)
    * 可以回傳 null 表示不產出任何結果
* GenerateOutputFiles.Response.File 主要可以設置
    * DirectoryPath: 檔案存放的目錄位置
    * Name: 檔案名稱
    * Content: 檔案內容

  如果沒有設置 DirectoryPath 則會以 Generator 的 namespace 來作為目錄  
  如果想要以 namespace 來作為目錄的一部分，你可以在 DirectoryPath 中以 {0} 來表示 namespace 轉換的路徑  
  另外可以設置 Description 來對檔案進行備註
* 如果你需要非同步 (async) 的方法，你可以將原本繼承 IGenerator&lt;T&gt; 改為繼承 IAsyncGenerator&lt;T&gt;  
  其 IAsyncGenerator&lt;T&gt;.GenerateAsync 的使用方式與 IGenerator&lt;T&gt;.Generate 相同
  
## 進階開發說明

### View

* View 預設是將 input 透過 serializeArray 的結果送至 Server  
  如果你想要改變這個方法，可以在 View 加入一個名稱為 overrideGetRequestAsync(callback) 的方法  
  將想要送至 Server 的結果作為 callback 的第一個參數呼叫 callback 即可
* 如果 View 有動態產生的控制項，通常會需要加入 overrideSetRequestAsync(request) 這個方法
  在載入文字範本參數時，先透過 request 的內容建立動態的控制項，再呼叫 defaultSetRequestAsync(request);
  
### ViewModel

* 一個 View 可以對應多個 ViewModel  
    * 每個 ViewModel 可以各自定義需要的參數
    * 每個 ViewModel 可以各自選擇是否繼承 IGeneratorRequest，並各自實作 IGeneratorRequest 的內容
    
### Generator

* 當 View 有對應多個 ViewModel 的時候  
  Generator 只需要選擇一個 ViewModel 作為 IGenerator&lt;T&gt; 的泛型類別即可
  
## 新增選單

上述步驟建立文字範本後，還需要添加到選單上，這樣才能從介面進入  
目前選單的管理功能還在開發階段，故提供 SQL 語法，從資料庫加入選單
```
-- 設置框架名稱
DECLARE @Provider NVARCHAR(200) = ''
-- 設置功能名稱
DECLARE @Module NVARCHAR(200) = ''
DECLARE @MenuId UNIQUEIDENTIFIER = Newid()
-- 設定選單名稱
DECLARE @MenuText UNIQUEIDENTIFIER = ''
-- 設置上層選單 Id (預設的值為「通用」)
DECLARE @ParentId UNIQUEIDENTIFIER = '3e3f51f6-903b-409d-bb9c-90484c2d0184'

INSERT INTO [AppMenu]
([Id], [ParentId], [Name], [Icon], [Text], [Href], [Target], [Sort], [IsActivated], [IsLink])
VALUES
(@MenuId, @ParentId, NULL, NULL, @MenuText, NULL, NULL, (SELECT Isnull(Max(Sort), 0) + 1 FROM [AppMenu] WHERE ParentId = @ParentId), 1, 1)

INSERT INTO [AppMenuRoute]
([MenuId], [Name], [Value])
VALUES
(@MenuId, N'Action', N'Input'),
(@MenuId, N'Controller', N'Generate'),
(@MenuId, N'Module', @Module),
(@MenuId, N'Provider', @Provider) 
```
