using Lift.Core.Common;
using Lift.Core.ImageArray;
using Simscop.Hardware;
using Simscop.Hardware.Camera;
using Simscop.Hardware.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simscop.Fake.Base;

public abstract class Camera : ICamera
{
    /// <inheritdoc cref="ICamera.Exposure"/>
    public double Exposure { get; set; } = 0;

    /// <inheritdoc cref="ICamera.EnableRoi"/>
    public bool EnableRoi { get; set; } = false;

    /// <inheritdoc cref="ICamera.Resolution"/>
    public Vector Resolution { get; set; } = new Vector(0, 0, 0);

    /// <inheritdoc cref="ICamera.Level"/>
    public Vector Level { get; set; } = new Vector(0, 0, 0);

    /// <inheritdoc cref="ICamera.EnableDenoise"/>
    public bool EnableDenoise { get; set; } = false;

    /// <inheritdoc cref="ICamera.DenoiseLevel"/>
    public int DenoiseLevel { get; set; } = 0;

    /// <inheritdoc cref="ICamera.Gamma"/>
    public double Gamma { get; set; } = 0;

    /// <inheritdoc cref="ICamera.Contrast"/>
    public double Contrast { get; set; } = 0;

    /// <inheritdoc cref="ICamera.Brightness"/>
    public double Brightness { get; set; } = 0;

    /// <inheritdoc cref="ICamera.EnableSmooth"/>
    public bool EnableSmooth { get; set; } = false;

    /// <inheritdoc cref="ICamera.VerticalFlip"/>
    public bool VerticalFlip { get; set; } = false;

    /// <inheritdoc cref="ICamera.HorizontalFlip"/>
    public bool HorizontalFlip { get; set; } = false;

    /// <inheritdoc cref="ICamera.Rotation"/>
    public double Rotation { get; set; } = 0;

    /// <inheritdoc cref="ICamera.ImageBucket"/>
    public object? ImageBucket { get; }=new object();

    /// <inheritdoc cref="IDevice.Id"/>
    public virtual string Id { get; init; } = "9D22EAD1-5797-4DC7-997E-53901E02298E";

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
    public Dictionary<string, string>? Dependencies { get; set; }

    /// <inheritdoc cref="ICamera.OnPreCapture"/>
    public event DeviceEventHandler? OnPreCapture;

    /// <inheritdoc cref="ICamera.OnCapture"/>
    public event CaptureEventHandler? OnCapture;

    /// <inheritdoc cref="ICamera.OnPreSave"/>
    public event DeviceEventHandler? OnPreSave;

    /// <inheritdoc cref="ICamera.OnSave"/>
    public event DeviceEventHandler? OnSave;

    /// <inheritdoc cref="ICamera.OnPreRecord"/>
    public event DeviceEventHandler? OnPreRecord;

    /// <inheritdoc cref="ICamera.OnRecord"/>
    public event DeviceEventHandler? OnRecord;

    /// <inheritdoc cref="IDevice.OnPreConnect"/>
    public event DeviceEventHandler? OnPreConnect;

    /// <inheritdoc cref="IDevice.OnConnect"/>
    public event DeviceEventHandler? OnConnect;

    /// <inheritdoc cref="IDevice.OnPreDisconnect"/>
    public event DeviceEventHandler? OnPreDisconnect;

    /// <inheritdoc cref="IDevice.OnDisconnect"/>
    public event DeviceEventHandler? OnDisconnect;

    /// <inheritdoc cref="IDevice.OnPreInit"/>
    public event DeviceEventHandler? OnPreInit;

    /// <inheritdoc cref="IDevice.OnInit"/>
    public event DeviceEventHandler? OnInit;

    /// <inheritdoc cref="IDevice.OnPreDeInit"/>
    public event DeviceEventHandler? OnPreDeInit;

    /// <inheritdoc cref="IDevice.OnDeInit"/>
    public event DeviceEventHandler? OnDeInit;

    /// <inheritdoc cref="ICamera.Capture"/>
    public ImageArray? Capture()
    {
        ImageArray imageArray=new ImageArray(0,0,0);
        return imageArray;
    }

    /// <inheritdoc cref="IDevice.Connect"/>
    public bool Connect()
    {
        Debug.WriteLine($"{Id}: Func - Connected");
        IsConnected = true;
        return true;
    }

    /// <inheritdoc cref="IDevice.DeInit"/>
    public bool DeInit()
    {
        Debug.WriteLine($"{Id}: Func - DeInit");
        IsInit = false;
        return true;
    }

    /// <inheritdoc cref="IDevice.Disconnect"/>
    public bool Disconnect()
    {
        Debug.WriteLine($"{Id}: Func - Disconnect");
        IsConnected = false;
        return true;
    }

    /// <inheritdoc cref="ICamera.Get{T}(string, out T?)"/>
    public bool Get<T>(string name, out T? obj)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc cref="ICamera.Brightness"/>
    public bool GetBrightness(out double brightness)
    {
        brightness = 0;
        Debug.WriteLine($"{Id}: Func - GetBrightness");
        return true;
    }

    /// <inheritdoc cref="ICamera.GetContrast"/>
    public bool GetContrast(out double contrast)
    {
        contrast = 0;
        Debug.WriteLine($"{Id}: Func - GetContrast");
        return true;
    }

    /// <inheritdoc cref="ICamera.GetDenoise(out int)"/>
    public bool GetDenoise(out int level)
    {
        level = 0;
        Debug.WriteLine($"{Id}: Func - GetDenoise");
        return true;
    }

    /// <inheritdoc cref="ICamera.GetExposure"/>
    public bool GetExposure()
    {
        Debug.WriteLine($"{Id}: Func - GetExposure");
        return true;
    }

    /// <inheritdoc cref="ICamera.GetGain(double, object?)"/>
    public bool GetGain(double value, object? args)
    {
        Debug.WriteLine($"{Id}: Func - GetGain");
        return true;
    }

    /// <inheritdoc cref="ICamera.GetGamma(out double)"/>
    public bool GetGamma(out double gamma)
    {
        gamma = 0;
        Debug.WriteLine($"{Id}: Func - GetGamma");
        return true;
    }

    /// <inheritdoc cref="ICamera.GetLevel(out Vector)"/>
    public bool GetLevel(out Vector vector)
    {
        vector = new Vector(0, 0, 0);
        Debug.WriteLine($"{Id}: Func - GetLevel");
        return true;
    }

    /// <inheritdoc cref="ICamera.GetResolution(out Vector)"/>
    public bool GetResolution(out Vector resolution)
    {
        resolution = new Vector(0, 0, 0);
        Debug.WriteLine($"{Id}: Func - GetResolution");
        return true;
    }

    /// <inheritdoc cref="ICamera.GetRoi(out RoiArgs)"/>
    public bool GetRoi(out RoiArgs args)
    {
        args = new RoiArgs();
        Debug.WriteLine($"{Id}: Func - RoiArgs");
        return true;
    }

    /// <inheritdoc cref="ICamera.GetSmmoth(out SmoothArgs)"/>
    public bool GetSmmoth(out SmoothArgs args)
    {
        args = new SmoothArgs();
        Debug.WriteLine($"{Id}: Func - GetSmmoth");
        return true;
    }

    /// <inheritdoc cref="IDevice.Init"/>
    public bool Init()
    {
        IsInit = true;
        Debug.WriteLine($"{Id}: Func - Init");
        return true;
    }

    /// <inheritdoc cref="ICamera.Record(string, RecordArgs?)"/>
    public bool Record(string path, RecordArgs? args)
    {
        Debug.WriteLine($"{Id}: Func - Record");
        return true;
    }

    /// <inheritdoc cref="ICamera.Save(string, SaveImageArgs?)"/>
    public bool Save(string path, SaveImageArgs? args)
    {
        Debug.WriteLine($"{Id}: Func - Save");
        return true;
    }

    /// <inheritdoc cref="ICamera.Set{T}(string, T?)"/>
    public bool Set<T>(string name, T? obj)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc cref="ICamera.SetBrightness(double)"/>
    public bool SetBrightness(double brightness)
    {
        Debug.WriteLine($"{Id}: Func - SetBrightness");
        return true;
    }
    
    /// <inheritdoc cref="ICamera.SetContrast(double)"/>
    public bool SetContrast(double contrast)
    {
        Debug.WriteLine($"{Id}: Func - SetContrast");
        return true;
    }

    /// <inheritdoc cref="ICamera.SetDenoise(int)"/>
    public bool SetDenoise(int level)
    {
        Debug.WriteLine($"{Id}: Func - SetDenoise");
        return true;
    }

    /// <inheritdoc cref="ICamera.SetExposure(double)"/>
    public bool SetExposure(double value)
    {
        Debug.WriteLine($"{Id}: Func - SetExposure");
        return true;
    }

    /// <inheritdoc cref="ICamera.SetGain(double, object?)"/>
    public bool SetGain(double value, object? args)
    {
        Debug.WriteLine($"{Id}: Func - SetGain");
        return true;
    }

    /// <inheritdoc cref="ICamera.SetGamma(double)"/>
    public bool SetGamma(double gamma)
    {
        Debug.WriteLine($"{Id}: Func - SetGamma");
        return true;
    }

    /// <inheritdoc cref="ICamera.SetLevel(Vector)"/>
    public bool SetLevel(Vector range)
    {
        Debug.WriteLine($"{Id}: Func - SetLevel");
        return true;
    }

    /// <inheritdoc cref="ICamera.SetResolution(Vector)"/>
    public bool SetResolution(Vector resolution)
    {
        Debug.WriteLine($"{Id}: Func - SetResolution");
        return true;
    }

    /// <inheritdoc cref="ICamera.SetRoi(RoiArgs)"/>
    public bool SetRoi(RoiArgs args)
    {
        Debug.WriteLine($"{Id}: Func - SetRoi");
        return true;
    }

    /// <inheritdoc cref="ICamera.SetSmooth(SmoothArgs)"/>
    public bool SetSmooth(SmoothArgs args)
    {
        Debug.WriteLine($"{Id}: Func - SetSmooth");
        return true;
    }

    /// <inheritdoc cref="IDevice.Id=Valid"/>
    public bool Valid()
    {
        IsValid = true;
        Debug.WriteLine($"{Id}: Func - Valid");
        return true;
    }
}
