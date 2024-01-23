using Simscop.Hardware.Common;
using Simscop.Hardware.Stage;
using Simscop.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Diagnostics;

namespace Simscop.Fake.Base;

public abstract class State : IStage
{
    /// <inheritdoc cref="IDevice.Id"/>
    public virtual string Id { get; init; } = "23E30972-2D37-45AD-8134-AF66A24473C2";

    /// <inheritdoc cref="IDevice.Mode"/>
    public string? Mode { get; init; } = "Usb";

    /// <inheritdoc cref="IDevice.ModeReserve"/>
    public string? ModeReserve { get; init; } = "COM?";

    /// <inheritdoc cref="IDevice.IsConnected"/>
    public bool IsConnected { get; set; } = false;

    /// <inheritdoc cref="IDevice.IsInit"/>
    public bool IsInit { get; set; } = false;

    /// <inheritdoc cref="IDevice.IsValid"/>
    public bool IsValid { get; set; } = false;

    /// <inheritdoc cref="IDevice.Dependencies"/>
    public Dictionary<string, string>? Dependencies { get;}

    /// <inheritdoc cref="IDevice.OnPreConnect"/>
    public event DeviceEventHandler? OnPreConnect;

    /// <inheritdoc cref="IDevice.OnConnect"/>
    public event DeviceEventHandler? OnConnect;

    /// <inheritdoc cref="IDevice.Connect"/>
    public bool Connect()
    {
        Debug.WriteLine($"{Id}: Func - Connected");
        IsConnected = true;
        return true;
    }

    /// <inheritdoc cref="IDevice.OnPreDisconnect"/>
    public event DeviceEventHandler? OnPreDisconnect;

    /// <inheritdoc cref="IDevice.OnDisconnect"/>
    public event DeviceEventHandler? OnDisconnect;

    /// <inheritdoc cref="IDevice.Disconnect"/>
    public bool Disconnect()
    {
        Debug.WriteLine($"{Id}: Func - Disconnect");
        IsConnected = false;
        return true;
    }

    /// <inheritdoc cref="IDevice.OnPreDeInit"/>
    public event DeviceEventHandler? OnPreDeInit;

    /// <inheritdoc cref="IDevice.OnDeInit"/>
    public event DeviceEventHandler? OnDeInit;

    /// <inheritdoc cref="IDevice.DeInit"/>
    public bool DeInit()
    {
        Debug.WriteLine($"{Id}: Func - Init");
        IsInit = false;
        return true;
    }

    /// <inheritdoc cref="IDevice.OnPreInit"/>
    public event DeviceEventHandler? OnPreInit;

    /// <inheritdoc cref="IDevice.OnInit"/>
    public event DeviceEventHandler? OnInit;

    /// <inheritdoc cref="IDevice.Init"/>
    public bool Init()
    {
        Debug.WriteLine($"{Id}: Func - Init");
        IsInit = true;
        return true;
    }

    /// <inheritdoc cref="IDevice.Valid"/>
    public bool Valid()
    {
        Debug.WriteLine($"{Id}: Func - Valid");
        IsValid = true;
        return true;
    }

    /// <inheritdoc cref="IStage.Axes"/>
    public List<StageAxis> Axes { get; } = new List<StageAxis>();

    /// <inheritdoc cref="IStage.AutoRefreshAxis"/>
    public event Action? AutoRefreshAxis;

    /// <inheritdoc cref="IStage.AxisChanged"/>
    public event EventHandler? AxisChanged;

    /// <inheritdoc cref="IStage.SetRelativePosition(StageAxis, double)"/>
    public bool SetRelativePosition(StageAxis axis, double value)
    {
        Debug.WriteLine($"{Id}: Func - SetRelativePosition");
        return true;
    }

    /// <inheritdoc cref="IStage.SetAbsolutePosition(StageAxis, double)"/>
    public bool SetAbsolutePosition(StageAxis axis, double value)
    {
        Debug.WriteLine($"{Id}: Func - SetAbsolutePosition");
        return true;
    }

    /// <inheritdoc cref="IStage.Reset"/>
    public bool Reset()
    {
        Debug.WriteLine($"{Id}: Func - Reset");
        return true;
    }

    /// <inheritdoc cref="IStage.SetCenter"/>
    public bool SetCenter()
    {
        Debug.WriteLine($"{Id}: Func - SetCenter");
        return true;
    }

    /// <inheritdoc cref="IStage.SetAcceleration(StageAxis, int, double)"/>
    public bool SetAcceleration(StageAxis axis, int type, double value)
    {
        Debug.WriteLine($"{Id}: Func - SetAcceleration");
        return true;
    }

    /// <inheritdoc cref="IStage.Get{T}(string, out T?)"/>
    public bool Get<T>(string prop, out T? value)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc cref="IStage.Set{T}(string, T?)"/>
    public bool Set<T>(string prop, T? obj)
    {
        throw new NotImplementedException();
    }

}


