using Lift.Core.Common;
using Lift.Core.ImageArray;
using Simscop.Hardware;
using Simscop.Hardware.Camera;
using Simscop.Hardware.Common;

namespace WinFormsApp1
{
    internal class WorkCamera : ICamera
    {
        private AndorModule andorModule = new AndorModule();

        #region IDevice
        string IDevice.Id { get; init; } = "7BE2A269-DCF5-414B-B8C6-81EC2023903C";
        public string? Mode { get; init; } = "USB";
        public string? ModeReserve { get; init; }
        public bool IsConnected { get; set; }=false;
        public bool IsInit { get; set; }=false;
        public bool IsValid { get; set; }=false;
        public Dictionary<string, string>? Dependencies { get; set; }

        public event DeviceEventHandler? OnPreConnect;
        public event DeviceEventHandler? OnConnect;
        public event DeviceEventHandler? OnPreDisconnect;
        public event DeviceEventHandler? OnDisconnect;
        public event DeviceEventHandler? OnPreInit;
        public event DeviceEventHandler? OnInit;
        public event DeviceEventHandler? OnPreDeInit;
        public event DeviceEventHandler? OnDeInit;

        public bool Connect()
        {
            IsConnected = false;
            try
            {
                this.OnPreConnect?.Invoke(this);
                if (!andorModule.Connect())
                {
                    IsConnected = false;
                    return false;
                }
                this.OnConnect?.Invoke(this);
                IsConnected = true;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"The error of connect: {ex}");
                return false;
            }
        }

        public bool DeInit()
        {
            this.OnPreDeInit?.Invoke(this);
            andorModule?.DeInit();
            this.OnDeInit?.Invoke(this);
            IsInit = false;
            return true;
        }

        public bool Disconnect()
        {
            this.OnPreDisconnect?.Invoke(this);
            andorModule?.Disconnect();
            this.OnConnect?.Invoke(this);
            IsConnected = false;
            return true;
        }

        public bool Init()
        {
            IsInit = false;
            try
            {
                this.OnPreInit?.Invoke(this);
                andorModule?.Init();
                this.OnInit?.Invoke(this);
                IsInit = true;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"The error of Init: {ex}");
                return false;
            }
        }

        public bool Valid()
        {
            IsValid = andorModule.Valid();
            return IsValid;
        }

        #endregion

        #region ICamera

        public double Exposure { get; set; } = 0;
        public bool EnableRoi { get; set; } = false;
        public Vector Resolution { get; set; } = new Vector(0, 0, 0);
        public Vector Level { get; set; }=new Vector(0, 0,0);
        public bool EnableDenoise { get; set; } = false;
        public int DenoiseLevel { get; set; } = 0;
        public double Gamma { get; set; } = 1;
        public double Contrast { get; set; } = 1;
        public double Brightness { get; set; } = 0;
        public bool EnableSmooth { get; set; } = false;
        public bool VerticalFlip { get; set; } = false;
        public bool HorizontalFlip { get; set; } = false;
        public double Rotation { get; set; } = 0;
        public object? ImageBucket { get; set; }

        public event DeviceEventHandler? OnPreCapture;
        public event CaptureEventHandler? OnCapture;
        public event DeviceEventHandler? OnPreSave;
        public event DeviceEventHandler? OnSave;
        public event DeviceEventHandler? OnPreRecord;
        public event DeviceEventHandler? OnRecord;

        public ImageArray? Capture()
        {
            this.OnPreCapture?.Invoke(this);
            ImageArray? imageArray= andorModule?.Capture();
            this.OnCapture?.Invoke(this,ref imageArray);
            return imageArray;
        }

        public bool Record(string path, RecordArgs? args)
        {
            bool result=false;
            this.OnPreRecord?.Invoke(this);
            result= andorModule.Record(path, args);
            this.OnRecord?.Invoke(this);
            return result;
        }

        public bool Save(string path, SaveImageArgs? args)
        {
            bool result = false;
            this.OnPreSave?.Invoke(this);
            result = andorModule.Save(path, args);
            this.OnSave?.Invoke(this);
            return result;
        }

        public bool GetBrightness(out double brightness)
        {
            return andorModule.GetBrightness(out brightness);
        }

        public bool GetContrast(out double contrast)
        {
            return andorModule.GetContrast(out contrast);
        }

        public bool GetDenoise(out int level)
        {
            return andorModule.GetDenoise(out level);
        }

        public bool GetExposure()
        {
            return andorModule.GetExposure(out double exposure);
        }

        public bool GetGain(double value, object? args)
        {
            return andorModule.GetGain(value,  args);
        }

        public bool GetGamma(out double gamma)
        {
            return andorModule.GetGamma(out gamma);
        }
        
        public bool GetLevel(out Vector vector)
        {
            return andorModule.GetLevel(out vector);
        }

        public bool GetResolution(out Vector resolution)
        {
            return andorModule.GetResolution(out resolution);
        }

        public bool GetRoi(out RoiArgs args)
        {
            return andorModule.GetRoi(out args);
        }

        public bool GetSmmoth(out SmoothArgs args)
        {
            return andorModule.GetSmmoth(out  args);
        }

        public bool SetBrightness(double brightness)
        {
            return andorModule.SetBrightness(brightness);
        }

        public bool SetContrast(double contrast)
        {
            return andorModule.SetContrast(contrast);
        }

        public bool SetDenoise(int level)
        {
            return andorModule.SetDenoise(level);
        }

        public bool SetExposure(double value)
        {
           return andorModule.SetExposure(value);
        }

        public bool SetGain(double value, object? args)
        {
            throw new NotImplementedException();
        }

        public bool SetGamma(double gamma)
        {
            return andorModule.SetGamma(gamma);
        }

        public bool SetLevel(Vector range)
        {
            return andorModule.SetLevel(range);
        }

        public bool SetResolution(Vector resolution)
        {
            return andorModule.SetResolution(resolution);
        }

        public bool SetRoi(RoiArgs args)
        {
            return andorModule.SetRoi(args);
        }

        public bool SetSmooth(SmoothArgs args)
        {
            return andorModule.SetSmooth(args);
        }

        public bool Get<T>(string name, out T? obj)
        {
            throw new NotImplementedException();
        }

        public bool Set<T>(string name, T? obj)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
