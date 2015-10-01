using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perspex.Platform
{
    /// <summary>
    /// Loads assets compiled into the application binary.
    /// </summary>
    public interface IAssetLoader
    {
        /// <summary>
        /// Opens the resource with the requested URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>A stream containing the resource contents.</returns>
        /// <exception cref="FileNotFoundException">
        /// The resource was not found.
        /// </exception>
        Stream Open(Uri uri);
    }
}
