using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Mao.Web.Features.Attributes
{
    public class PasswordValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string password = value as string;
            string errorMessage = null;
            if (!string.IsNullOrEmpty(password))
            {
                if (!Regex.IsMatch(password, @"[A-Z]")
                    || !Regex.IsMatch(password, @"[a-z]")
                    || !Regex.IsMatch(password, @"[0-9]"))
                {
                    errorMessage = $"{validationContext.DisplayName} 必須包含一個大寫字母、一個小寫字母、一個數字";
                }
            }
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return new ValidationResult(errorMessage, new[] { validationContext.MemberName });
            }
            return ValidationResult.Success;
        }
    }
}