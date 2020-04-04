using System;

namespace Lab.Common.Logging
{
    public interface ILogFactory
    {
        ILogger GetLogger(Type type);
    }
}
