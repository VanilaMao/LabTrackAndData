
using System;

namespace Lab.Common.Models
{
    public class EventModel<T> : IEventModel
    {
        public EventModel(object data)
        {
            ModelType = typeof(T);
            Data = data;
        }
        public Type ModelType { get;}
        public object Data { get; }
    }
}
