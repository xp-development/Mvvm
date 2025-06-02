using System;
using System.Threading.Tasks;

namespace XP.Mvvm.Events;

public interface IEventAggregator
{
  void Subscribe<TEvent>(Func<TEvent, Task> action)
  where TEvent : IEvent;

  void Unsubscribe<TEvent>(Func<TEvent, Task> onEditLogReceiverRequestEvent)
  where TEvent : IEvent;

  Task PublishAsync<TEvent>(TEvent @event)
  where TEvent : IEvent;
}