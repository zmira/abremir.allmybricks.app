using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using System;
using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Implementations
{
    public class DataSynchronizerEventManager : IDataSynchronizerEventManager
    {
        private static readonly Lazy<Dictionary<Type, List<Delegate>>> _lazyEventHandlers = new Lazy<Dictionary<Type, List<Delegate>>>(() => new Dictionary<Type, List<Delegate>>());

        private static Dictionary<Type, List<Delegate>> _eventHandlers => _lazyEventHandlers.Value;

        public void Register<T>(Action<T> eventHandler) where T : IDataSynchronizerEvent
        {
            var type = typeof(T);

            if (!_eventHandlers.ContainsKey(type))
            {
                _eventHandlers.Add(type, new List<Delegate>());
            }

            _eventHandlers[typeof(T)].Add(eventHandler);
        }

        public void Raise<T>(T dataSynchronizerEvent) where T : IDataSynchronizerEvent
        {
            foreach(var handler in _eventHandlers[dataSynchronizerEvent.GetType()])
            {
                var action = (Action<T>)handler;

                action(dataSynchronizerEvent);
            }
        }
    }
}