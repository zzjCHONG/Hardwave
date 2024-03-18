using Simscop.Hardware.Stage;
using Simscop.API;
using OpenCvSharp;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System;
using Hardwave.Laser;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region Camera
        private void btnLive_Click(object sender, EventArgs e)
        {
            AndorModule andorModule = new AndorModule();
            //andorModule.bwLive();
            andorModule.GetLiveImage();
        }

        private void btnReconnectduringAcquisition_Click(object sender, EventArgs e)
        {
            AndorModule andorModule = new AndorModule();
            andorModule.ReconnectduringAcquisition();
        }

        private void btnGetSerialNumber_Click(object sender, EventArgs e)
        {
            AndorModule andorModule = new AndorModule();
            andorModule.GetSerialNumber();
        }

        private void btnAcquisition_Click(object sender, EventArgs e)
        {
            AndorModule andorModule = new AndorModule();
            andorModule.Acquisition();
        }

        private void btnConverBuffer_Click(object sender, EventArgs e)
        {
            AndorModule andorModule = new AndorModule();
            andorModule.ConverBuffer();
        }

        private void btnCameraTest_Click(object sender, EventArgs e)
        {
            WorkCamera workCamera = new WorkCamera();
            if (!workCamera.IsValid) workCamera.Valid();
            if (!workCamera.IsValid) return;

            if (!workCamera.IsInit) workCamera.Init();
            workCamera.Connect();
        }

        #endregion

        #region LASER&&STAGE
        WorkState? workState = null;
        private void btnStage_Click(object sender, EventArgs e)
        {
            workState = new WorkState();

            if (workState.IsConnected) workState.Disconnect();
            if (!workState.IsValid) workState.Valid();
            if (!workState.IsValid) return;
            if (!workState.Connect()) return;
            if (!workState.IsInit) workState.Init();
            if (workState.Init()) MessageBox.Show("Stage Connect OK!");

            //workState.AxisChanged += (sender, args) => { Debug.WriteLine("AxisChanged!"); };//使用示例
        }

        private void PosUpdate()
        {
            this.Invoke(new Action(() =>
            {
                label1.Text = workState.Axes[0].Position.ToString();
                label2.Text = workState.Axes[1].Position.ToString();
                label3.Text = workState.Axes[2].Position.ToString();
            }));
            Thread.Sleep(50);
        }

        private void btnStageMove_Click(object sender, EventArgs e)
        {
            workState.Reset();
            workState.SetCenter();

            if (!workState.IsConnected) return;
            StageAxis stageAxisX = new StageAxis() { Id = 1, Alias = "X" };
            StageAxis stageAxisY = new StageAxis() { Id = 2, Alias = "Y" };
            StageAxis stageAxisZ = new StageAxis() { Id = 3, Alias = "Z" };
            workState.SetAbsolutePosition(stageAxisX, 3150.05);
            workState.SetRelativePosition(stageAxisY, 500);
            workState.SetRelativePosition(stageAxisZ, 700);

            PosUpdate();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (workState != null && workState.IsConnected)
                workState.Disconnect();
        }

        private void btnLaser_Click(object sender, EventArgs e)
        {
            WorkLaser workLaser = new WorkLaser();
            try
            {
                if (!workLaser.IsValid) workLaser.Valid();
                if (!workLaser.IsValid) return;
                //if (workLaser.IsConnected) workLaser.Disconnect();
                if (!workLaser.IsInit) workLaser.Init();//TRUE
                workLaser.Connect();

                workLaser.Channels = new List<Simscop.Hardware.Laser.Channel>();
                Simscop.Hardware.Laser.Channel channel405 = new Simscop.Hardware.Laser.Channel() { WaveLength = 405 };
                Simscop.Hardware.Laser.Channel channel488 = new Simscop.Hardware.Laser.Channel() { WaveLength = 488 };
                workLaser.Channels.Add(channel488);
                workLaser.SetPower(channel488, 4095);
                workLaser.LightOn(channel488);
                workLaser.AsyncStatus();
                workLaser.LightOff(channel488);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                workLaser.Disconnect();
            }
        }

        #endregion

        Andor _andorCamera = new Andor();
        private void Init_Click(object sender, EventArgs e)
        {
            _andorCamera.Init();
        }

        private void btnUnInitialize_Click(object sender, EventArgs e)
        {
            andorImplemented.UnInitializeCamera();
            andorImplemented.UninitializeSdk();
        }

        private void StartCapture_Click(object sender, EventArgs e)
        {
            _andorCamera.StartCapture();
        }

        private void StopCapture_Click(object sender, EventArgs e)
        {
            _andorCamera.StopCapture();
        }

        ManualResetEvent m = new ManualResetEvent(true);//线程阻断
        CancellationTokenSource tokensource = new CancellationTokenSource();
        private void Capture_Click(object sender, EventArgs e)
        {

            //Test2
            CancellationToken cancellationToken = tokensource.Token;
            Task.Run(() =>
            {
                while (true)
                {

                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return false;

                        m.WaitOne();

                        if (!_andorCamera.Capture(out Mat? matImg)) return false;

                        Thread.Sleep(5);

                    }
                    catch (Exception)
                    {
                    }

                }
            });

            ////Test3
            //_andorCamera.Capture(out Mat matImg);
        }

        private void Save_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "AndorImage"));
            _andorCamera.SaveCapture(path);
        }

        private void CaptureReset_Click(object sender, EventArgs e)
        {
            m.Reset();
        }

        private void CaptureSet_Click(object sender, EventArgs e)
        {
            m.Set();
        }

        AndorImplemented andorImplemented = new AndorImplemented();
        private void Setting_Click(object sender, EventArgs e)
        {

            _andorCamera.GetExposure(out double ex);

            _andorCamera.SetExposure(Convert.ToInt32(textBox1.Text));

        }

        private void btnAcqStartCommand_Click(object sender, EventArgs e)
        {
            andorImplemented.AcqStartCommand();
        }

        private void btnStopCommand_Click(object sender, EventArgs e)
        {
            andorImplemented.AcqStopCommand();
        }

        private void btnEnumTest_Click(object sender, EventArgs e)
        {
            andorImplemented.EnumSettingDemo();
            //andorImplemented.LoopGetEnum();
        }

        private void btnSensorCooling_Click(object sender, EventArgs e)
        {
            andorImplemented.SetsSensorCooling();
        }
    }
}
