using Lab.Common.Enums;
using Prism.Events;

namespace Lab.Common.EventMessages
{
    public class CommandSentEvent : PubSubEvent<EventParams<CommandTypes>>
    {
    }
}
