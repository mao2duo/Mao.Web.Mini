using Mao.Web.Features.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mao.Web.ApiActions
{
    public class GenerateOutputFiles
    {
        public class Request : IRequest<Response>
        {
            public string Provider { get; set; }
            public string Module { get; set; }
            internal Action<object> UpdateModel { get; set; }
        }

        public class Response
        {
            public string Provider { get; set; }
            public string Module { get; set; }
            public Directory Container { get; set; }
            public ModelStateDictionary ModelState { get; set; }

            public class File
            {
                public string DirectoryPath { get; set; }
                public string Name { get; set; }
                public string Description { get; set; }
                public string Content { get; set; }
            }

            public class Files : System.Collections.ObjectModel.Collection<File>
            {
                public Files() : base() { }
                public Files(IList<File> files) : base(files) { }

                public static implicit operator Files(File file) => new Files(new[] { file });
                public static implicit operator Files(File[] files) => new Files(files);
                public static implicit operator Files(List<File> files) => new Files(files);
            }

            public class Directory
            {
                public string Name { get; set; }
                public List<Directory> Directories { get; set; }
                public List<File> Files { get; set; }
            }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IServiceProvider _serviceProvider;
            public Handler(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                string baseNamespace = $"{nameof(Mao)}.Web.Features.Generators.{request.Provider}.{request.Module}";
                var generatorTypes = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(x => x.Namespace != null)
                    .Where(x => x.Namespace == baseNamespace || x.Namespace.StartsWith($"{baseNamespace}."))
                    .Where(x => x.IsClass && !x.IsAbstract)
                    .Where(x => x.GetInterfaces()
                        .Any(y => y.IsGenericType
                            && (y.GetGenericTypeDefinition() == typeof(IGenerator<>)
                                || y.GetGenericTypeDefinition() == typeof(IAsyncGenerator<>))))
                    .OrderBy(x => x.Namespace)
                    .ToArray();
                Response.Directory container = new Response.Directory()
                {
                    Directories = new List<Response.Directory>(),
                    Files = new List<Response.File>()
                };
                // 將 namespace 轉換成儲存路徑
                Func<string, string> getPath = @namespace =>
                {
                    if (@namespace == baseNamespace)
                    {
                        return "";
                    }
                    return @namespace.Substring(baseNamespace.Length + 1).Split('.').Join("\\");
                };
                // 從儲存路徑取得階層中對應的物件
                Func<string, Response.Directory> getDirectory = path =>
                {
                    path = path?.Trim('/', '\\');
                    if (string.IsNullOrWhiteSpace(path))
                    {
                        return container;
                    }
                    var directoryNames = path.Split('/', '\\');
                    Response.Directory currentDirectory = container;
                    foreach (var directoryName in directoryNames)
                    {
                        var directory = currentDirectory.Directories.FirstOrDefault(x => x.Name == directoryName);
                        if (directory == null)
                        {
                            directory = new Response.Directory()
                            {
                                Name = directoryName,
                                Directories = new List<Response.Directory>(),
                                Files = new List<Response.File>()
                            };
                            currentDirectory.Directories.Add(directory);
                        }
                        currentDirectory = directory;
                    }
                    return currentDirectory;
                };
                // 取得每一個範本的類別
                Dictionary<Type, object> generatorTypeRequests = new Dictionary<Type, object>();
                foreach (var generatorType in generatorTypes)
                {
                    // 取得範本的輸入類型
                    var generatorRequestType = generatorType.GetInterfaces()
                        .First(x => x.IsGenericType
                            && (x.GetGenericTypeDefinition() == typeof(IGenerator<>)
                                || x.GetGenericTypeDefinition() == typeof(IAsyncGenerator<>)))
                        .GetGenericArguments()[0];
                    object generatorRequest;
                    // 判斷是否有產生過相同輸入類型的物件
                    if (generatorTypeRequests.ContainsKey(generatorRequestType))
                    {
                        // 有產生過輸入類型的物件，重複使用即可
                        generatorRequest = generatorTypeRequests[generatorRequestType];
                    }
                    else
                    {
                        // 建立範本輸入物件
                        generatorRequest = ActivatorUtilities.CreateInstance(_serviceProvider, generatorRequestType);
                        // 判斷範本輸入物件是否為 IGeneratorRequest
                        if (generatorRequest is IGeneratorRequest regularGeneratorRequest)
                        {
                            #region IGeneratorRequest
                            // 判斷 IGeneratorRequest 是否需要 UpdateModel
                            if (regularGeneratorRequest.UseUpdateModel)
                            {
                                request.UpdateModel.Invoke(regularGeneratorRequest);
                            }
                            // 然後把 Request.Form 傳遞給 IGeneratorRequest
                            Invoker.Try<NotImplementedException>(
                                () => regularGeneratorRequest.ReceiveRequestForm(HttpContext.Current.Request.Form), null);
                            // 執行 OnAfterUpdateModel
                            Invoker.Try<NotImplementedException>(
                                () => regularGeneratorRequest.OnAfterUpdateModel(), null);
                            // 判斷 IGeneratorRequest 是否設置 UseDefaultInstance
                            if (regularGeneratorRequest.UseDefaultInstance)
                            {
                                Action<object> setNewInstance = null;
                                setNewInstance = x =>
                                {
                                    foreach (var prop in x.GetType().GetProperties())
                                    {
                                        if (prop.PropertyType.IsClass
                                            && !prop.PropertyType.IsArray)
                                        {
                                            var propValue = prop.GetValue(x);
                                            if (propValue == null)
                                            {
                                                // 尋找建構子建立物件
                                                var propConstructor = prop.PropertyType.GetConstructor(new Type[0]);
                                                if (propConstructor != null)
                                                {
                                                    propValue = propConstructor.Invoke(new object[0]);
                                                    prop.SetValue(x, propValue);
                                                }
                                            }
                                            // 往 class 的屬性去尋找
                                            if (propValue != null)
                                            {
                                                setNewInstance(propValue);
                                            }
                                        }
                                    }
                                };
                                setNewInstance.Invoke(regularGeneratorRequest);
                            }
                            // 判斷 IGeneratorRequest 是否設置 UseDefaultStringEmpty
                            if (regularGeneratorRequest.UseDefaultStringEmpty)
                            {
                                Action<object> setStringEmpty = null;
                                setStringEmpty = x =>
                                {
                                    foreach (var prop in x.GetType().GetProperties())
                                    {
                                        if (prop.PropertyType == typeof(string))
                                        {
                                            // 找到 string 之後把 null 設置為 string.Empty
                                            if (prop.GetValue(x) == null)
                                            {
                                                prop.SetValue(x, string.Empty);
                                            }
                                        }
                                        else if (prop.PropertyType.IsClass
                                            && !typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType))
                                        {
                                            // 往 class 的屬性去尋找
                                            var propValue = prop.GetValue(x);
                                            if (propValue != null)
                                            {
                                                setStringEmpty(propValue);
                                            }
                                        }
                                        else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType))
                                        {
                                            // 往集合的 class 的屬性去尋找
                                            var enumerableGenericType = prop.PropertyType.GetInterface("IEnumerable`1")?.GetGenericArguments().First();
                                            if (enumerableGenericType != null && enumerableGenericType.IsClass)
                                            {
                                                var propValues = prop.GetValue(x) as System.Collections.IEnumerable;
                                                if (propValues != null)
                                                {
                                                    foreach (var propValue in propValues)
                                                    {
                                                        setStringEmpty(propValue);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                };
                                setStringEmpty.Invoke(regularGeneratorRequest);
                            }
                            // 判斷 IGeneratorRequest 是否設置 UseDefaultEmptyCollection
                            if (regularGeneratorRequest.UseDefaultEmptyCollection)
                            {
                                Action<object> setEmptyCollection = null;
                                setEmptyCollection = x =>
                                {
                                    foreach (var prop in x.GetType().GetProperties())
                                    {
                                        if (prop.PropertyType.IsClass
                                            && !typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType))
                                        {
                                            // 往 class 的屬性去尋找
                                            var propValue = prop.GetValue(x);
                                            if (propValue != null)
                                            {
                                                setEmptyCollection(propValue);
                                            }
                                        }
                                        else if (prop.PropertyType != typeof(string)
                                            && typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType))
                                        {
                                            // 取得集合的類型
                                            var enumerableGenericType = prop.PropertyType.GetInterface("IEnumerable`1")?.GetGenericArguments().First();
                                            if (enumerableGenericType != null)
                                            {
                                                // 取得集合的值
                                                var propValues = prop.GetValue(x) as System.Collections.IEnumerable;
                                                if (propValues == null)
                                                {
                                                    // 建立空集合
                                                    if (prop.PropertyType.IsArray)
                                                    {
                                                        var propArray = Array.CreateInstance(enumerableGenericType, 0);
                                                        prop.SetValue(x, propArray);
                                                    }
                                                    else
                                                    {
                                                        var propListType = typeof(List<>).MakeGenericType(enumerableGenericType);
                                                        if (prop.PropertyType.IsAssignableFrom(propListType))
                                                        {
                                                            var propList = Activator.CreateInstance(propListType);
                                                            prop.SetValue(x, propList);
                                                        }
                                                    }
                                                }
                                                else if (enumerableGenericType.IsClass)
                                                {
                                                    // 往集合的 class 的屬性去尋找
                                                    foreach (var propValue in propValues)
                                                    {
                                                        setEmptyCollection(propValue);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                };
                                setEmptyCollection.Invoke(regularGeneratorRequest);
                            }
                            #endregion
                        }
                        else
                        {
                            // 如果不是 IGeneratorRequest 則直接使用 UpdateModel 來更新物件
                            request.UpdateModel.Invoke(generatorRequest);
                        }
                        generatorTypeRequests.Add(generatorRequestType, generatorRequest);
                    }
                    var generator = ActivatorUtilities.CreateInstance(_serviceProvider, generatorType);
                    Response.Files files;
                    // 優先判斷是否有 GenerateAsync 
                    var generateAsync = generatorType.GetMethod(nameof(IAsyncGenerator<Request>.GenerateAsync));
                    if (generateAsync != null)
                    {
                        files = await (generateAsync.Invoke(generator, new object[] { generatorRequest }) as Task<Response.Files>);
                    }
                    else
                    {
                        // 沒有 GenerateAsync 才執行 Generate
                        files = generatorType.GetMethod(nameof(IGenerator<Request>.Generate)).Invoke(generator, new object[] { generatorRequest }) as Response.Files;
                    }
                    // 預設的儲存路徑依照 Namespace 決定
                    var defaultDirectoryPath = getPath(generatorType.Namespace);
                    if (files != null)
                    {
                        foreach (var file in files.Where(x => x != null))
                        {
                            if (string.IsNullOrWhiteSpace(file.DirectoryPath))
                            {
                                file.DirectoryPath = defaultDirectoryPath;
                            }
                            else
                            {
                                // 讓路徑有 Format 的作用可以串接預設儲存路徑
                                file.DirectoryPath = string.Format(file.DirectoryPath, defaultDirectoryPath);
                            }
                            // 處理檔案名稱
                            if (string.IsNullOrWhiteSpace(file.Name))
                            {
                                file.Name = "未命名";
                            }
                            file.Name = file.Name.ToFileName();
                            // 將檔案放置目錄中
                            var directory = getDirectory(file.DirectoryPath);
                            directory.Files.Add(file);
                        }
                    }
                }
                return new Response()
                {
                    Provider = request.Provider,
                    Module = request.Module,
                    Container = container
                };
            }
        }
    }
}