using System;

namespace abremir.AllMyBricks.DataSynchronizer.Interfaces
{
    public interface IDataSynchronizerEventManager
    {
        void Register<T>(Action<T> eventHandler) where T : IDataSynchronizerEvent;
        void Raise<T>(T dataSynchronizerEvent) where T : IDataSynchronizerEvent;
    }
}