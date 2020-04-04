using System;

namespace Lab.Common.Utilities
{
    public static class ExceptionUtilities
    {
        public static string GetNestedExceptionText(Exception exception)
        {
            string str = exception.Message;
            int num = 0;
            for (Exception innerException = exception.InnerException; innerException != null; innerException = innerException.InnerException)
            {
                str = str + " (" + innerException.Message;
                ++num;
            }
            while (--num >= 0)
                str += ")";
            return str;
        }
    }
}
