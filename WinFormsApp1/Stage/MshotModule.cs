using Simscop.Hardware.Stage;
using System.Diagnostics;
using System.IO.Ports;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace WinFormsApp1
{
    public class MshotModule
    {
        /// <summary>
        /// 转成μm，倍率为20
        /// </summary>
        private const double Factor = 20.0;
        /// <summary>
        /// X对应轴号1
        /// </summary>
        public const int XAxes = 1;
        /// <summary>
        /// Y对应轴号2
        /// </summary>
        public const int YAxes = 2;
        /// <summary>
        /// Z对应轴号3
        /// </summary>
        public const int ZAxes = 3;
        /// <summary>
        /// X轴实时位置
        /// </summary>
        public double XPosition => (double)MshotAPI.ReadPosition(XAxes) / Factor;
        /// <summary>
        /// Y轴实时位置
        /// </summary>
        public double YPosition => (double)MshotAPI.ReadPosition(YAxes) / Factor;
        /// <summary>
        /// Z轴实时位置
        /// </summary>
        public double ZPosition => (double)MshotAPI.ReadPosition(ZAxes) / Factor;

        /// <summary>
        /// 绝对移动
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool MoveAbsolute(StageAxis axis, double position)
        {
            try
            {
                uint axes = 0;
                switch (axis.Id)
                {
                    case 1:
                        axes = XAxes;
                        break;
                    case 2:
                        axes = YAxes;
                        break;
                    case 3:
                        axes = ZAxes;
                        break;
                    default:
                        return false;
                }
                var offset = (int)(position * Factor);
                if (MshotAPI.PositionAbsoluteMove(axes, offset) == 1) return true;

                Debug.WriteLine("MoveAbsolute failed:" + GetErrorMsg());
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MoveAbsolute error:" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 相对移动
        /// </summary>
        /// <param name="address"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool MoveRelative(StageAxis axis, double position)
        {
            try
            {
                uint axes = 0;
                switch (axis.Id)
                {
                    case 1:
                        axes = XAxes;
                        break;
                    case 2:
                        axes = YAxes;
                        break;
                    case 3:
                        axes = ZAxes;
                        break;
                    default:
                        return false;
                }
                var offset = (int)(position * Factor);
                if (MshotAPI.PositionRelativeMove(axes, offset) == 1) return true;

                Debug.WriteLine("MoveRelative failed:" + GetErrorMsg());
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MoveRelative error:" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <returns></returns>
        public bool OpenCom(int ComNum, int Baud = 115200)
        {
            try
            {
                if (MshotAPI.OpenCom(ComNum, Baud) == 1)
                    return true;

                Debug.WriteLine("OpenCom failed:" + GetErrorMsg());
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("OpenCom error:" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 打开串口-QK
        /// </summary>
        /// <param name="isOpen"></param>
        /// <returns></returns>
        public bool OpenComQK(bool isOpen)
        {
            try
            {
                if (MshotAPI.OpenQK((uint)(isOpen ? 2 : 0)) == 1)
                    return true;

                Debug.WriteLine("OpenComQK failed:" + GetErrorMsg());
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("OpenComQK error:" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 关闭端口
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool CloseCom()
        {
            try
            {
                if (MshotAPI.CloseCom() == 1) return true;

                Debug.WriteLine("CloseCom failed:" + GetErrorMsg());
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CloseCom error:" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 设置当前位置为原点
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool SetHome()
        {
            try
            {
                if (MshotAPI.SetZeroPosition(XAxes) != 1) return false;
                if (MshotAPI.SetZeroPosition(YAxes) != 1) return false;
                if (MshotAPI.SetZeroPosition(ZAxes) != 1) return false;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SetHome error:" + ex.Message);
                return false;
            }
            finally
            {
                Debug.WriteLine("SetHome completed:" + GetErrorMsg());
            }
        }

        /// <summary>
        /// 控制器失能
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public bool DeInit()
        {
            try
            {
                if (MshotAPI.AxisEnable(XAxes, (byte)'G') != 1) return false;
                if (MshotAPI.AxisEnable(YAxes, (byte)'G') != 1) return false;
                if (MshotAPI.AxisEnable(ZAxes, (byte)'G') != 1) return false;
                return true;
            }
            catch (Exception ex )
            {
                Debug.WriteLine("DeInit error:" + ex.Message);
                return false;
            }
            finally
            {
                Debug.WriteLine("DeInit completed:" + GetErrorMsg());
            }
        }

        /// <summary>
        /// 控制器使能
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public bool Init()
        {
            try
            {
                if (MshotAPI.AxisEnable(XAxes, (byte)'K') != 1) return false;
                if (MshotAPI.AxisEnable(YAxes, (byte)'K') != 1) return false;
                if (MshotAPI.AxisEnable(ZAxes, (byte)'K') != 1) return false;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Init error:" + ex.Message);
                return false;
            }
            finally
            {
                Debug.WriteLine("Init completed:" + GetErrorMsg());
            }
        }

        /// <summary>
        /// 校验有效性
        /// </summary>
        /// <returns></returns>
        public bool Valid()
        {
            try
            {
                //1、文件存在
                string dllFilepath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, MshotAPI.DllName);
                if (!File.Exists(dllFilepath)) return false;

                //2、能连接且可用接口正确获取轴的使能状态
                if (MshotAPI.OpenQK(2) != 1) return false;//打开
                if (MshotAPI.AxisEnable(XAxes, (byte)'K') == -1) return false;//设置
                if (MshotAPI.GetAxisStatus(XAxes, 1) == 1)//获取
                    if (MshotAPI.AxisEnable(XAxes, (byte)'G') == -1) return false; //复位                
                if (MshotAPI.CloseCom() != 1) return false;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Valid error:" + ex.Message);
                return false;
            }
        }

        private static MshotErrorCode ErrorMessage { get; set; } = MshotErrorCode.NONE;
        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns></returns>
        public string GetErrorMsg()
        {
            var code = MshotAPI.GetError();
            ErrorMessage = Enum.IsDefined(typeof(MshotErrorCode), code) ? (MshotErrorCode)code : MshotErrorCode.NO_DEFINE;
            //Debug.WriteLine(ErrorMessage);
            return ErrorMessage.ToString();
        }
    }
}
