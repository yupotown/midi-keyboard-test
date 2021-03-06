﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MidiKeyboardTest
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WmDeviceChange)
            {
                new Thread(new ThreadStart(detectDevice)).Start();
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        private const int WmDeviceChange = 0x0219;

        private IntPtr midiIn;
        private Win32Midi.MidiInOpenCallback callbackDelegate;
        private IntPtr callbackPtr;

        private void FormMain_Load(object sender, EventArgs e)
        {
            callbackDelegate = new Win32Midi.MidiInOpenCallback(callback);
            callbackPtr = (IntPtr)Marshal.GetFunctionPointerForDelegate(callbackDelegate);
            new Thread(new ThreadStart(detectDevice)).Start();
        }

        private delegate void addLogDelegate(string line);

        private void addLog(string line)
        {
            textBoxDebug.Text += line + Environment.NewLine;
        }

        private Mutex mutexDetectDevice = new Mutex();

        private void detectDevice()
        {
            mutexDetectDevice.WaitOne();
            var devNum = Win32Midi.MidiInGetNumDevs();
            if (devNum != 0)
            {
                var res = Win32Midi.MidiInOpen(ref midiIn, 0, callbackPtr, (IntPtr)0, Win32Midi.CallbackFunction);
                if (res == Win32Midi.MMSysErrNoError)
                {
                    res = Win32Midi.MidiInStart(midiIn);
                }
                if (res != Win32Midi.MMSysErrNoError)
                {
                    var sb = new StringBuilder();
                    sb.Capacity = 1024;
                    Win32Midi.MidiInGetErrorText(res, sb, (uint)sb.Capacity);
                    Invoke(new addLogDelegate(addLog), sb.ToString());
                }
            }
            mutexDetectDevice.ReleaseMutex();
        }

        private void callback(IntPtr hdrvr, uint msg, uint user, uint dw1, uint dw2)
        {
            var state = (int)dw1 & 0xff;
            var data1 = (int)dw1 >> 8 & 0xff;
            var data2 = (int)dw1 >> 16 & 0xff;
            Invoke(new addLogDelegate(addLog), string.Format("{0}\t{1}\t{2}", state, data1, data2));
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Win32Midi.MidiInStop(midiIn);
            Win32Midi.midiInClose(ref midiIn);
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBoxDebug.Text = "";
        }
    }
}
