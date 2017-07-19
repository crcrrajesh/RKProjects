using System;

namespace EventHandlerGeneric
{
    internal interface ISubdeviceControl<T> where T:EventArgs
    {
        void ExecuteCommand();
        event EventHandler<T> StateChangeEvent;
    }
}