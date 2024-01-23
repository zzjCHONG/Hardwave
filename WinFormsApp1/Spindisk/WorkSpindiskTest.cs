using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simscop.Hardware.SpinDisk;
using Simscop.Spindisk.Crest;
using Simscop.Hardware.Common;
using System.Diagnostics;
using Lift.Core.Exception;
using Simscop.Hardware;


namespace WinFormsApp1
{
    public class WorkSpindiskTest : ISpindisk
    {
        /// <inheritdoc />
        public int Excitation { get; internal set; }

        /// <inheritdoc />
        public int Emission { get; internal set; }

        /// <inheritdoc />
        public int Dichroic { get; internal set; }

        /// <inheritdoc />
        public int Spinning { get; internal set; }

        /// <inheritdoc />
        public MotorState State { get; internal set; }

        /// <inheritdoc />
        public string Id { get; internal set; } = "322D603E-E9BC-49B6-94B7-256A3BDEC3C0";

        /// <inheritdoc />
        public bool IsConnected { get; internal set; } = false;

        /// <inheritdoc />
        public bool IsInit { get; internal set; } = false;

        /// <inheritdoc />
        public bool IsValid { get; internal set; } = false;

        public Dictionary<string, string>? Dependencies { get; } = null;

        public event DeviceEventHandler? OnPreConnect;
        public event DeviceEventHandler? OnConnect;
        public event DeviceEventHandler? OnPreDisconnect;
        public event DeviceEventHandler? OnDisconnect;
        public event DeviceEventHandler? OnPreInit;
        public event DeviceEventHandler? OnInit;
        public event DeviceEventHandler? OnPreDeInit;
        public event DeviceEventHandler? OnDeInit;

        public bool AsyncUpdate()
        {
            if (!WriteAndValid(CommandConstant.Query))
                return false;

            var cache = _port!.Cache;
            if (cache is null) return false;

            var nums = (from c in cache where char.IsDigit(c) select int.Parse(c.ToString())).ToArray();

            Emission = nums[0];
            Dichroic = nums[1];
            Spinning = nums[2];
            State = nums[3] switch
            {
                0 => MotorState.Off,
                1 => MotorState.On,
                _ => throw new InvalidException("The number must be 0 or 1.")
            };
            Excitation = nums[4];

            return true;
        }

        public bool Connect()
        {
            IsConnected = false;
            try
            {
                OnPreConnect?.Invoke(this);
                _port?.Open();
                OnConnect?.Invoke(this);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"The error of connect: {e}");
                return false;
            }

            IsConnected = true;
            return true;
        }

        public bool DeInit() => true;

        public bool Disconnect()
        {
            OnPreDisconnect?.Invoke(this);
            _port?.Close();
            OnDisconnect?.Invoke(this);

            IsConnected = false;
            return true;
        }

        public int Get(string wheel)
        {
            if (_port == null)
                throw new Exception("The port is not connected.");

            var command = GetCommand(wheel);

            _port.Write($"r{command}\r");
            Thread.Sleep(100);

            var cache = _port!.Cache;
            return cache is null
                ? throw new Exception("The cache is null.")
                : (from c in cache where char.IsDigit(c) select int.Parse(c.ToString())).ToArray()[0];
        }

        string? GetCommand(string wheel) => wheel switch
        {
            nameof(CommandConstant.Excitation) => CommandConstant.Excitation,
            nameof(CommandConstant.Emission) => CommandConstant.Emission,
            nameof(CommandConstant.Dichroic) => CommandConstant.Dichroic,
            nameof(CommandConstant.SpinningPosition) => CommandConstant.SpinningPosition,
            nameof(CommandConstant.SpinningMotor) => CommandConstant.SpinningMotor,
            _ => null
        };

        public T? Get<T>(string prop)
        {
            if (typeof(T) == typeof(string))
            {
                return prop switch
                {
                    "ComName" => (T?)(object?)ComName,
                    "Version" => (T?)(object)FirewareVersion,
                    _ => (T?)(object?)null
                };
            }

            throw new Lift.Core.Exception.NotSupportedException("The property is not supported.");
        }

        public bool Home()
              // ReSharper disable once ConditionalTernaryEqualBranch
              => WriteAndValid($"{CommandConstant.Home}", 50);

        public bool Init() => true;

        public bool Off()
            => WriteAndValid($"{CommandConstant.SpinningMotor}0", 50);

        public bool On()
            => WriteAndValid($"{CommandConstant.SpinningMotor}1", 50);

        public bool Set(string wheel, int value)
        {
            if (_port == null)
                throw new Exception("The port is not connected.");

            var command = GetCommand(wheel);

            var valid = IsValidWheel(wheel, value);

            if (!valid)
                throw new InvalidException("The input value invalid.");

            return WriteAndValid($"{command}{Math.Abs(value)}{(value < 0 ? "m" : "")}");
        }

        bool IsValidWheel(string wheel, int value) => wheel switch
        {
            nameof(CommandConstant.Excitation) => CommandConstant.IsVaidExcitation(value),
            nameof(CommandConstant.Emission) => CommandConstant.IsVaidEmission(value),
            nameof(CommandConstant.Dichroic) => CommandConstant.IsVaidDichroic(value),
            nameof(CommandConstant.SpinningPosition) => CommandConstant.IsVaidSpinningPosition(value),
            nameof(CommandConstant.SpinningMotor) => CommandConstant.IsVaidSpinningMotor(value),
            _ => throw new Exception()
        };

        public bool Set<T>(string prop, T value)
        {
            throw new Lift.Core.Exception.NotSupportedException("The property is not supported.");
        }

        public string? ComName { get; set; } = null;
        string IDevice.Id { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public string? Mode { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public string? ModeReserve { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }

        public const string FirewareVersion = "Crest driver Ver  6.0.1";
        //
        // 摘要:
        //     Read the firmware version. Examples
        //
        //     r<CR> => Ver. 6.0.0<CR>
        public static readonly string Version = "v";
        private SimplePort? _port = null;

        public bool Valid()
        {
            ComName = ComHelper.Serach($"{Version}\r", FirewareVersion, 1000, true);
            if (ComName != null)
            {
                Debug.WriteLine($"The valid pass and the com is {ComName}");
                IsValid = true;

                _port = new SimplePort(ComName);
            }
            else Debug.WriteLine("The ComName not found.");

            return ComName != null;
        }

        bool WriteAndValid(string command ,int count = 100)
        {
            if (_port == null)
                throw new Exception("The port is not connected.");
            _port.Write($"{command}\r");
            var response = _port.Read(count);
            return response?.Replace("\n", "").Replace("\r", "").Contains(command.Replace("m", "")) ?? false;

        }

    }
}
