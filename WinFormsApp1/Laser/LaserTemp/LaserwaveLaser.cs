using System.Diagnostics;
using System.IO.Ports;

namespace Hardwave.Laser
{
    public class LaserwaveLaser : ILaserTemp
    {
        Laserwave _laserwave;

        public LaserwaveLaser()
        {
            _laserwave = new Laserwave();
        }

        public string GetConnectState() => _laserwave._connectState;

        public bool GetPower(int count, out int value) => _laserwave.GetPower(count, out value);

        public bool GetStatus(int count, out bool status) => _laserwave.GetStatus(count, out status);

        public bool Init() => _laserwave.OpenCom();

        public bool SetPower(int count, int value) => _laserwave.SetPower(count, value);

        public bool SetStatus(int count, bool status) => _laserwave.SetStatus(count, status);

        ~LaserwaveLaser()
        {
            _laserwave.CloseCom();
        }
    }

    public class Laserwave
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        public string _connectState = string.Empty;

        /// <summary>
        /// 端口号
        /// </summary>
        public string? _portName;

        /// <summary>
        /// 单位转换
        /// 0~100 对应 0~4095
        /// </summary>
        public double _unitTran = 409.5;

        /// <summary>
        /// 串口设置
        /// </summary>
        private readonly SerialPort? _serialPort = new()
        {
            BaudRate = 115200,
            StopBits = StopBits.One,
            DataBits = 8,
            Parity = Parity.None,
        };

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <returns></returns>
        public bool OpenCom()
        {
            try
            {
                if (Valid())
                {
                    if (_serialPort != null)
                        _serialPort.PortName = _portName;
                }
                else
                {
                    _connectState = "No available laser serial port found";
                    return false;
                }
                if (_serialPort != null && !_serialPort.IsOpen)
                {
                    _serialPort.Open();
                    Channel1Enable = false;
                    Channel2Enable = false;
                    Channel3Enable = false;
                    Channel4Enable = false;
                    return true;
                }
                else
                {
                    Debug.WriteLine($"Failed to connect to port {_portName}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _connectState = $"Failed to connect to port {_portName}";
                Console.WriteLine($"{_connectState}:{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        /// <returns></returns>
        public bool CloseCom()
        {
            try
            {
                if (_serialPort == null)
                    return false;

                if (_serialPort.PortName == string.Empty)
                    _serialPort.PortName = _portName;

                if (_serialPort.IsOpen)
                    _serialPort.Close();

                return _serialPort.IsOpen == false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Disconnect Failed:" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 设置功率（0~4095）
        /// </summary>
        /// <param name="count"></param>
        /// <param name="power">0~100，需转换对应0~4095</param>
        /// <returns></returns>
        public bool SetPower(int count, int power)
        {
            try
            {
                if (_serialPort == null) return false;
                int laserPower = (int)Math.Round(power * _unitTran);
                _serialPort.Write($"SET@{count + 1}={laserPower}#");
                string rtn = _serialPort.ReadExisting();
                var value = CheckContent(rtn);
                return value == "OK!#";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SetPower Failed:" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取激光功率
        /// </summary>
        /// <param name="count"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public bool GetPower(int count, out int power)
        {
            power = 0;
            try
            {
                if (_serialPort == null) return false;
                _serialPort.Write("SET?#");
                string value = CheckContent(_serialPort.ReadExisting());
                if (value == string.Empty) return false;

                Dictionary<int, int> dic = new Dictionary<int, int>();
                foreach (string valueScreen in value.Remove(value.Length - 1).Split(','))
                {
                    dic.Add(Convert.ToInt32(valueScreen.Split(":")[0]), Convert.ToInt32(valueScreen.Split(":")[1]));
                }
                int laserPower = 0;
                if (!dic.TryGetValue(count + 1, out laserPower)) return false;
                power = (int)Math.Round(laserPower / _unitTran);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetPower Error:{ex.Message}; ");
                return false;
            }

        }

        /// <summary>
        /// 设置启停
        /// </summary>
        /// <param name="count"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool SetStatus(int count, bool status)
        {
            if (status)
            {
                if (!LightOn(count)) return false;
                SetChannelEnable(count);
            }
            else
            {
                if (!LightOff(count)) return false;
                SetSingleChannelEnable(count, false);
            }
            return true;
        }

        /// <summary>
        /// 获取设置状态
        /// </summary>
        /// <param name="count"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool GetStatus(int count, out bool status)
        {
            status = false;
            switch (count)
            {
                case 1:
                    status = Channel1Enable;
                    break;
                case 2:
                    status = Channel2Enable;
                    break;
                case 3:
                    status = Channel3Enable;
                    break;
                case 4:
                    status = Channel4Enable;
                    break;
            }
            return true;
        }

        static bool Channel1Enable = false;//原硬件无获取状态功能
        static bool Channel2Enable = false;
        static bool Channel3Enable = false;
        static bool Channel4Enable = false;

        /// <summary>
        /// 设置通道启停状态
        /// </summary>
        /// <param name="count"></param>
        private void SetChannelEnable(int count)
        {
            switch (count)
            {
                case 1:
                    SetSingleChannelEnable(1, true);
                    SetSingleChannelEnable(2, false);
                    SetSingleChannelEnable(3, false);
                    SetSingleChannelEnable(4, false);
                    break;
                case 2:
                    SetSingleChannelEnable(1, false);
                    SetSingleChannelEnable(2, true);
                    SetSingleChannelEnable(3, false);
                    SetSingleChannelEnable(4, false);
                    break;
                case 3:
                    SetSingleChannelEnable(1, false);
                    SetSingleChannelEnable(2, false);
                    SetSingleChannelEnable(3, true);
                    SetSingleChannelEnable(4, false);
                    break;
                case 4:
                    SetSingleChannelEnable(1, false);
                    SetSingleChannelEnable(2, false);
                    SetSingleChannelEnable(3, true);
                    SetSingleChannelEnable(4, false);
                    break;
            }
        }

        /// <summary>
        /// 设置单通道启停状态
        /// </summary>
        /// <param name="count"></param>
        /// <param name="status"></param>
        private void SetSingleChannelEnable(int count, bool status)
        {
            switch (count)
            {
                case 1:
                    Channel1Enable = status;
                    break;
                case 2:
                    Channel2Enable = status;
                    break;
                case 3:
                    Channel3Enable = status;
                    break;
                case 4:
                    Channel4Enable = status;
                    break;
            }
        }

        /// <summary>
        /// 关闭通道
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private bool LightOff(int count)
        {
            try
            {
                if (_serialPort == null) return false;
                _serialPort.Write($"RUN@{count + 1}=0#");
                string rtn = _serialPort.ReadExisting();
                bool isSetSuccessd = CheckContent(rtn) == "SET RUN:OK!#";
                return isSetSuccessd;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LightOff Failed: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 启动通道
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private bool LightOn(int count)
        {
            try
            {
                if (_serialPort == null) return false;
                _serialPort.Write($"RUN@{count + 1}=1#");
                string rtn = _serialPort.ReadExisting();
                bool isSetSuccessd = CheckContent(rtn) == "SET RUN:OK!#";
                return isSetSuccessd;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LightOn Failed: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 验证结束符
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string CheckContent(string value)
        {
            string str = "";
            if (!string.IsNullOrEmpty(value))
            {
                string strScreen = value.Replace("\r\n", "");
                bool checkResult = strScreen[strScreen.Length - 1] == '#';
                if (!checkResult) return "";

                int count = strScreen.Count(x => x == '#');
                if (count > 1)
                {
                    string s = strScreen.Substring(0, strScreen.Length - 1);//剔除最后一个#
                    int num = s.LastIndexOf('#');
                    str = s.Substring(num + 1, s.Length - num - 1) + "#"; ;
                }
                else
                {
                    str = strScreen;
                }
            }
            return str;
        }

        /// <summary>
        /// 校验有效性
        /// </summary>
        /// <returns></returns>
        private bool Valid()
        {
            string comNum = string.Empty;
            try
            {
                if (_serialPort == null)
                    return false;

                //只校验串口是否存在且可用，不考虑是否实际操作
                string[] portNames = SerialPort.GetPortNames();
                foreach (string portName in portNames)
                {
                    comNum = portName;
                    _serialPort.Close();
                    _serialPort.PortName = portName;
                    _serialPort.Open();
                    _serialPort.Write("PN?#");
                    string rtn = _serialPort.ReadExisting();
                    string value = CheckContent(rtn);
                    if (value == "")
                    {
                        _serialPort.Close();
                        continue;
                    }
                    if (value.Substring(0, 2) == "PN")//校验，【查询备注】
                    {
                        Debug.WriteLine($"Connected to {value} on port: {portName}");
                        _serialPort.Close();
                        _portName = portName;
                        break;
                    }
                }
                return _portName != null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Judge valid statement Failed:{ex.Message};Error comNum:{comNum}");
                return false;
            }
        }
    }

}
