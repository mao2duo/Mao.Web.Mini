using Mao.Web.ApiActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ModelBinding.Binders;

namespace Mao.Web.Features
{
    public class GenerateOutputFilesRequestModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(GenerateOutputFiles.Request))
            {
                var binderProvider = new CompositeModelBinderProvider(actionContext.ControllerContext.Configuration.Services.GetModelBinderProviders().Where(x => (x as SimpleModelBinderProvider)?.ModelType != typeof(GenerateOutputFiles.Request)));
                var metadataProvider = actionContext.ControllerContext.Configuration.Services.GetModelMetadataProvider();
                var binder = binderProvider.GetBinder(actionContext.ControllerContext.Configuration, bindingContext.ModelType);
                if (binder.BindModel(actionContext, bindingContext))
                {
                    var request = bindingContext.Model as GenerateOutputFiles.Request;
                    request.UpdateModel = x =>
                    {
                        Type modelType = x.GetType();
                        var modelBinder = binderProvider.GetBinder(actionContext.ControllerContext.Configuration, modelType);
                        var modelBindingContext = new ModelBindingContext
                        {
                            ModelName = "",
                            ValueProvider = bindingContext.ValueProvider,
                            ModelMetadata = metadataProvider.GetMetadataForType(() => x, modelType)
                        };
                        modelBinder.BindModel(actionContext, modelBindingContext);
                    };
                    return true;
                }
            }
            return false;
        }
    }
}