using Lab.ShellModule.Models;
using Prism.Events;

namespace Lab.ShellModule.Events
{
    public class CameraSelectedChangedEvent: PubSubEvent<CameraSelectedChangedEventModel>
    {
    }
}
