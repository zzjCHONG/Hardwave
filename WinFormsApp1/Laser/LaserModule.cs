﻿using System.Diagnostics;
using System.IO.Ports;

namespace Hardwave.Laser
{
    public class LaserModule
    {
        /// <summary>
        /// 连接状态
        /// </summary>
       public string _connectState = string.Empty;

        /// <summary>
        /// 端口名
        /// </summary>
        public string? _portName;

        private SerialPort? _serialPort = new()
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
                Debug.WriteLine($"Disconnect Failed:"+ex.Message);
                return false;
            }
        }

        public bool Init() => true;

        public bool DeInit() => true;

        /// <summary>
        /// 校验有效性
        /// </summary>
        /// <returns></returns>
        public bool Valid()
        {
            string comNum = string.Empty;
            try
            {
                if(_serialPort == null) 
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
                Debug.WriteLine($"Judge valid statement Failed:{ex.Message};Error comNum:{comNum}" );
                return false;
            }
        }

        /// <summary>
        /// 获得使能状态
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool Enable(Simscop.Hardware.Laser.Channel channel)
        {
            return channel.Enable;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool LightOff(Simscop.Hardware.Laser.Channel channel)
        {
            try
            {
                if (_serialPort == null) return false;
                int param = 0;
                switch (channel.WaveLength)
                {
                    case 405:
                        param = 1;
                        break;
                    case 488:
                        param = 2;
                        break;
                    case 561:
                        param = 3;
                        break;
                    case 640:
                        param = 4;
                        break;
                    default:
                        return false;
                }
                _serialPort.Write($"RUN@{param}=0#");
                string rtn = _serialPort.ReadExisting();
                bool isSetSuccessd = CheckContent(rtn) == "SET RUN:OK!#";
                channel.Enable = isSetSuccessd;
                return isSetSuccessd;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LightOff Failed: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool LightOn(Simscop.Hardware.Laser.Channel channel)
        {
            try
            {
                if (_serialPort == null) return false;
                int param = 0;
                switch (channel.WaveLength)
                {
                    case 405:
                        param = 1;
                        break;
                    case 488:
                        param = 2;
                        break;
                    case 561:
                        param = 3;
                        break;
                    case 640:
                        param = 4;
                        break;
                    default:
                        return false;
                }
                _serialPort.Write($"RUN@{param}=1#");
                string rtn = _serialPort.ReadExisting();
                bool isSetSuccessd = CheckContent(rtn) == "SET RUN:OK!#";
                channel.Enable = isSetSuccessd;
                return isSetSuccessd;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LightOn Failed: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 设置功率
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public bool SetPower(Simscop.Hardware.Laser.Channel channel, double power)
        {
            try
            {
                if (_serialPort == null) return false;
                int param = 0;
                switch (channel.WaveLength)
                {
                    case 405:
                        param = 1;
                        break;
                    case 488:
                        param = 2;
                        break;
                    case 561:
                        param = 3;
                        break;
                    case 640:
                        param = 4;
                        break;
                    default: 
                        return false;
                }
                channel.Power = power;
                _serialPort.Write($"SET@{param}={power}#");
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
        /// 同步激光状态
        /// 只有功率参数（百分比，0~4095对应0%~100%）
        /// </summary>
        /// <param name="channels"></param>
        /// <returns></returns>
        public bool AsyncStatus(List<Simscop.Hardware.Laser.Channel> channels)
        {
            string rtn = string.Empty;
            try
            {
                if (_serialPort == null) return false;
                _serialPort.Write("SET?#");
                string value = CheckContent(_serialPort.ReadExisting());
                if (value==string.Empty)
                {
                    Debug.WriteLine($"AsyncStatus Failed, rtnMsg is empty;");
                    return false;
                }
                Dictionary<int, int> dic = new Dictionary<int, int>();
                foreach (string valueScreen in value.Remove(value.Length - 1).Split(','))
                {
                    dic.Add(Convert.ToInt32(valueScreen.Split(":")[0]), Convert.ToInt32(valueScreen.Split(":")[1]));
                }

                foreach (var channel in channels)
                {
                    int param = 0;
                    switch (channel.WaveLength)
                    {
                        case 405:
                            param = 1;
                            break;
                        case 488:
                            param = 2;
                            break;
                        case 561:
                            param = 3;
                            break;
                        case 640:
                            param = 4;
                            break;
                    }
                    if (!dic.TryGetValue(param, out int power))
                        return false;
                    channel.Power = power;
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AsyncStatus Error:{ex.Message}; rtnMsg:={rtn}");
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
            if ( !string.IsNullOrEmpty(value))
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
    }
}
