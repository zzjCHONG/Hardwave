using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    public static class MshotAPI
    {
        public const string DllName = "SerialCom.dll";

        /// <summary>
        /// 读取指定轴号的位置数据
        /// </summary>
        /// <param name="address">目标轴号(1~9)</param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "DLL_ReadPosition", CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadPosition(uint address);
        /// <summary>
        /// 打开串口通信
        /// 打开后才能控制其他部分
        /// </summary>
        /// <param name="Com">串口号</param>
        /// <param name="Baud">波特率，默认设为 115200</param>
        /// <returns>倘若串口成功打开，返回 1，否则返回-1</returns>
        [DllImport(DllName, EntryPoint = "DLL_OpenCom", CallingConvention = CallingConvention.StdCall)]
        public static extern int OpenCom(int Com, int Baud);
        /// <summary>
        /// 关闭通信串口
        /// </summary>
        /// <returns>倘若串口成功关闭或者串口本身处于关闭 状态，返回 1，否则返回-1</returns>
        [DllImport(DllName, EntryPoint = "DLL_CloseCom", CallingConvention = CallingConvention.StdCall)]
        public static extern int CloseCom();
        /// <summary>
        /// 打开驱动设备
        /// 用于打开或关闭设备，打开设备时自动搜寻可用串口，波特率默认 115200，关闭设备时功能等同 DLL_CloseCom()。
        /// </summary>
        /// <param name="controlNum">‘0’关闭设置，‘1’或‘2’打 开设备</param>
        /// <returns>设置成功，返回 1，否则返回-1</returns>
        [DllImport(DllName, EntryPoint = "DLL_OpenQK", CallingConvention = CallingConvention.StdCall)]
        public static extern int OpenQK(uint controlNum);
        /// <summary>
        /// 控制器使能与失能设置
        /// 上位机中，不打开串口则无法使能以及失能，而在没有使能的情况下，驱控器无法控制位移台进行位移
        /// </summary>
        /// <param name="Address">轴号设置</param>
        /// <param name="KG">“K”使能，“G”失能</param>
        /// <returns>1设置成功，-1 设置失败</returns>
        [DllImport(DllName, EntryPoint = "DLL_AxisEnable", CallingConvention = CallingConvention.StdCall)]
        public static extern int AxisEnable(uint Address, byte KG);
        /// <summary>
        /// 相对移动（以当前平台当前位置为参考进行移动）
        /// </summary>
        /// <param name="address">轴号</param>
        /// <param name="Position">位移距离设置值，位移距离的正负则决定位移的方向</param>
        /// <returns>设置成功，返回 1，否则返回-1</returns>
        [DllImport(DllName, EntryPoint = "DLL_PositionRelativeMove", CallingConvention = CallingConvention.StdCall)]
        public static extern int PositionRelativeMove(uint address, int Position);
        /// <summary>
        /// 绝对移动（以所设零点位置为参考进行移动）
        /// </summary>
        /// <param name="Address">驱控器地址(1~9)</param>
        /// <param name="Position">移动目标位置值（单位count，为位置反馈的最小分辨率)（数值正负 代表目标点位于0点正方向还是负方向）</param>
        /// <returns>设置成功，返回 1，否则返回-1</returns>
        [DllImport(DllName, EntryPoint = "DLL_PositionAbsoluteMove", CallingConvention = CallingConvention.StdCall)]
        public static extern int PositionAbsoluteMove(uint Address, int Position);
        /// <summary>
        /// 设置位移台当前位置为零点
        /// </summary>
        /// <param name="Address"></param>
        /// <returns>设置成功，返回 1，否则返回-1</returns>
        [DllImport(DllName, EntryPoint = "DLL_SetZeroPosition", CallingConvention = CallingConvention.StdCall)]
        public static extern int SetZeroPosition(uint Address);
        /// <summary>
        /// 读取最近一次软件配置出错的错误信息
        /// </summary>
        /// <returns>读取成功则返回错误信息码，没有错误则 返回 0</returns>
        [DllImport(DllName, EntryPoint = "DLL_GetError", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetError();
        /// <summary>
        /// 查询指定轴的运动状态
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="StatusNum">（1查询控制器是否使能，2查询是否 运动，3查询马达是否接入，4查询控制器是否有异常反馈）</param>
        /// <returns>返回 1 则状态位“是”，返回 0 则状态位 “否”，返回-1 则读取失败</returns>
        [DllImport(DllName, EntryPoint = "DLL_GetAxisStatus", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetAxisStatus(uint Address, uint StatusNum);
        /// <summary>
        /// JOG 模式移动
        /// </summary>
        /// <param name="Address">位移轴的轴号</param>
        /// <param name="CMD">CMD 运动命令 ‘L’左运行，‘R’右运行，‘T’停止运行</param>
        /// <returns>1设置成功，-1 设置失败</returns>
        [DllImport(DllName, EntryPoint = "DLL_JOG_Move", CallingConvention = CallingConvention.StdCall)]
        public static extern int JOGMove(uint Address, byte CMD);
        /// <summary>
        /// 速度设置（最低分辨率为 10000count/s，速度设置范围 30000~600000count/s）
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="Type">Type速度类型（'V'普通定位速度，'J'Jog速度，'S'脉冲扫描 速度，'F'开机找index速度）</param>
        /// <param name="countsPerS">速度设置值</param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "DLL_SetSpeed2", CallingConvention = CallingConvention.StdCall)]
        public static extern int SetSpeed2(uint Address, byte Type, uint countsPerS);
        /// <summary>
        /// 读取设置速度（count/s）
        /// </summary>
        /// <param name="Address">驱控器地址(1~9)</param>
        /// <param name="Type">Type速度类型（'V'普通定位速度，'J'Jog速度，'S'脉冲扫描 速度，'F'开机找index速度）</param>
        /// <returns>若读取成功则返回速度值，返回 0 则读 取失败</returns>
        [DllImport(DllName, EntryPoint = "DLL_GetSpeed2", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetSpeed2(uint Address, char Type);
        /// <summary>
        /// 轴停止
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="StatusNum"></param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "DLL_AxisStop", CallingConvention = CallingConvention.StdCall)]
        public static extern int AxisStop(uint Address, byte KG);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="StatusNum"></param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "DLL_SetCMDRT", CallingConvention = CallingConvention.StdCall)]
        public static extern int SetCMDRT(uint Address, uint StatusNum);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="StatusNum"></param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "DLL_GetCMDRT", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetCMDRT(uint Address, uint StatusNum);
    }

    public enum MshotErrorCode
    {
        NONE = -1,
        NO_DEFINE = -2,
        Correct_正确 = 0,
        MOTOR_马达相关错误 = 991,
        NO_MOTOR_没有检测到马达连接 = 992,
        NO_ENABLE_没有使能 = 993,
        NO_CONNECT_没有设备连接 = 994,
        NO_CONTENT_所查询的目标位置没有最新内容 = 995,
        INPUT_PARAMETER_输入参数不符合要求 = 996,
        COM_OPEN_FAILED_DLL_OpenCom_串口打开失败 = 997,
        COM_CLOSE_FAILED_串口关闭失败 = 998,
        COM_CLOSED_串口处于关闭状态 = 999,
        RESPONSE_BUT_FAIL_控制器异常状态或与其他命令设置有冲突 = 1000,
        SET_OVER_TIME_设置超时 = 1001,
        SOCKET_INIT_FAILED_Socket初始化失败 = 1002,
        SOCKET_LINK_FAILED_Socket链接失败 = 1003,
        SOCKET_RECEIVED_Socket1 = 1004,
        QKCMD_SET_ERROR_驱控器返回指令设置错误1 = 1099,
        INVALID_AIXS_ADDRESS_操作轴号为无效轴号 = 1100,
        NO_VALID_DEVICE_没有查询到有效设备1 = 1101,
    }

    [Flags]
    public enum MshotAxis
    {
        ONE = 0x01,
        TWO = 0x02,
        THREE = 0x04,
        ALL = ONE + TWO + THREE,
    }
    public enum AxisStateEnum
    {
        isAxisEnable = 1,
        isMoving = 2,
        isMotorAccess = 3,
        isAxisError = 4
    }

    public enum JogCMDEnum
    {
        L,//左位移
        R,//右位移
        T,//停止移动
    }

    public enum SpeedTypeEnum
    {
        V,//普通定位速度
        J,//JOG位移速度
        S,//扫描速度
        F,//开机找index速度
    }

}
