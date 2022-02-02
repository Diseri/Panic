using AASA.NetCore.Lib.Common;
using AASA.NetCore.Lib.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AASA.NetCore.IDomainService.Panic.Models
{
    public class TowTypeAttribute : ValidationAttribute
    {
        private List<string> _allowedValues = GeneralManager.AllTowTypes.Select(x => x.label).ToList();
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var towtype = value as string;
            if (!string.IsNullOrEmpty(towtype))
            {
                if (_allowedValues.Contains(towtype))
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult($"{ towtype } is not a valid tow type. Please use one of the following: {string.Join(',', _allowedValues)} or leave it empty.");
            }
            return ValidationResult.Success;
        }
    }

    public class IsAccessibleAttribute : ValidationAttribute
    {
        private List<string> _allowedValues = GeneralManager.AllIsAccessible.Select(x => x.label).ToList();
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var val = value as string;
            if (!string.IsNullOrEmpty(val))
            {
                if (_allowedValues.Contains(val))
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult($"{ val } is not valid for is accessible. Please use one of the following: {string.Join(',', _allowedValues)} or leave it empty.");
            }
            return ValidationResult.Success;
        }
    }


    public class NoOfPassengersAttribute : ValidationAttribute
    {
        private List<string> _allowedValues = GeneralManager.AllNumberOfPassengers.Select(x => x.label).ToList();
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var val = value as string;
            if (!string.IsNullOrEmpty(val))
            {
                if (_allowedValues.Contains(val))
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult($"{ val } is not valid for no of passengers. Please use one of the following: {string.Join(',', _allowedValues)} or leave it empty.");
            }
            return ValidationResult.Success;
        }
    }


    public class ClientServiceTypeAttribute : ValidationAttribute
    {
        private List<string> _allowedValues = GeneralManager.AllWorkOrderTypeConfigurations.Where(f => !string.IsNullOrEmpty(f.aasa_ClientServiceType)).Select(x => x.aasa_ClientServiceType.ToLower()).ToList();
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var val = (value as string)?.ToLower();
            if (!string.IsNullOrEmpty(val))
            {
                if (_allowedValues.Contains(val))
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult($"{ val } is not valid for client service type. Please specify a valid value or leave it empty.");
            }
            return ValidationResult.Success;
        }
    }

    public class CaseTypeAttribute : ValidationAttribute
    {
        private List<string> _allowedValues = GeneralManager.AllCaseTypes.Select(x => x.aasa_name.ToLower()).ToList();
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var val = (value as string)?.ToLower();
            if (!string.IsNullOrEmpty(val))
            {
                if (_allowedValues.Contains(val))
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult($"{ val } is not valid for case type. Please use one of the following: {string.Join(',', _allowedValues)} or leave it empty.");
            }
            return ValidationResult.Success;
        }
    }

    public class LatLngAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var val = (value as string)?.ToLower();
            if (!string.IsNullOrEmpty(val))
            {
                if (double.TryParse(val, out double _))
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult($"{ val } is not valid. Please pass a valid latitude and longitude.");
            }
            return ValidationResult.Success;
        }
    }

    public class ContactNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var val = (value as string)?.ToLower();
            if (!string.IsNullOrEmpty(val))
            {
                Regex phone = new Regex(@"^((?:\+27|27)|0)(\d{2})?(\d{3})?(\d{4})$");
                if (phone.IsMatch(val))
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult($"{ val } is not a valid contact number.");
            }
            return ValidationResult.Success;
        }
    }


    public class EmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var val = (value as string)?.ToLower();
            if (!string.IsNullOrEmpty(val))
            {
                if (val.IsValidEmail())
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult($"{ val } is not a valid email.");
            }
            return ValidationResult.Success;
        }
    }

    public class SchemeIdAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var val = (value as string)?.ToLower();
            if (!string.IsNullOrEmpty(val))
            {
                if (Guid.TryParse(val, out Guid _))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    if (val.ToLower().Contains("<string>"))
                    { return new ValidationResult($"{ val } is not a valid Guid for Scheme Id."); }
                }
            }
            return ValidationResult.Success;
        }
    }

    public class GenericStringValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var val = (value as string)?.ToLower();
            if (!string.IsNullOrEmpty(val))
            {
                if (val.ToLower().Contains("<string>") 
                    || val.ToLower().Contains("null"))
                {
                    return new ValidationResult($"{ val } is not a valid assignment.");
                }
            }
            return ValidationResult.Success;
        }
    }

}
