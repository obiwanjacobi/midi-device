using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TestApp;

internal abstract class ViewModel : ObservableObject
{
    // designer support only!
    protected ViewModel()
    {
        Parent = null;
        Services = null;
    }

    protected ViewModel(ViewModel parent)
    {
        Services = parent.Services;
        Parent = parent;
    }

    protected ViewModel(IServiceProvider services)
    {
        Services = services;
        Parent = null;
    }

    public ViewModel? Parent { get; }

    public IServiceProvider Services { get; protected set; }
}
