using System;

namespace EventHandlerGeneric
{
    internal class KeyboardControl : ISubdeviceControl<StateNotificationEventArgs>
    {
        public void ExecuteCommand()
        {
            
        }

        public event EventHandler<StateNotificationEventArgs> StateChangeEvent;
    }
}