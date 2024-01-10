namespace LXGaming.Extractorr.Server.Services.Event;

public delegate Task AsyncEventHandler<in TEventArgs>(object? sender, TEventArgs eventArgs);