
using System;

namespace Lab.Common.Foundations
{
    public interface IPlatformHelper
    {
        /// <summary>
        /// Raises when the system detects that commands should be requeried.
        /// </summary>
        event EventHandler RequerySuggested;


    }
}
