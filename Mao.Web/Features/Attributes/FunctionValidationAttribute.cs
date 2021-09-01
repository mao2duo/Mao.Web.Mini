using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mao.Web.Features.Attributes
{
    /// <summary>
    /// 以類別中的方法來驗證成員
    /// <para>預設的方法名稱為 Validate + 成員名稱</para>
    /// <para>方法的回傳類型必須是 ValidationResult</para>
    /// </summary>
    public class FunctionValidationAttribute : ValidationAttribute
    {
        public string FunctionName { get; }

        public FunctionValidationAttribute()
        {
        }
        public FunctionValidationAttribute(string functionName)
        {
            FunctionName = functionName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string functionName = FunctionName;
            if (string.IsNullOrEmpty(functionName))
            {
                functionName = $"Validate{validationContext.MemberName}";
            }
            var function = validationContext.ObjectType.GetMethod(functionName);
            if (function == null)
            {
                throw new Exception($"[FunctionValidation] 在類型 {validationContext.ObjectType.FullName} 中無法找到方法 {functionName}");
            }
            if (!typeof(ValidationResult).IsAssignableFrom(function.ReturnType))
            {
                throw new Exception($"[FunctionValidation] 類型 {validationContext.ObjectType.FullName} 方法 {functionName} 的回傳類型必須是 {nameof(ValidationResult)}");
            }
            var model = validationContext.ObjectInstance;
            var memberProperty = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            var methodParameters = function.GetParameters();
            var invokeParameters = new object[methodParameters.Length];
            for (int i = 0; i < methodParameters.Length; i++)
            {
                var methodParameter = methodParameters[i];
                if (methodParameter.ParameterType == memberProperty.PropertyType)
                {
                    invokeParameters[i] = value;
                    continue;
                }
                if (methodParameter.ParameterType == typeof(ValidationContext))
                {
                    invokeParameters[i] = validationContext;
                    continue;
                }
                if (methodParameter.HasDefaultValue)
                {
                    invokeParameters[i] = methodParameter.DefaultValue;
                    continue;
                }
            }
            return function.Invoke(model, invokeParameters) as ValidationResult;
        }
    }
}