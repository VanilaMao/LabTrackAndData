using System;
using System.Diagnostics;
using System.Windows;
using Lab.Common.ApplicationHelpers;
using Lab.Common.Logging;
using Lab.Common.Utilities;

namespace Lab.Application.ExceptionHandlers
{
    internal class ExceptionHandler
    {
        private readonly ILogService _logService;
        private bool _recursionGuard;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandler"/> class.
        /// </summary>
        /// <param name="logService">The log service.</param>
        public ExceptionHandler(ILogService logService)
        {
            _logService = logService;
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="isTerminating">if set to <c>true</c> [is terminating].</param>
        public void HandleException(Exception e, bool isTerminating)
        {
            // if there is no exception, do nothing.
            if (e == null)
            {
                return;
            }

            // ensure this method is not re-entered due to another exception.
            if (_recursionGuard)
            {
                return;
            }
            _recursionGuard = true;

            try
            {
                // show the error to the user
                string exceptionText = ExceptionUtilities.GetNestedExceptionText(e);
#if DEBUG
                string message = string.Format("{0} has encountered the internal error described below. Please report this error to {1}.{2}{2}{3}{2}{2}Stack trace:{2}{4}",
                    ApplicationInfo.ProductName, ApplicationInfo.Company, Environment.NewLine, exceptionText, e.StackTrace);
#else
                string message = string.Format("{0} has encountered the internal error described below. Please report this error to {1}.{2}{2}{3}",
                    ApplicationInfo.ProductName, ApplicationInfo.Company, Environment.NewLine, exceptionText);
#endif
                // trace the error and log
                Trace.TraceError(e.ToString());
                if (!isTerminating)
                {
                    _logService.Error(e);
                }
                else
                {
                    _logService.Fatal(e);
                }

                MessageBox.Show(message, $"Internal {ApplicationInfo.ProductName} Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                // Resharper disabled because it needs to be failsafe
            }

            _recursionGuard = false;
        }
    }
}
