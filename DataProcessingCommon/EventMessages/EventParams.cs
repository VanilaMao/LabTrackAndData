using Lab.Common.Enums;
using Lab.Common.Models;

namespace Lab.Common.EventMessages
{
    public class EventParams<T>
    {
        public EventParams(T type, IEventModel eventModel)
        {
            EventModel = eventModel;
            Type = type;
        }

        public T Type { get; }
        public Status Status { get; set; }

        //real params passed to event subscribes handlers
        public IEventModel EventModel { get; }
        public string Message { get; set; }
    }
}
