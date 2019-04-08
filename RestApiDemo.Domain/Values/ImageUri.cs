using System;
using System.Collections.Generic;
using System.IO;

namespace RestApiDemo.Domain.Values
{
    public class ImageUri
    {
        private static HashSet<string> imageFileExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif" };

        public Uri Uri { get; }

        public ImageUri(string uri)
        {
            if (null == uri)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            Uri parsedUri;
            try
            {
                parsedUri = new Uri(uri);
            }
            catch(Exception ex)
            {
                throw new ArgumentException($"The value \"{uri}\" is not a valid Uri.", nameof(uri), ex);
            }

            var fileExtension = Path.GetExtension(parsedUri.LocalPath);
            if (!imageFileExtensions.Contains(fileExtension))
            {
                throw new ArgumentException($"Uri \"{uri}\" is not regonized as a link to a file.", nameof(uri));
            }

            Uri = parsedUri;
        }
    }
}
