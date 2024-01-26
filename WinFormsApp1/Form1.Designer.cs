namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnLaser = new Button();
            btnLive = new Button();
            btnAcquisition = new Button();
            btnGetSerialNumber = new Button();
            btnReconnectduringAcquisition = new Button();
            btnConverBuffer = new Button();
            btnCameraTest = new Button();
            btnStage = new Button();
            btnStageMove = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            button1 = new Button();
            button6 = new Button();
            button7 = new Button();
            button8 = new Button();
            button9 = new Button();
            button2 = new Button();
            button3 = new Button();
            btnSetting = new Button();
            btnUnInitialize = new Button();
            textBox1 = new TextBox();
            btnEnumTest = new Button();
            btnStopCommand = new Button();
            btnAcqStartCommand = new Button();
            btnSensorCooling = new Button();
            textBox2 = new TextBox();
            SuspendLayout();
            // 
            // btnLaser
            // 
            btnLaser.Location = new Point(56, 29);
            btnLaser.Name = "btnLaser";
            btnLaser.Size = new Size(75, 46);
            btnLaser.TabIndex = 0;
            btnLaser.Text = "Laser";
            btnLaser.UseVisualStyleBackColor = true;
            btnLaser.Click += btnLaser_Click;
            // 
            // btnLive
            // 
            btnLive.Location = new Point(27, 219);
            btnLive.Name = "btnLive";
            btnLive.Size = new Size(193, 23);
            btnLive.TabIndex = 0;
            btnLive.Text = "bwLive";
            btnLive.UseVisualStyleBackColor = true;
            btnLive.Click += btnLive_Click;
            // 
            // btnAcquisition
            // 
            btnAcquisition.Location = new Point(27, 265);
            btnAcquisition.Name = "btnAcquisition";
            btnAcquisition.Size = new Size(193, 23);
            btnAcquisition.TabIndex = 0;
            btnAcquisition.Text = "Acquisition";
            btnAcquisition.UseVisualStyleBackColor = true;
            btnAcquisition.Click += btnAcquisition_Click;
            // 
            // btnGetSerialNumber
            // 
            btnGetSerialNumber.Location = new Point(27, 306);
            btnGetSerialNumber.Name = "btnGetSerialNumber";
            btnGetSerialNumber.Size = new Size(193, 23);
            btnGetSerialNumber.TabIndex = 0;
            btnGetSerialNumber.Text = "GetSerialNumber";
            btnGetSerialNumber.UseVisualStyleBackColor = true;
            btnGetSerialNumber.Click += btnGetSerialNumber_Click;
            // 
            // btnReconnectduringAcquisition
            // 
            btnReconnectduringAcquisition.Enabled = false;
            btnReconnectduringAcquisition.Location = new Point(27, 350);
            btnReconnectduringAcquisition.Name = "btnReconnectduringAcquisition";
            btnReconnectduringAcquisition.Size = new Size(193, 29);
            btnReconnectduringAcquisition.TabIndex = 0;
            btnReconnectduringAcquisition.Text = "ReconnectduringAcquisition";
            btnReconnectduringAcquisition.UseVisualStyleBackColor = true;
            btnReconnectduringAcquisition.Click += btnReconnectduringAcquisition_Click;
            // 
            // btnConverBuffer
            // 
            btnConverBuffer.Location = new Point(27, 400);
            btnConverBuffer.Name = "btnConverBuffer";
            btnConverBuffer.Size = new Size(193, 23);
            btnConverBuffer.TabIndex = 0;
            btnConverBuffer.Text = "ConverBuffer";
            btnConverBuffer.UseVisualStyleBackColor = true;
            btnConverBuffer.Click += btnConverBuffer_Click;
            // 
            // btnCameraTest
            // 
            btnCameraTest.Location = new Point(27, 180);
            btnCameraTest.Name = "btnCameraTest";
            btnCameraTest.Size = new Size(193, 23);
            btnCameraTest.TabIndex = 0;
            btnCameraTest.Text = "CameraTest";
            btnCameraTest.UseVisualStyleBackColor = true;
            btnCameraTest.Click += btnCameraTest_Click;
            // 
            // btnStage
            // 
            btnStage.Location = new Point(27, 97);
            btnStage.Name = "btnStage";
            btnStage.Size = new Size(83, 46);
            btnStage.TabIndex = 0;
            btnStage.Text = "StageConc";
            btnStage.UseVisualStyleBackColor = true;
            btnStage.Click += btnStage_Click;
            // 
            // btnStageMove
            // 
            btnStageMove.Location = new Point(133, 97);
            btnStageMove.Name = "btnStageMove";
            btnStageMove.Size = new Size(92, 46);
            btnStageMove.TabIndex = 0;
            btnStageMove.Text = "StageMove";
            btnStageMove.UseVisualStyleBackColor = true;
            btnStageMove.Click += btnStageMove_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(245, 70);
            label1.Name = "label1";
            label1.Size = new Size(20, 21);
            label1.TabIndex = 1;
            label1.Text = "X";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(245, 108);
            label2.Name = "label2";
            label2.Size = new Size(20, 21);
            label2.TabIndex = 1;
            label2.Text = "Y";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(245, 150);
            label3.Name = "label3";
            label3.Size = new Size(20, 21);
            label3.TabIndex = 1;
            label3.Text = "Z";
            // 
            // button1
            // 
            button1.Location = new Point(468, 106);
            button1.Name = "button1";
            button1.Size = new Size(127, 23);
            button1.TabIndex = 0;
            button1.Text = "Initialize";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Init_Click;
            // 
            // button6
            // 
            button6.Location = new Point(468, 191);
            button6.Name = "button6";
            button6.Size = new Size(127, 23);
            button6.TabIndex = 0;
            button6.Text = "StopCapture";
            button6.UseVisualStyleBackColor = true;
            button6.Click += StopCapture_Click;
            // 
            // button7
            // 
            button7.Location = new Point(468, 162);
            button7.Name = "button7";
            button7.Size = new Size(127, 23);
            button7.TabIndex = 0;
            button7.Text = "StartCapture";
            button7.UseVisualStyleBackColor = true;
            button7.Click += StartCapture_Click;
            // 
            // button8
            // 
            button8.Location = new Point(623, 106);
            button8.Name = "button8";
            button8.Size = new Size(127, 23);
            button8.TabIndex = 0;
            button8.Text = "Save";
            button8.UseVisualStyleBackColor = true;
            button8.Click += Save_Click;
            // 
            // button9
            // 
            button9.Location = new Point(623, 135);
            button9.Name = "button9";
            button9.Size = new Size(127, 23);
            button9.TabIndex = 0;
            button9.Text = "Capture";
            button9.UseVisualStyleBackColor = true;
            button9.Click += Capture_Click;
            // 
            // button2
            // 
            button2.Location = new Point(623, 193);
            button2.Name = "button2";
            button2.Size = new Size(127, 23);
            button2.TabIndex = 0;
            button2.Text = "Set";
            button2.UseVisualStyleBackColor = true;
            button2.Click += CaptureSet_Click;
            // 
            // button3
            // 
            button3.Location = new Point(623, 164);
            button3.Name = "button3";
            button3.Size = new Size(127, 23);
            button3.TabIndex = 0;
            button3.Text = "Reset";
            button3.UseVisualStyleBackColor = true;
            button3.Click += CaptureReset_Click;
            // 
            // btnSetting
            // 
            btnSetting.Location = new Point(468, 220);
            btnSetting.Name = "btnSetting";
            btnSetting.Size = new Size(127, 36);
            btnSetting.TabIndex = 0;
            btnSetting.Text = "Setting";
            btnSetting.UseVisualStyleBackColor = true;
            btnSetting.Click += Setting_Click;
            // 
            // btnUnInitialize
            // 
            btnUnInitialize.Location = new Point(468, 135);
            btnUnInitialize.Name = "btnUnInitialize";
            btnUnInitialize.Size = new Size(127, 23);
            btnUnInitialize.TabIndex = 0;
            btnUnInitialize.Text = "UnInitialize";
            btnUnInitialize.UseVisualStyleBackColor = true;
            btnUnInitialize.Click += btnUnInitialize_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(601, 227);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(60, 23);
            textBox1.TabIndex = 2;
            textBox1.Text = "3000";
            // 
            // btnEnumTest
            // 
            btnEnumTest.Location = new Point(661, 12);
            btnEnumTest.Name = "btnEnumTest";
            btnEnumTest.Size = new Size(127, 36);
            btnEnumTest.TabIndex = 0;
            btnEnumTest.Text = "EnumTest";
            btnEnumTest.UseVisualStyleBackColor = true;
            btnEnumTest.Click += btnEnumTest_Click;
            // 
            // btnStopCommand
            // 
            btnStopCommand.Location = new Point(506, 321);
            btnStopCommand.Name = "btnStopCommand";
            btnStopCommand.Size = new Size(127, 23);
            btnStopCommand.TabIndex = 0;
            btnStopCommand.Text = "StopCommand";
            btnStopCommand.UseVisualStyleBackColor = true;
            btnStopCommand.Click += btnStopCommand_Click;
            // 
            // btnAcqStartCommand
            // 
            btnAcqStartCommand.Location = new Point(506, 292);
            btnAcqStartCommand.Name = "btnAcqStartCommand";
            btnAcqStartCommand.Size = new Size(127, 23);
            btnAcqStartCommand.TabIndex = 0;
            btnAcqStartCommand.Text = "StartCommand";
            btnAcqStartCommand.UseVisualStyleBackColor = true;
            btnAcqStartCommand.Click += btnAcqStartCommand_Click;
            // 
            // btnSensorCooling
            // 
            btnSensorCooling.Location = new Point(506, 12);
            btnSensorCooling.Name = "btnSensorCooling";
            btnSensorCooling.Size = new Size(127, 36);
            btnSensorCooling.TabIndex = 0;
            btnSensorCooling.Text = "SensorCooling";
            btnSensorCooling.UseVisualStyleBackColor = true;
            btnSensorCooling.Click += btnSensorCooling_Click;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(354, 181);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(100, 23);
            textBox2.TabIndex = 3;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnReconnectduringAcquisition);
            Controls.Add(btnGetSerialNumber);
            Controls.Add(btnAcquisition);
            Controls.Add(btnConverBuffer);
            Controls.Add(btnSensorCooling);
            Controls.Add(btnEnumTest);
            Controls.Add(btnSetting);
            Controls.Add(btnAcqStartCommand);
            Controls.Add(btnStopCommand);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button9);
            Controls.Add(button8);
            Controls.Add(button7);
            Controls.Add(button6);
            Controls.Add(btnUnInitialize);
            Controls.Add(button1);
            Controls.Add(btnCameraTest);
            Controls.Add(btnLive);
            Controls.Add(btnStageMove);
            Controls.Add(btnStage);
            Controls.Add(btnLaser);
            Name = "Form1";
            Text = "Form1";
            FormClosing += Form1_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnLaser;
        private Button btnLive;
        private Button btnAcquisition;
        private Button btnGetSerialNumber;
        private Button btnReconnectduringAcquisition;
        private Button btnConverBuffer;
        private Button btnCameraTest;
        private Button btnStage;
        private Button btnStageMove;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button button1;
        private Button button6;
        private Button button7;
        private Button button8;
        private Button button9;
        private Button button2;
        private Button button3;
        private Button btnSetting;
        private Button btnUnInitialize;
        private TextBox textBox1;
        private Button btnEnumTest;
        private Button btnStopCommand;
        private Button btnAcqStartCommand;
        private Button btnSensorCooling;
        private TextBox textBox2;
    }
}
