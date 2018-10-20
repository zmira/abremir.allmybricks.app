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

            _eventHandlers[type].Add(eventHandler);
        }

        public void Raise<T>(T dataSynchronizerEvent) where T : IDataSynchronizerEvent
        {
            var type = dataSynchronizerEvent.GetType();

            if (!_eventHandlers.ContainsKey(type))
            {
                return;
            }

            foreach (var handler in _eventHandlers[type])
            {
                var action = (Action<T>)handler;

                action(dataSynchronizerEvent);
            }
        }
    }
}