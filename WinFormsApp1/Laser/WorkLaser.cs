using Simscop.Hardware;
using Simscop.Hardware.Common;
using Simscop.Hardware.Laser;

namespace WinFormsApp1
{
    internal class WorkLaser : ILaser
    {
        LaserModule laserModule = new LaserModule();

        /// <summary>
        /// 可用串口号
        /// </summary>
        public string? PortName { get; set; }

        #region IDevice
        public string? ModeReserve { get; init; }
        string IDevice.Id { get; init; } = "B7C320BB-6581-4495-BAE9-BD4E0DD6A98A";
        public string? Mode { get; init; } = "USB";
        public bool IsConnected { get; internal set; } = false;
        public bool IsInit { get; internal set; } = false;
        public bool IsValid { get; internal set; } = false;
        public Dictionary<string, string>? Dependencies { get; set; }//校验ID用，dll相对路径+MD5+GUID，
        
        public bool Connect()
        {
            IsConnected = false;
            try
            {
                this.OnPreConnect?.Invoke(this);
                if (!laserModule.OpenCom())
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
        public event DeviceEventHandler? OnPreConnect;
        public event DeviceEventHandler? OnConnect;

        public bool Disconnect()
        {
            this.OnPreDisconnect?.Invoke(this);
            laserModule?.CloseCom();
            this.OnConnect?.Invoke(this);
            IsConnected = false;
            return true;
        }
        public event DeviceEventHandler? OnPreDisconnect;
        public event DeviceEventHandler? OnDisconnect;

        public bool Init()
        {
            IsInit = false;
            try
            {
                this.OnPreInit?.Invoke(this);
                laserModule?.Init();
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
        public event DeviceEventHandler? OnPreInit;
        public event DeviceEventHandler? OnInit;

        public bool DeInit()
        {
            this.OnPreDeInit?.Invoke(this);
            laserModule?.DeInit();
            this.OnDeInit?.Invoke(this);
            IsInit = false;
            return true;
        }
        public event DeviceEventHandler? OnPreDeInit;
        public event DeviceEventHandler? OnDeInit;
               
        public bool Valid()
        {
            IsValid = laserModule.Valid();
            if (IsValid)
            {
                //输出已验证串口端号
                PortName = laserModule._portName;
            }       
            return IsValid;
        }

        #endregion

        #region ILaser

        public event EventHandler? ChannelChanged;
        private List<Channel> _channels = new List<Channel>() { new Channel(), new Channel(), new Channel(), new Channel(), };
        public List<Channel> Channels
        {
            get { return _channels; }    
            set
            {
                if (value != null)
                {
                    _channels = value;
                    OnChannelChanged();
                }
            }
        }
        private void OnChannelChanged()
        {
            ChannelChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 设备型号
        /// </summary>
        public string Model { get; set; } = "DACx8";
    
        /// <summary>
        /// 同步状态
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool AsyncStatus()
        {
            return laserModule.AsyncLaserStatus(Channels);
        }

        /// <summary>
        /// 设置能量
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool SetPower(Channel channel, double power)
        {
            return laserModule.SetPower(channel, power);
        }

        /// <summary>
        /// 使能
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Enable(Channel channel)
        {
            return laserModule.Enable(channel);
        }

        /// <summary>
        /// 关闭激光
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool LightOff(Channel channel)
        {
            return laserModule.LightOff(channel);
        }

        /// <summary>
        /// 打开激光
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool LightOn(Channel channel)
        {
            return laserModule.LightOn(channel);
        }

        /// <summary>
        /// 获取某一个属性的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public T? Get<T>(Channel channel, string properties)
        {
            if (typeof(T) == typeof(string))
            {
                return properties switch
                {
                    "ComName" => (T?)(object?)PortName,
                    _ => (T?)(object?)null
                };
            }

            throw new NotSupportedException("The property is not supported.");
        }

        /// <summary>
        /// 设置某一个属性的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="properties"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Set<T>(Channel channel, string properties, T param)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
