using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BusinessCardAPI.Attributes
{
    public class MaxBase64SizeAttribute : ValidationAttribute
    {
        private readonly int _maxBytes;

        public MaxBase64SizeAttribute(int maxBytes)
        {
            _maxBytes = maxBytes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Photo is optional
            if (value == null)
            {
                
                return ValidationResult.Success;
            }

            if (value is string base64String)
            {
                int byteCount = Encoding.UTF8.GetByteCount(base64String);
                if (byteCount > _maxBytes)
                {
                    return new ValidationResult($"The photo size cannot exceed {_maxBytes / 1024} KB.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
