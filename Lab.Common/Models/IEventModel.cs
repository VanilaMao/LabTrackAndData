using System;

namespace Lab.Common.Models
{
    public interface IEventModel
    {
        Type ModelType { get;}
        object Data { get;}
    }
}
