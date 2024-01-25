using Lift.Core.Common;
using Lift.Core.ImageArray;
using OpenCvSharp;
using Simscop.Hardware.Camera;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using Simscop.API;
using System.Diagnostics.Contracts;

namespace WinFormsApp1
{

    public class AndorModule
    {
        private static int Hndl = 0;
        private static int NumberDevices = 0;
        private static int ImageSizeBytes;

        #region AssertRet
        private bool AssertRet(int ret, bool assertInit = true, bool assertConnect = true)
        {
            if (assertInit && !IsInitialized()) return false;
            if (assertConnect && !IsConnected()) return false;

            var st = new StackTrace(true);
            var msg = AndorErrorCodeEnum.NO_DEFINE;
            msg = Enum.IsDefined(typeof(AndorErrorCodeEnum), ret) ? (AndorErrorCodeEnum)ret : AndorErrorCodeEnum.NO_DEFINE;
            if (ret != (int)AndorErrorCodeEnum.AT_SUCCESS)
            {
                Debug.WriteLine($"[ERROR] [{st?.GetFrame(1)?.GetMethod()?.Name}] {ret}-{msg}");
                return false;
            }

            Debug.WriteLine($"[INFO] [{st?.GetFrame(1)?.GetMethod()?.Name}] {ret}-{msg}");
            return true;
        }
        private bool IsInitialized()
        {
            if (NumberDevices != 0) return true;
            Debug.WriteLine("No camera found");
            return false;
        }
        private bool IsConnected()
        {
            if (Hndl == 0 || ImageSizeBytes == 0)
            {
                Debug.WriteLine("No camera connected");
                return false;
            }
            return true;
        }
        #endregion


        internal bool Connect(int cameraId = 0)
        {
            if (AndorAPI.InitialiseLibrary() != (int)AndorErrorCodeEnum.AT_SUCCESS)
            {
                Debug.WriteLine("InitialiseLibrary Error");
                return false;
            }
            if (!AssertRet(AndorAPI.GetInt(1, "Device Count", ref NumberDevices), false, false)) return false;
                
            Debug.WriteLine("InitializeSdk completed!");

            if (!AssertRet(AndorAPI.Open(cameraId, ref Hndl), assertConnect: false))
            {
                Debug.WriteLine("OpenCamera Error");
                return false;
            }
            
            if (!AssertRet(AndorAPI.GetInt(Hndl, "imageSizeBytes", ref ImageSizeBytes), assertConnect: false)) return false;
            
            SetExposure(100);


            Debug.WriteLine("InitializeCamera completed!");
            return true;
        }

        internal bool Disconnect()=> 
            AssertRet(AndorAPI.Close(Hndl), false, false) 
            && AssertRet(AndorAPI.FinaliseLibrary(), false, false);

        internal bool Init() => true;

        internal bool DeInit() => true;

        internal bool Valid()
        {
            try
            {
                if(!Connect()) return false;

                StringBuilder stringBuilder = new StringBuilder(64);
                if(!AssertRet(AndorAPI.GetString(Hndl, "SerialNumber", stringBuilder, 64)))return false;

                bool result= stringBuilder.ToString() == "VSC-09583";

                if (!Disconnect()) return false;

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Valid error:" + ex.Message);
                return false;
            }
        }

        internal ImageArray? Capture()
        {
            throw new NotImplementedException();
        }

        internal bool Save(string path, SaveImageArgs? args)
        {
            throw new NotImplementedException();
        }

        internal bool Record(string path, RecordArgs? args)
        {
            throw new NotImplementedException();
        }

        #region Setting

        private bool CheckSetMethod(int handle, string feature,ref double input)
        {
            bool isImplemented = false;
            if (!AssertRet(AndorAPI.IsImplemented(handle, feature, ref isImplemented))) return false;

            bool isReadOnly = false;
            if (!AssertRet(AndorAPI.IsReadOnly(handle, feature, ref isReadOnly))) return false;

            if (isImplemented)
            {
                double max = 0;
                double min = 0;
                if (!AssertRet(AndorAPI.GetFloatMax(handle, feature, ref max))) return false;
                if (!AssertRet(AndorAPI.GetFloatMax(handle, feature, ref min))) return false;
                input = input > max ? max : input;
                input = input < min ? min : input;

                bool isWritable = false;
                if (!AssertRet(AndorAPI.IsWritable(handle, feature, ref isWritable))) return false;
            }

            return true;
        }

        internal bool SetExposure(double exposure)
        {
            if (!CheckSetMethod(Hndl, "Exposure Time",ref exposure)) return false;
            if (!AssertRet(AndorAPI.SetFloat(Hndl, "Exposure Time", exposure))) return false;
            return true;
        }

        internal bool SetGamma(double gamma)
        {
            if (!CheckSetMethod(Hndl, "Gamma",ref gamma)) return false;
            if (!AssertRet(AndorAPI.SetFloat(Hndl, "Gamma", gamma))) return false;
            return true;
        }

        internal bool SetLevel(Vector range)
        {
            throw new NotImplementedException();
        }

        internal bool SetResolution(Vector resolution)
        {
            throw new NotImplementedException();
        }

        internal bool SetSmooth(SmoothArgs args)
        {
            throw new NotImplementedException();
        }

        internal bool SetRoi(RoiArgs args)
        {
            throw new NotImplementedException();
        }

        internal bool SetDenoise(int level)
        {
            throw new NotImplementedException();
        }

        internal bool SetContrast(double contrast)
        {
            if (!CheckSetMethod(Hndl, "Gamma", ref contrast)) return false;
            if (!AssertRet(AndorAPI.SetFloat(Hndl, "Gamma", contrast))) return false;
            return true;
        }

        internal bool SetBrightness(double brightness)
        {
            if (!CheckSetMethod(Hndl, "Gamma", ref brightness)) return false;
            if (!AssertRet(AndorAPI.SetFloat(Hndl, "Gamma", brightness))) return false;
            return true;
        }

        private bool CheckGetMethod(int handle, string feature)
        {
            bool isImplemented = false;
            if (!AssertRet(AndorAPI.IsImplemented(handle, feature, ref isImplemented))) return false;

            bool isReadOnly = false;
            if (!AssertRet(AndorAPI.IsReadOnly(handle, feature, ref isReadOnly))) return false;

            if (isImplemented)
            {
                bool isReadable = false;
                if (!AssertRet(AndorAPI.IsReadable(handle, feature, ref isReadable))) return false;
            }

            return true;
        }

        internal bool GetSmmoth(out SmoothArgs args)
        {
            args= new SmoothArgs();
            if (!CheckGetMethod(Hndl, "Gamma")) return false;
            int smoothType = 0;
            if (!AssertRet(AndorAPI.GetInt(Hndl, "Smmoth",ref smoothType))) return false;
            return true;
        }

        internal bool GetRoi(out RoiArgs args)
        {
            throw new NotImplementedException();
        }

        internal bool GetResolution(out Vector resolution)
        {
            throw new NotImplementedException();
        }

        internal bool GetLevel(out Vector vector)
        {
            throw new NotImplementedException();
        }

        internal bool GetGamma(out double gamma)
        {
            gamma = 0;
            if (!CheckGetMethod(Hndl, "Gamma")) return false;
            if (!AssertRet(AndorAPI.GetFloat(Hndl, "Gamma", ref gamma))) return false;
            return true;
        }

        internal bool GetGain(double value, object? args)
        {
            throw new NotImplementedException();
        }

        internal bool GetExposure(out double exposure)
        {
            exposure = 0;
            if (!CheckGetMethod(Hndl, "Exposure")) return false;
            if (!AssertRet(AndorAPI.GetFloat(Hndl, "Exposure", ref exposure))) return false;
            return true;
        }

        internal bool GetDenoise(out int level)
        {
            level = 0;
            if (!CheckGetMethod(Hndl, "Level")) return false;
            if (!AssertRet(AndorAPI.GetInt(Hndl, "Exposure", ref level))) return false;
            return true;
        }

        internal bool GetContrast(out double contrast)
        {
            contrast = 0;
            if (!CheckGetMethod(Hndl, "Contrast")) return false;
            if (!AssertRet(AndorAPI.GetFloat(Hndl, "Contrast", ref contrast))) return false;
            return true;
        }

        internal bool GetBrightness(out double brightness)
        {
            brightness = 0;
            if (!CheckGetMethod(Hndl, "Brightness")) return false;
            if (!AssertRet(AndorAPI.GetFloat(Hndl, "Brightness", ref brightness))) return false;
            return true;
        }

        #endregion

        #region Andor Example

        /// <summary>
        /// 错误信息
        /// </summary>
        private static AndorErrorCodeEnum ErrorMessage { get; set; } = AndorErrorCodeEnum.NO_DEFINE;
        private string GetErrorMsg(int resCode)
        {
            ErrorMessage = Enum.IsDefined(typeof(AndorErrorCodeEnum), resCode) ? (AndorErrorCodeEnum)resCode : AndorErrorCodeEnum.NO_DEFINE;
            return ErrorMessage.ToString();
        }

        #region Acquisition
        /// <summary>
        /// 采图
        /// </summary>
        public void Acquisition()
        {
            try
            {
                int rtnCode = 0;
                Debug.WriteLine("Initialising ...");
                rtnCode = AndorAPI.InitialiseLibrary();
                if (rtnCode != (int)AndorErrorCodeEnum.AT_SUCCESS)
                {
                    Debug.WriteLine("Error initialising library");
                }
                else
                {
                    int iNumberDevices = 0;
                    int AT_HANDLE_SYSTEM = 1;
                    AndorAPI.GetInt(AT_HANDLE_SYSTEM, "Device Count", ref iNumberDevices);
                    if (iNumberDevices <= 0)
                    {
                        Debug.WriteLine("No cameras detected");
                    }
                    else
                    {
                        int Hndl = 0;
                        rtnCode = AndorAPI.Open(0, ref Hndl);
                        if (rtnCode != (int)AndorErrorCodeEnum.AT_SUCCESS)
                        {
                            Debug.WriteLine("Error condition, could not initialise camera" + GetErrorMsg(rtnCode));
                        }
                        else
                        {
                            Debug.WriteLine("Successfully initialised camera");

                            rtnCode = AndorAPI.SetEnumeratedString(Hndl, "PixelEncoding", "Mono16");//Mono12Packed
                            if (rtnCode != (int)AndorErrorCodeEnum.AT_SUCCESS)
                                Debug.WriteLine("Pixel Encoding Setting Error:" + GetErrorMsg(rtnCode));
                            else
                                Debug.WriteLine("Pixel Encoding Set Mono16");

                            rtnCode = AndorAPI.SetEnumeratedString(Hndl, "PixelReadoutRate", "100 MHz");
                            if (rtnCode == (int)AndorErrorCodeEnum.AT_SUCCESS)
                                Debug.WriteLine("Pixel Readout Rate set to 100 MHz");

                            rtnCode = AndorAPI.SetFloat(Hndl, "Exposure Time", 10);
                            if (rtnCode != (int)AndorErrorCodeEnum.AT_SUCCESS)
                                Debug.WriteLine("SetFloat Error:" + GetErrorMsg(rtnCode));

                            int ImageSizeBytes = 0;
                            rtnCode = AndorAPI.GetInt(Hndl, "ImageSizeBytes", ref ImageSizeBytes);

                            //int BufferSize = (int)ImageSizeBytes;
                            byte[] UserBuffer = new byte[ImageSizeBytes];
                            rtnCode = AndorAPI.QueueBuffer(Hndl, UserBuffer, ImageSizeBytes);
                            if (rtnCode != (int)AndorErrorCodeEnum.AT_SUCCESS)
                                Debug.WriteLine("QueueBuffer Error:" + GetErrorMsg(rtnCode));

                            rtnCode = AndorAPI.Command(Hndl, "AcquisitionStart");
                            if (rtnCode != (int)AndorErrorCodeEnum.AT_SUCCESS)
                            {
                                Debug.WriteLine("Acquisition Start Error:" + GetErrorMsg(rtnCode));
                            }
                            else
                            {
                                Debug.WriteLine("Waiting for acquisition ...");
                            }

                            byte[]? buffer = new byte[ImageSizeBytes];
                            IntPtr imgPtr = Marshal.AllocHGlobal(buffer.Length);
                            if (AndorAPI.WaitBuffer(Hndl, ref imgPtr, ref ImageSizeBytes, 20000) == (int)AndorErrorCodeEnum.AT_SUCCESS)
                            {
                                //int width = 2560;
                                //int height = 2160;
                                //buffer = PtrToArr(imgPtr, width, height);
                                Marshal.Copy(imgPtr, buffer, 0, buffer.Length);

                                //Debug.WriteLine("Acquisition finished successfully");
                                //Debug.WriteLine("Number bytes received ");
                                //Debug.WriteLine("Print out of first 20 pixels ");
                                //for (int i = 0; i < 3 * 10; i += 3)
                                //{
                                //    byte[] newBuffer = new byte[buffer.Length - i];
                                //    System.Buffer.BlockCopy(buffer, i, newBuffer, 0, newBuffer.Length);
                                //    int LowPixel = ExtractLowPacked_Mono12Packed(newBuffer);
                                //    int HighPixel = ExtractHighPacked_Mono12Packed(newBuffer);
                                //    Debug.WriteLine(LowPixel);
                                //    Debug.WriteLine(HighPixel);
                                //}

                                string imageFileName = $"C:\\Users\\dell\\Desktop\\AndorImage\\{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + ".tif"}";
                                SaveCVImage(Hndl, buffer, PixelEncodingEnum.Mono16, imageFileName);
                                Debug.WriteLine("Image generate completed!");
                                //SaveHoImage(Hndl, imgPtr, fileName);//Halcon

                                buffer = null;
                            }
                            else
                            {
                                Debug.WriteLine("Timeout occurred check the log file ...");
                            }
                            rtnCode = AndorAPI.Command(Hndl, "Acquisition Stop");
                            rtnCode = AndorAPI.Flush(Hndl);
                        }
                        rtnCode = AndorAPI.Close(Hndl);
                    }
                    rtnCode = AndorAPI.FinaliseLibrary();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static int ExtractLowPacked_Mono12Packed(byte[] sourcePtr)//Mono12Packed转换，其他格式需要切换移位方式
        {
            return (sourcePtr[0] << 4) + (sourcePtr[1] & 0xF);
        }
        public static int ExtractHighPacked_Mono12Packed(byte[] sourcePtr)
        {
            return (sourcePtr[2] << 4) + (sourcePtr[1] >> 4);
        }

        /// <summary>
        /// 存图
        /// </summary>
        /// <param name="Hndl"></param>
        /// <param name="buffer"></param>
        /// <param name="pixelEncoding"></param>
        /// <param name="imageFileName"></param>
        /// <returns></returns>
        public bool SaveCVImage(int Hndl, byte[] buffer, PixelEncodingEnum pixelEncoding, string imageFileName)
        {
            try
            {
                int ImageHeight = 0;
                AndorAPI.GetInt(Hndl, "AOI Height", ref ImageHeight);
                int ImageWidth = 0;
                AndorAPI.GetInt(Hndl, "AOI Width", ref ImageWidth);
                MatType matType = new MatType();
                switch (pixelEncoding)
                {
                    case PixelEncodingEnum.Mono8:
                        matType = MatType.CV_8UC1;
                        break;
                    case PixelEncodingEnum.Mono12:
                        matType = MatType.CV_16UC1;
                        break;
                    case PixelEncodingEnum.Mono12PACKED:
                        matType = MatType.CV_16UC1;
                        break;
                    case PixelEncodingEnum.Mono16:
                        matType = MatType.CV_16UC1;
                        break;
                    case PixelEncodingEnum.Mono32:
                        matType = MatType.CV_32SC1;
                        break;
                    default:
                        throw new ArgumentException("Invalid pixel format.");
                }
                Mat matImg = new Mat(ImageHeight, ImageWidth, matType);
                Marshal.Copy(buffer, 0, matImg.Data, buffer.Length);

                //旋转
                Mat matImgRotate = new Mat((int)ImageHeight, (int)ImageWidth, MatType.CV_16UC1);
                Cv2.Rotate(matImg, matImgRotate, RotateFlags.Rotate180);
                Mat matImgFlip = new Mat((int)ImageHeight, (int)ImageWidth, MatType.CV_16UC1);
                Cv2.Flip(matImgRotate, matImgFlip, FlipMode.Y);

                //图像水平&垂直分辨率、压缩比
                ImwriteFlags flags = ImwriteFlags.TiffCompression;
                ImwriteFlags dpix = ImwriteFlags.TiffXDpi;
                ImwriteFlags dpiy = ImwriteFlags.TiffYDpi;
                ImageEncodingParam[] encodingParams = new ImageEncodingParam[] { new ImageEncodingParam(dpix, 96), new ImageEncodingParam(dpiy, 96), new ImageEncodingParam(flags, 1) };

                Cv2.ImWrite(imageFileName, matImgFlip, encodingParams);

                return true;
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
                return false;
            }
        }


        private void SaveCVImage(int Hndl, byte[] buffer)
        {
            int ImageHeight = 0;
            AndorAPI.GetInt(Hndl, "AOI Height", ref ImageHeight);
            int ImageWidth = 0;
            AndorAPI.GetInt(Hndl, "AOI Width", ref ImageWidth);
            Mat matImg = new Mat(ImageHeight, ImageWidth, MatType.CV_16UC1);
            Marshal.Copy(buffer, 0, matImg.Data, buffer.Length);
            ImwriteFlags flags = ImwriteFlags.TiffCompression;
            ImwriteFlags dpix = ImwriteFlags.TiffXDpi;
            ImwriteFlags dpiy = ImwriteFlags.TiffYDpi;
            ImageEncodingParam[] encodingParams = new ImageEncodingParam[] { new ImageEncodingParam(dpix, 96), new ImageEncodingParam(dpiy, 96), new ImageEncodingParam(flags, 1) };
            //string fileName = $"C:\\Users\\dell\\Desktop\\ImageAndorCamera\\{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".tif"}";
            //Cv2.ImWrite(fileName, matImg, encodingParams);

            string fileNameRotate = $"C:\\Users\\dell\\Desktop\\ImageAndorCamera\\{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + ".tif"}";
            Mat matImgRotate = new Mat(ImageHeight, ImageWidth, MatType.CV_16UC1);
            Cv2.Rotate(matImg, matImgRotate, RotateFlags.Rotate180);
            Mat matImgFlip = new Mat(ImageHeight, ImageWidth, MatType.CV_16UC1);
            Cv2.Flip(matImgRotate, matImgFlip, FlipMode.Y);
            Cv2.ImWrite(fileNameRotate, matImgFlip, encodingParams);
        }

        public void ConvertByteArrayToTif(byte[] byteArray, string filePath)
        {
            using (var ms = new MemoryStream(byteArray))
            {
                //Image image = Image.FromStream(ms);
                using (var image = new Bitmap(ms))
                {
                    image.Save(filePath, ImageFormat.Tiff);
                }
            }
        }

        //private void SaveHoImage(int Hndl, IntPtr imgPtr, string fileName)
        //{
        //    try
        //    {
        //        int ImageHeight = 0;
        //        AndorAPI.GetInt(Hndl, "AOI Height", ref ImageHeight);
        //        int ImageWidth = 0;
        //        AndorAPI.GetInt(Hndl, "AOI Width", ref ImageWidth);

        //        HOperatorSet.GenImage1(out HObject ho_Image, "byte", ImageWidth, ImageHeight, imgPtr);

        //        string? path = Path.GetDirectoryName(fileName);
        //        if (!System.IO.Directory.Exists(path))
        //            System.IO.Directory.CreateDirectory(path);
        //        string strExtensionWithoutPoint = fileName.Substring(fileName.LastIndexOf(".") + 1);
        //        if (ho_Image != null && ho_Image.IsInitialized())
        //            HOperatorSet.WriteImage(ho_Image, strExtensionWithoutPoint, 255, fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        private static byte[] PtrToArr(IntPtr pRet, int width, int height)
        {
            unsafe
            {
                byte* memBytePtr = (byte*)pRet.ToPointer();
                UnmanagedMemoryStream readStream = new UnmanagedMemoryStream(memBytePtr, width * height, width * height, FileAccess.Read);
                byte[] buffer = new byte[readStream.Length];
                int bytesRead;
                readStream.Position = 0;
                using (MemoryStream ret = new MemoryStream())
                {
                    while ((bytesRead = readStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ret.Write(buffer, 0, bytesRead);
                    }
                    ret.Read(buffer, 0, (int)ret.Length);
                }
                return buffer;
            }
        }

        #endregion

        #region Serialnumber
        /// <summary>
        /// 获得当前设备序列号
        /// </summary>
        public void GetSerialNumber()
        {
            try
            {
                Debug.WriteLine("Initialising ...");
                int i_retCode = 0;
                if (AndorAPI.InitialiseLibrary() != (int)AndorErrorCodeEnum.AT_SUCCESS)
                {
                    Debug.WriteLine("Error initialising library");
                }
                else
                {
                    int iNumberDevices = 0;
                    int AT_HANDLE_SYSTEM = 1;

                    i_retCode = AndorAPI.GetInt(AT_HANDLE_SYSTEM, "Device Count", ref iNumberDevices);
                    if (iNumberDevices <= 0)
                    {
                        Debug.WriteLine("No cameras detected");
                    }
                    else
                    {
                        int Hndl = 0;
                        if (AndorAPI.Open(0, ref Hndl) != (int)AndorErrorCodeEnum.AT_SUCCESS)
                            Debug.WriteLine("Open Error:" + GetErrorMsg(i_retCode));

                        //i_retCode =AndorAPI.OpenDevice("Library:regcam, Index:0", ref Hndl);

                        if (i_retCode != (int)AndorErrorCodeEnum.AT_SUCCESS)
                        {
                            Debug.WriteLine("Error condition, could not initialise camera");
                        }
                        else
                        {
                            Debug.WriteLine("Successfully initialised camera");
                        }
                        StringBuilder stringBuilder = new StringBuilder(64);
                        i_retCode = AndorAPI.GetString(Hndl, "FirmwareVersion", stringBuilder, 64);//SerialNumber，CameraModel,FirmwareVersion
                        if (i_retCode == (int)AndorErrorCodeEnum.AT_SUCCESS)
                        {
                            Debug.WriteLine("The serial number is " + stringBuilder);
                        }
                        else
                        {
                            Debug.WriteLine("Error obtaining Serial number");
                        }
                        AndorAPI.Close(Hndl);
                    }
                    AndorAPI.FinaliseLibrary();
                }
                Debug.WriteLine("Press any key then enter to close");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Live

        /// <summary>
        /// 说明书中例程，当前可用
        /// </summary>
        public void GetLiveImage()
        {
            int rtnCode = 0;
            rtnCode = AndorAPI.InitialiseLibrary();
            int Handle = 0;
            rtnCode = AndorAPI.Open(0, ref Handle);
            rtnCode = AndorAPI.SetFloat(Handle, "ExposureTime", 0.01);
            rtnCode = AndorAPI.SetEnumeratedString(Handle, "PixelEncoding", "Mono16");//Mono12Packed
            int ImageSizeBytes = 0;
            rtnCode = AndorAPI.GetInt(Handle, "ImageSizeBytes", ref ImageSizeBytes);
            int BufferSize = (int)(ImageSizeBytes);

            //Declare the number of buffers and the number of frames interested in
            int NumberOfBuffers = 10;
            int NumberOfFrames = 20;

            //Allocate a number of memory buffers to store frame
            byte[][] AcqBuffers = new byte[NumberOfBuffers][];
            byte[][] AlignedBuffers = new byte[NumberOfBuffers][];
            for (int i = 0; i < NumberOfBuffers; i++)
            {
                AcqBuffers[i] = new byte[BufferSize + 7];
                //AlignedBuffers[i] = new byte[BufferSize];
                AlignedBuffers[i] = (byte[])(Array.CreateInstance(typeof(byte), 1));
                Buffer.BlockCopy(AcqBuffers[i % NumberOfBuffers], 0, AlignedBuffers[i], 0, BufferSize + 7);
            }

            //Pass these buffers to the SDK
            for (int i = 0; i < NumberOfBuffers; i++)
            {
                rtnCode = AndorAPI.QueueBuffer(Handle, AlignedBuffers[i], BufferSize);
            }

            //Set the camera to continuously acquires frames
            rtnCode = AndorAPI.SetEnumeratedString(Handle, "CycleMode", "Continuous");
            //Start the Acquisition running
            rtnCode = AndorAPI.Command(Handle, "AcquisitionStart");

            //Sleep in this thread until data is ready, in this case set
            //the timeout to infinite for simplicity
            byte[] pBuf = new byte[BufferSize];
            IntPtr imgPtr = Marshal.AllocHGlobal(pBuf.Length);
            int BufSize = 0;
            uint AT_INFINITE = unchecked(0xFFFFFFFF);
            int[] data = new int[2];
            for (int i = 0; i < NumberOfFrames; i++)
            {
                rtnCode = AndorAPI.WaitBuffer(Handle, ref imgPtr, ref BufSize, AT_INFINITE);

                //Application specific data processing goes here..
                Marshal.Copy(imgPtr, pBuf, 0, pBuf.Length);
                Extract2from3(pBuf, data);
                Debug.WriteLine($"Image {i + 1}  First 2 pixels " + data[0] + " " + data[1]);//打印每张图的前两个像素

                string imageFileName = $"C:\\Users\\dell\\Desktop\\ImageAndorCamera\\{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + ".tif"}";
                SaveCVImage(Handle, pBuf, PixelEncodingEnum.Mono16, imageFileName);
                Thread.Sleep(20);

                //Re-queue the buffers
                //%取余，i % NumberOfBuffers，确保索引始终在0到NumberOfBuffers-1的范围内
                rtnCode = AndorAPI.QueueBuffer(Handle, AlignedBuffers[i % NumberOfBuffers], BufferSize);
            }

            //Stop the acquisition
            rtnCode = AndorAPI.Command(Handle, "AcquisitionStop");
            rtnCode = AndorAPI.Flush(Handle);

            //Application specific data processing goes here..

            //Free the allocated buffer
            for (int i = 0; i < NumberOfBuffers; i++)
            {
                AcqBuffers[i] = null;
            }
            AlignedBuffers = null;
            AcqBuffers = null;

            rtnCode = AndorAPI.Close(Handle);
            rtnCode = AndorAPI.FinaliseLibrary();
        }

        /// <summary>
        /// 无法正常使用
        /// </summary>
        public void Live()
        {
            int i_retCode;
            Debug.WriteLine("Initialising ...\n");
            i_retCode = AndorAPI.InitialiseLibrary();
            if (i_retCode != (int)AndorErrorCodeEnum.AT_SUCCESS)
            {
                Debug.WriteLine("Error initialising library\n");
            }
            else
            {
                int iNumberDevices = 0;
                int AT_HANDLE_SYSTEM = 1;
                AndorAPI.GetInt(AT_HANDLE_SYSTEM, "DeviceCount", ref iNumberDevices);
                if (iNumberDevices <= 0)
                {
                    Debug.WriteLine("No cameras detected\n");
                }
                else
                {
                    int Hndl = 0;
                    i_retCode = AndorAPI.Open(0, ref Hndl);
                    if (i_retCode != (int)AndorErrorCodeEnum.AT_SUCCESS)
                    {
                        Debug.WriteLine("Error condition, could not initialise camera\n");
                    }
                    else
                    {
                        Debug.WriteLine("Successfully initialised camera\n");

                        StringBuilder szValue = new StringBuilder(64);
                        i_retCode = AndorAPI.GetString(Hndl, "Serial Number", szValue, 64);
                        if (i_retCode == (int)AndorErrorCodeEnum.AT_SUCCESS)
                        {
                            Debug.WriteLine("The serial number is " + szValue + "\n");
                        }
                        else
                        {
                            Debug.WriteLine("Error obtaining Serial number\n");
                        }

                        i_retCode = AndorAPI.SetEnumeratedString(Hndl, "PixelEncoding", "Mono12Packed");
                        if (i_retCode == (int)AndorErrorCodeEnum.AT_SUCCESS)
                        {
                            Debug.WriteLine("Pixel Encoding set to Mono12Packed\n");
                        }

                        int iret = AndorAPI.SetEnumeratedString(Hndl, "TriggerMode", "Software");
                        if (iret != (int)AndorErrorCodeEnum.AT_SUCCESS)
                        {
                            Debug.WriteLine("Error setting trigger mode to Software, retcode=" + iret + "\n");
                        }
                        else
                        {
                            Debug.WriteLine("Trigger mode set to Software\n");
                        }

                        iret = AndorAPI.SetEnumeratedString(Hndl, "CycleMode", "Continuous");
                        if (iret != (int)AndorErrorCodeEnum.AT_SUCCESS)
                        {
                            Debug.WriteLine("Error setting Cycle Mode to Continuous, retcode=" + iret + "\n");
                        }
                        else
                        {
                            Debug.WriteLine("CycleMode set to Continuous\n");
                        }

                        int i_numAcqs = 0;
                        GetUserSettings(Hndl, ref i_numAcqs);

                        Debug.WriteLine("\nAbout to perform " + i_numAcqs + " acquisitions\n");

                        PerformAcquisition(Hndl, i_numAcqs);
                    }
                    AndorAPI.Close(Hndl);
                }
                AndorAPI.FinaliseLibrary();
            }
        }

        int GetUserSettings(int _handle, ref int _i_numAcqs)
        {
            int i_retCode;

            Debug.WriteLine("Enter the pixel Readout rate, 100, 200 or 280");
            //int i_rate = int.Parse(Debug.ReadLine());
            int i_rate = 100;
            Debug.WriteLine($"Choose the pixel Readout rate: {i_rate}");
            if (i_rate == 100)
            {
                i_retCode = AndorAPI.SetEnumeratedString(_handle, "PixelReadoutRate", "100 MHz");
            }
            else if (i_rate == 200)
            {
                i_retCode = AndorAPI.SetEnumeratedString(_handle, "PixelReadoutRate", "200 MHz");
            }
            else
            {
                i_retCode = AndorAPI.SetEnumeratedString(_handle, "PixelReadoutRate", "280 MHz");
            }
            if (i_retCode != (int)AndorErrorCodeEnum.AT_SUCCESS)
            {
                Debug.WriteLine("Error setting Pixel Readout Rate ");
            }

            int i_index = 0;
            i_retCode = AndorAPI.GetEnumerated(_handle, "PixelReadoutRate", ref i_index);
            if (i_retCode != (int)AndorErrorCodeEnum.AT_SUCCESS)
            {
                Debug.WriteLine("Error getting PixelReadoutRate index ");
            }

            StringBuilder szValue = new StringBuilder(64);
            i_retCode = AndorAPI.GetEnumeratedstring(_handle, "PixelReadoutRate", i_index, szValue, 64);
            if (i_retCode != (int)AndorErrorCodeEnum.AT_SUCCESS)
            {
                Debug.WriteLine("Error getting PixelReadoutRate string ");
            }
            Debug.WriteLine("PixelReadoutRate set to " + szValue);

            Debug.WriteLine("Enter the Exposure time in seconds, eg 0.01");
            //float f_exp = float.Parse(Debug.ReadLine());
            float f_exp = (float)0.01;//单位：s
            i_retCode = AndorAPI.SetFloat(_handle, "ExposureTime", f_exp);
            if (i_retCode != (int)AndorErrorCodeEnum.AT_SUCCESS)
            {
                Debug.WriteLine($"Error setting Exposure time to {f_exp} Error code{i_retCode}");
            }
            double d_actual = 0;
            i_retCode = AndorAPI.GetFloat(_handle, "ExposureTime", ref d_actual);
            if (i_retCode != (int)AndorErrorCodeEnum.AT_SUCCESS)
            {
                Debug.WriteLine("Error getting Exposure time, Error code " + i_retCode);
            }
            Debug.WriteLine($"Exposure time set to {d_actual} second(s)");

            _i_numAcqs = 5;
            Debug.WriteLine($"Enter the number of acquisitions to perform：{_i_numAcqs}");
            //_i_numAcqs = int.Parse(Debug.ReadLine());

            return i_retCode;

        }

        int PerformAcquisition(int _handle, int _numberAcquisitions)
        {
            int iret = 0;

            CreateBuffers(_handle);

            iret = AndorAPI.Command(_handle, "AcquisitionStart");
            Thread.Sleep(100);

            if (iret == 0)
            {
                if (DoLoopOfAcquisition(_handle, _numberAcquisitions) != 0)
                {
                    iret = 1;
                }
            }
            AndorAPI.Command(_handle, "AcquisitionStop");
            AndorAPI.Flush(_handle);

            DeleteBuffers();

            return iret;
        }

        void Extract2from3(byte[] _buffer, int[] _i_returns)
        {
            _i_returns[1] = (_buffer[0] << 4) + (_buffer[1] & 0xF);
            _i_returns[0] = (_buffer[2] << 4) + (_buffer[1] >> 4);
        }

        byte[]? acqBuffer = null;
        byte[]? acqBuffer1 = null;
        byte[]? acqBuffer2 = null;
        byte[]? acqBuffer3 = null;
        byte[]? acqBuffer4 = null;
        void CreateBuffers(int _handle)
        {
            acqBuffer = QueueBuffer(_handle);
            acqBuffer1 = QueueBuffer(_handle);
            acqBuffer2 = QueueBuffer(_handle);
            acqBuffer3 = QueueBuffer(_handle);
            acqBuffer4 = QueueBuffer(_handle);
        }

        int DoLoopOfAcquisition(int _handle, int _i_count)
        {
            try
            {
                int iret;
                int[] data = new int[2];

                int ImageSizeBytes = 0;
                iret = AndorAPI.GetInt(_handle, "ImageSizeBytes", ref ImageSizeBytes);

                for (int i = 0; i < _i_count; i++)
                {
                    byte[]? pBuf = new byte[ImageSizeBytes];
                    IntPtr imgPtr = IntPtr.Zero;


                    iret = AndorAPI.Command(_handle, "SoftwareTrigger");
                    if (iret != (int)AndorErrorCodeEnum.AT_SUCCESS)
                    {
                        Debug.WriteLine("Error:Return from Software trigger command not success " + iret);
                        return 1;
                    }

                    imgPtr = Marshal.AllocHGlobal(pBuf.Length);
                    int BufSize = 0;

                    uint AT_INFINITE = unchecked(0xFFFFFFFF);
                    System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    iret = AndorAPI.WaitBuffer(_handle, ref imgPtr, ref BufSize, AT_INFINITE);
                    //do
                    //{
                    //    iret = AndorAPI.WaitBuffer(_handle, ref imgPtr, ref BufSize, AT_INFINITE);
                    //    Thread.Sleep(200);
                    //} while (iret != (int)AndorErrorCodeEnum.AT_SUCCESS);

                    stopwatch.Stop();
                    Debug.WriteLine($"间隔时间：{stopwatch.ElapsedMilliseconds}");

                    if (iret != (int)AndorErrorCodeEnum.AT_SUCCESS)
                    {
                        pBuf = null;
                        Marshal.FreeHGlobal(imgPtr);
                        Thread.Sleep(100);
                        Debug.WriteLine("Error:Acquisition timeout when not expecting, retcode " + GetErrorMsg(iret));
                        return 1;
                    }
                    Debug.WriteLine("Got image " + (i + 1) + " out of " + _i_count);

                    pBuf = new byte[BufSize];
                    Marshal.Copy(imgPtr, pBuf, 0, pBuf.Length);
                    Extract2from3(pBuf, data);
                    Debug.WriteLine("  First 2 pixels " + data[0] + " " + data[1]);//打印每张图的前两个像素

                    iret = AndorAPI.QueueBuffer(_handle, pBuf, (int)ImageSizeBytes);

                    Thread.Sleep(100);
                    Marshal.FreeHGlobal(imgPtr);
                }
                MessageBox.Show("Complete!");
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"DoLoopOfAcquisition {ex.Message}");
                return 1;
            }
        }

        void DeleteBuffers()
        {
            acqBuffer = null;
            acqBuffer1 = null;
            acqBuffer2 = null;
            acqBuffer3 = null;
            acqBuffer4 = null;
        }

        private byte[] QueueBuffer(int _handle)
        {
            int iError = 0;
            int ImageSizeBytes = 0;
            iError = AndorAPI.GetInt(_handle, "ImageSizeBytes", ref ImageSizeBytes);
            if (iError != (int)AndorErrorCodeEnum.AT_SUCCESS)
            {
                Debug.WriteLine("AT_GetInt failed - ImageSizeBytes - return code " + iError);
            }

            byte[]? acqBuffer = null;
            if (iError == (int)AndorErrorCodeEnum.AT_SUCCESS)
            {
                // Allocate a memory buffer to store one frame
                acqBuffer = new byte[ImageSizeBytes + 7];
                // Pass this buffer to the SDK
                iError = AndorAPI.QueueBuffer(_handle, acqBuffer, (int)ImageSizeBytes);
                if (iError != (int)AndorErrorCodeEnum.AT_SUCCESS)
                {
                    Debug.WriteLine("AT_QueueBuffer failed - Image Size Bytes - return code " + iError);
                }
            }
            return acqBuffer;
        }

        #endregion

        #region ReconnectduringAcquisition
        //相机断连时的重连demo
        public void ReconnectduringAcquisition()
        {

        }

        private int Run()
        {
            Debug.WriteLine("Initialising ...");

            int deviceCount = 0;
            RetutnonFailure0(() => InitialiseLibrary(ref deviceCount));
            if (deviceCount == 0)
                return (int)AndorErrorCodeEnum.AT_SUCCESS;

            int hndl = 0;
            RetutnonFailure0(() => OpenCamera(0, ref hndl));

            //AcquisitionManager manager(hndl);

            //RETURN_ON_FAILURE(manager.ConfigureAcquisition());
            //RETURN_ON_FAILURE(AT_RegisterFeatureCallback(hndl, L"CameraPresent", CameraPresentCallback, &manager));

            //RETURN_ON_FAILURE(manager.StartAcquisition());

            //std::cout << "Acquisition running, try disconnecting and reconnecting the camera" << std::endl;
            //int imageCount = 0;
            //while (1)
            //{
            //    int ret = manager.AcquireFrame();
            //    if (ret == AT_SUCCESS)
            //    {
            //        std::cout << "Acquired Image " << (++imageCount) << std::endl;
            //    }
            //    else
            //    {
            //        std::cout << "Acquisition failed with code " << ret << std::endl;
            //    }

            //    if (imageCount >= IMAGES_TO_ACQUIRE)
            //    {
            //        break;
            //    }
            //    Sleep(400);
            //}

            //WARN_ON_FAILURE(manager.StopAcquisition());
            //RETURN_ON_FAILURE(AT_Close(hndl));
            //RETURN_ON_FAILURE(AT_FinaliseLibrary());

            return (int)AndorErrorCodeEnum.AT_SUCCESS;
        }

        private int OpenCamera(int device, ref int Handle)
        {
            StringBuilder szValue = new StringBuilder(64);
            int Hndl = 0;

            RetutnonFailure0(() => AndorAPI.Open(device, ref Hndl));

            if (RetutnonFailure0(() => AndorAPI.GetString(Hndl, "SerialNumber", szValue, 64)))
                Debug.WriteLine("Software Version: " + szValue);
            if (RetutnonFailure0(() => AndorAPI.GetString(Hndl, "CameraModel", szValue, 64)))
                Debug.WriteLine("Software Version: " + szValue);
            if (RetutnonFailure0(() => AndorAPI.GetString(Hndl, "FirmwareVersion", szValue, 64)))
                Debug.WriteLine("Software Version: " + szValue);

            return (int)AndorErrorCodeEnum.AT_SUCCESS;
        }

        private int InitialiseLibrary(ref int deviceCount)
        {
            RetutnonFailure0(() => AndorAPI.InitialiseLibrary());
            StringBuilder szValue = new StringBuilder(64);
            int AT_HANDLE_SYSTEM = 1;
            if (RetutnonFailure0(() => AndorAPI.GetString(AT_HANDLE_SYSTEM, "SoftwareVersion", szValue, 64)))
                Debug.WriteLine("Software Version: " + szValue);
            int deviceCount64 = 0;
            if (RetutnonFailure0(() => AndorAPI.GetInt(AT_HANDLE_SYSTEM, "DeviceCount", ref deviceCount64)))
                Debug.WriteLine("Software Version: " + szValue);
            deviceCount = (int)deviceCount64;
            return (int)AndorErrorCodeEnum.AT_SUCCESS;
        }

        private bool RetutnonFailure0(Func<int> action)
        {
            int result = action();
            bool res = (result == (int)AndorErrorCodeEnum.AT_SUCCESS);
            if (!res)
            {
                ErrorMessage = Enum.IsDefined(typeof(AndorErrorCodeEnum), result) ? (AndorErrorCodeEnum)result : AndorErrorCodeEnum.NO_DEFINE;
                Debug.WriteLine($"{action} returned error codeMsg:{ErrorMessage}");
            }
            return res;
        }
        private string WarnonFailure(Func<int> action)
        {
            int result = action();
            if (result != (int)AndorErrorCodeEnum.AT_SUCCESS)
            {
                ErrorMessage = Enum.IsDefined(typeof(AndorErrorCodeEnum), result) ? (AndorErrorCodeEnum)result : AndorErrorCodeEnum.NO_DEFINE;
                Debug.WriteLine($"{action} returned error codeMsg:{ErrorMessage}");
            }
            return ErrorMessage.ToString();
        }



        #endregion

        #region ConverBuffer
        public void ConverBuffer()
        {
            try
            {
                int i_retCode;
                Debug.WriteLine("Initialising ...");
                i_retCode = AndorAPI.InitialiseLibrary();
                if (i_retCode == (int)AndorErrorCodeEnum.AT_SUCCESS)
                {
                    i_retCode = AndorAPI.InitialiseUtilityLibrary();
                    if (i_retCode == (int)AndorErrorCodeEnum.AT_SUCCESS)
                    {
                        int iNumberDevices = 0;
                        int AT_HANDLE_SYSTEM = 1;
                        i_retCode = AndorAPI.GetInt(AT_HANDLE_SYSTEM, "Device Count", ref iNumberDevices);
                        if (iNumberDevices > 0)
                        {
                            int Hndl = 0;
                            i_retCode = AndorAPI.Open(0, ref Hndl);
                            if (i_retCode == (int)AndorErrorCodeEnum.AT_SUCCESS)
                            {
                                Debug.WriteLine("Successfully initialised camera");

                                AndorAPI.SetEnumeratedString(Hndl, "Pixel Encoding", "Mono12Packed");
                                AndorAPI.SetFloat(Hndl, "Exposure Time", 0.01);

                                //Get the number of bytes required to store one frame
                                int iImageSizeBytes = 0;
                                AndorAPI.GetInt(Hndl, "Image Size Bytes", ref iImageSizeBytes);
                                int iBufferSize = (int)iImageSizeBytes;

                                //Allocate a memory buffer to store one frame
                                byte[] UserBuffer = new byte[iBufferSize];
                                //unsigned char* UserBuffer = new unsigned char[iBufferSize];

                                AndorAPI.QueueBuffer(Hndl, UserBuffer, iBufferSize);
                                AndorAPI.Command(Hndl, "Acquisition Start");
                                Debug.WriteLine("Waiting for acquisition ...");

                                byte[] buffer = new byte[1024];
                                IntPtr imgPtr = Marshal.AllocHGlobal(buffer.Length);
                                if (AndorAPI.WaitBuffer(Hndl, ref imgPtr, ref iBufferSize, 10000) == (int)AndorErrorCodeEnum.AT_SUCCESS)
                                {

                                    Marshal.Copy(imgPtr, buffer, 0, buffer.Length);

                                    Debug.WriteLine("Acquisition finished successfully");

                                    //Unpack the 12 bit packed data
                                    int ImageHeight = 0;
                                    AndorAPI.GetInt(Hndl, "AOI Height", ref ImageHeight);
                                    int ImageWidth = 0;
                                    AndorAPI.GetInt(Hndl, "AOI Width", ref ImageWidth);
                                    int ImageStride = 0;
                                    AndorAPI.GetInt(Hndl, "AOI Stride", ref ImageStride);
                                    //unsigned short* unpackedBuffer = new unsigned short[static_cast<size_t>(ImageHeight * ImageWidth)];
                                    //AT_ConvertBuffer(Buffer, reinterpret_cast < unsigned char *> (unpackedBuffer), ImageWidth, ImageHeight, ImageStride, L"Mono12Packed", L"Mono16");
                                    ushort[] unpackedBuffer = new ushort[ImageHeight * ImageWidth];
                                    byte[] byteArray = new byte[unpackedBuffer.Length * 2];
                                    Buffer.BlockCopy(unpackedBuffer, 0, byteArray, 0, byteArray.Length);

                                    AndorAPI.ConvertBuffer(buffer, byteArray, ImageWidth, ImageHeight, ImageStride, "Mono16", "Mono12Packed");

                                    Debug.WriteLine("Print out of first 20 pixels");
                                    for (int i = 0; i < 20; i++)
                                    {
                                        Debug.WriteLine(unpackedBuffer[i]);
                                    }
                                    unpackedBuffer = null;
                                }
                                AndorAPI.Command(Hndl, "Acquisition Stop");
                                AndorAPI.Flush(Hndl);
                                UserBuffer = null;
                            }
                            AndorAPI.Close(Hndl);
                        }
                    }
                }
                AndorAPI.FinaliseLibrary();
                AndorAPI.FinaliseUtilityLibrary();
            }
            catch (Exception EX)
            {
                Debug.WriteLine(EX.Message);
            }
        }
        #endregion

        #region Callback
        int Handle;
        int g_iCallbackCount = 0;
        int g_iCallbackContext = 0;
        private int CallBack(int Hndl, string Feature, IntPtr Context)
        {
            g_iCallbackCount++;
            g_iCallbackContext = Marshal.ReadInt32(Context);
            return 0;

        }

        void Main(string[] args)
        {

            //Set the call-back context, context values can be defined on per application basis
            IntPtr i_callbackContext = new IntPtr(5);

            //Reset the call-back count
            //Only required for the purposes of this example to show the call-back has been received
            g_iCallbackCount = 0;

            //Register a call-back for the given feature
            AndorAPI.RegisterFeatureCallback(Handle, "PixelReadoutRate", CallBack, i_callbackContext);

            //Set the feature in order to trigger the call-back
            AndorAPI.SetEnumIndex(Handle, "PixelReadoutRate", 0);

            // Application specific code should go here
            //For this example we shall check that the call-back has been successful
            if (g_iCallbackCount == 0 || g_iCallbackContext != i_callbackContext.ToInt32())
            {
                //Deal with failed call-back
            }

            //Unregister the call-back, no more updates will be received
            AndorAPI.UnregisterFeatureCallback(Handle, "PixelReadoutRate", CallBack, i_callbackContext);

        }

        #endregion

        /// <summary>
        /// Ensure a low noise level in the images
        /// </summary>
        /// <returns></returns>
        private bool SetsSensorCooling()
        {
            double temperature = 0;
            AndorAPI.SetBool(Hndl, "SensorCooling", true);
            int temperatureCount = 0;
            AndorAPI.GetEnumCount(Hndl, "TemperatureControl", ref temperatureCount);
            AndorAPI.SetEnumIndex(Hndl, "TemperatureControl", temperatureCount - 1);
            int temperatureStatusIndex = 0;
            StringBuilder temperatureStatus = new StringBuilder(256);
            do
            {
                AndorAPI.GetEnumIndex(Hndl, "TemperatureStatus",ref temperatureStatusIndex);
                AndorAPI.GetEnumStringByIndex(Hndl, "TemperatureStatus", temperatureStatusIndex,
                temperatureStatus, 256);
            }
            while (string.Compare("Stabilised", temperatureStatus.ToString()) != 0);

            return true;
        }

        

        #endregion
    }
}

