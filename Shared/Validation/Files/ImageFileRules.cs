

namespace AMZN.Shared.Validation.Files
{

    public static class ImageFileRules
    {
        // разрешенные типы (из заголовков запроса)
        private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

        // разрешенные расширения (из имени файла)
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };


        public static bool TryValidate(IFormFile file, long maxBytes, out string error)
        {
            error = string.Empty;

            if (file == null)
            {
                error = "File is missing";
                return false;
            }

            if (file.Length <= 0)
            {
                error = "File is empty";
                return false;
            }

            if (file.Length > maxBytes)
            {
                error = $"File is too large. Max {maxBytes} bytes";
                return false;
            }

            // расширение файла
            var ext = Path.GetExtension(file.FileName);

            if (string.IsNullOrWhiteSpace(ext) || !AllowedExtensions.Contains(ext))
            {
                error = "Unsupported file extension";
                return false;
            }

            if (string.IsNullOrWhiteSpace(file.ContentType) || !AllowedContentTypes.Contains(file.ContentType))
            {
                error = "Unsupported file content type";
                return false;
            }

            return true;
        }
    }
}