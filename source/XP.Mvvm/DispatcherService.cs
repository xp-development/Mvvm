﻿using System;
using System.Threading.Tasks;

namespace XP.Mvvm;

public class DispatcherService : IDispatcherService
{
    public Task BeginInvoke(Action action)
    {
        return Dispatcher.CurrentDispatcher.BeginInvoke(action);
    }
}