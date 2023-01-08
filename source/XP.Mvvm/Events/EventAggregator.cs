using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XP.Mvvm.Events
{
  public class EventAggregator : IEventAggregator
  {
    private readonly List<Delegate> _delegates = new();
    private readonly ILog _log = LogManager.GetLogger(typeof(EventAggregator));

    public void Subscribe<TEvent>(Func<TEvent, Task> action)
      where TEvent : IEvent
    {
      _log.Debug($"Subscribe {typeof(TEvent)}");
      _delegates.Add(action);
    }

    public void Unsubscribe<TEvent>(Func<TEvent, Task> action)
      where TEvent : IEvent
    {
      _log.Debug($"Unsubscribe {typeof(TEvent)}");
       _delegates.Remove(action);
    }

    public async Task PublishAsync<TEvent>(TEvent @event)
      where TEvent : IEvent
    {
      _log.Debug($"Publish {@event}");
      foreach (var @delegate in _delegates.ToList())
      {
        if (@delegate is Func<TEvent, Task> action)
          await action(@event);
      }
    }
  }
}