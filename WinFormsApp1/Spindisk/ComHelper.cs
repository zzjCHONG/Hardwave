using System.IO.Ports;

namespace WinFormsApp1;

public static class ComHelper
{
    /// <summary>
    /// 
    /// </summary>
    public static string EndOfLine = "\r";

    /// <summary>
    /// 
    /// </summary>
    public static int Baud = 115200;

    /// <summary>
    /// 
    /// </summary>
    public static int DataBits = 8;

    /// <summary>
    /// 
    /// </summary>
    public static Parity Parity = System.IO.Ports.Parity.None;

    /// <summary>
    /// 
    /// </summary>
    public static StopBits StopBits = System.IO.Ports.StopBits.One;

    /// <summary>
    /// 
    /// </summary>
    public static Handshake Handshake = System.IO.Ports.Handshake.None;

    /// <summary>
    /// 
    /// </summary>
    public static int ReadTimeout = 500;

    /// <summary>
    /// 
    /// </summary>
    public static int WriteTimeout = 500;

    /// <summary>
    /// 
    /// </summary>
    public static int ReadBufferSize = 1024;

    /// <summary>
    /// 
    /// </summary>
    public const int WriteBufferSize = 1024;

    /// <summary>
    /// 重复调用次数
    /// </summary>
    public const int Repeat = 5;

    /// <summary>
    /// 使用验证信息查找串口
    /// </summary>
    /// <param name="valid">验证使用的命令</param>
    /// <param name="receive">命令接收后的对比参考字符</param>
    /// <param name="wait">接收等待时间</param>
    /// <param name="isContain">是否使用Contain的方式判断，如果为否，需要receive和接收的串口信息一致</param>
    /// <returns>
    /// 如果返回为空，则未找到需要的串口
    /// </returns>
    /// <exception cref="Exception"></exception>
    public static string? Serach(string valid, string receive, int wait = 100, bool isContain = true)
    {
        var ports = SerialPort.GetPortNames();

        foreach (var name in ports)
        {
            var port = new SerialPort(name)
            {
                BaudRate = Baud,
                DataBits = DataBits,
                StopBits = StopBits,
                Parity = Parity,
                Handshake = Handshake,
                ReadTimeout = ReadTimeout,
                WriteTimeout = WriteTimeout,
                ReadBufferSize = ReadBufferSize,
                WriteBufferSize = WriteBufferSize,
                NewLine = EndOfLine,
            };
            try
            {
                port.Open();
                if (!port.IsOpen)
                    throw new Exception("The ComName can`t open.");
                port.Write(valid);
                Thread.Sleep(wait);

                var str = port.ReadExisting();
                if (isContain ? str.Contains(receive) : str == receive) 
                    return name;
            }
            catch (UnauthorizedAccessException e) // the ComName is connected
            {
                // do nothing
            }
            catch (Exception e)
            {
                // 这里超时错误SerialPort没有做单独的区分，因此这里错误不需要抛出，会出现没必要的错误
                //throw new Exception(e.Message, e);
            }
            finally
            {
                port.Close();
            }
        }

        return null;
    }
}

