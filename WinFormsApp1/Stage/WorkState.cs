using Simscop.Hardware;
using Simscop.Hardware.Common;
using Simscop.Hardware.Stage;
using System.Timers;

namespace WinFormsApp1
{
    public class WorkState : IStage
    {
        private MshotModule mshotModule = new MshotModule();

        #region IDevice
        string IDevice.Id { get; init; }= "23E30972-2D37-45AD-8134-AF66A24473C2";
        public string? Mode { get; init; } =  "USB";
        public string? ModeReserve { get; init; } = "COM?";
        public bool IsConnected { get;  set; } = false;
        public bool IsInit { get;  set; } = false;
        public bool IsValid { get;  set; } = false;
        public Dictionary<string, string>? Dependencies { get; set; }

        public event DeviceEventHandler? OnPreConnect;
        public event DeviceEventHandler? OnConnect;
        public bool Connect()
        {
            IsConnected = false;
            try
            {
                this.OnPreConnect?.Invoke(this);
                mshotModule?.OpenComQK(true);
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

        public event DeviceEventHandler? OnPreDisconnect;
        public event DeviceEventHandler? OnDisconnect;
        public bool Disconnect()
        {
            this.OnPreDisconnect?.Invoke(this);
            mshotModule?.CloseCom();
            this.OnConnect?.Invoke(this);
            IsConnected = false;
            return true;
        }

        public event DeviceEventHandler? OnPreDeInit;
        public event DeviceEventHandler? OnDeInit;
        public bool DeInit()
        {
            this.OnPreDeInit?.Invoke(this);
            mshotModule?.DeInit();
            this.OnDeInit?.Invoke(this);
            IsInit = false;
            return true;
        }

        public event DeviceEventHandler? OnPreInit;
        public event DeviceEventHandler? OnInit;
        public bool Init()
        {
            IsInit = false;
            try
            {
                this.OnPreInit?.Invoke(this);
                mshotModule?.Init();
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
            IsValid = mshotModule.Valid();
            return IsValid;
        }

        #endregion

        #region IStage

        //轴属性改变事件，暂无绑定
        public event EventHandler? AxisChanged;
        //public List<StageAxis> Axes { get; set; }
        private List<StageAxis> axes = new List<StageAxis>();
        public List<StageAxis> Axes
        {
            get { return axes; }
            set
            {
                if (value != null)
                {
                    axes = value;
                    OnAxesChanged(this, EventArgs.Empty);
                }
            }
        }
        private void OnAxesChanged(object? sender, EventArgs args)
        {
            AxisChanged?.Invoke(sender, args);
        }

        //轴数据自动更新
        public event Action AutoRefreshAxis;
        private void RefreshAxis()
        {
            foreach (var axis in Axes)//TODO,只刷新第一个输入的轴的数据，bug
            {
                switch (axis.Alias)
                {
                    case "X":
                        axis.Position = mshotModule.XPosition;
                        break;
                    case "Y":
                        axis.Position = mshotModule.YPosition;
                        break;
                    case "Z":
                        axis.Position = mshotModule.ZPosition;
                        break;
                    default:
                        Console.WriteLine("This axis is not Exist!");
                        break;
                }
            }
        }
        System.Timers.Timer timer;
        private double AutoRefreshTime = 100;
        private void OnConnectEvent(object sender)
        {
            timer.Start();
        }
        private void OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            if (AutoRefreshAxis != null)
            {
                AutoRefreshAxis.Invoke();
            }
        }
        
        public WorkState()
        {
            Axes = new List<StageAxis>
            {
                new StageAxis() { Id = MshotModule.XAxes, Alias = "X" },
                new StageAxis() { Id = MshotModule.ZAxes, Alias = "Z" },
                new StageAxis() { Id = MshotModule.YAxes, Alias = "Y" },             
            };

            AutoRefreshAxis += () => { RefreshAxis(); };//AutoRefreshAxis绑定RefreshAxis自动刷新

            OnConnect += OnConnectEvent;//OnConnect触发timer执行AutoRefreshAxis
            timer = new System.Timers.Timer()
            {
                Interval = AutoRefreshTime,
                AutoReset = true,
            };
            timer.Elapsed += OnTimedEvent;
        }

        public bool Reset()
        {
            bool isResetSuccessful=false;
            foreach (var axis in Axes)
            {
                isResetSuccessful&= SetAbsolutePosition(axis, 0);
                Thread.Sleep(100);
            }
            return isResetSuccessful;
        }

        public bool SetCenter()
        {
            return mshotModule.SetHome();
        }

        public bool SetAbsolutePosition(StageAxis axis, double absPosition)
        {
            return mshotModule.MoveAbsolute(axis, absPosition); 
        }   

        public bool SetRelativePosition(StageAxis axis, double relPosition)
        {
            return mshotModule.MoveRelative(axis, relPosition);
        }

        public bool SetAcceleration(StageAxis axis, int type, double value)
        {
            throw new NotImplementedException();
        }
        public bool Set<T>(string name, T? obj)
        {
            throw new NotImplementedException();
        }
        public bool Get<T>(string name, out T? value)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
