using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace SpeechToText
{
    public class Audio
    {
        public event EventHandler eventStopRecording;

        private WaveIn sourceStream;
        private WaveFileWriter waveWriter;
        private string FilePath = System.Windows.Forms.Application.StartupPath + "\\";
        private string FileName = "audio.wav";
        int InputDeviceIndex = 0;
        int counter = 0;

        public void StartRecording()
        {
            sourceStream = new WaveIn
            {
                DeviceNumber = this.InputDeviceIndex,
                WaveFormat = new WaveFormat(41000, 2)
            };

            sourceStream.DataAvailable += this.SourceStreamDataAvailable;

            if (!System.IO.Directory.Exists(FilePath))
            {
                System.IO.Directory.CreateDirectory(FilePath);
            }

            waveWriter = new WaveFileWriter(FilePath + FileName, sourceStream.WaveFormat);

                                                                                                                                                                                                                                                                                                                                             sourceStream.StartRecording();
            sw.Reset();
            sw.Start();
        }

        public void StopRecording()
        {
            if (sourceStream != null)
            {
                sourceStream.StopRecording();
                sourceStream.Dispose();
                sourceStream = null;
            }
            if (this.waveWriter == null)
            {
                return;
            }
            this.waveWriter.Dispose();
            this.waveWriter = null;
        }

        private System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        private int mCounter = 0;
        private void SourceStreamDataAvailable(object sender, WaveInEventArgs e)
        {
            NAudio.CoreAudioApi.MMDeviceEnumerator MMDE = new NAudio.CoreAudioApi.MMDeviceEnumerator();
            NAudio.CoreAudioApi.MMDeviceCollection DevCol = MMDE.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.All, NAudio.CoreAudioApi.DeviceState.Active);
            NAudio.CoreAudioApi.MMDevice device = DevCol[3];

            double volume = Math.Round(device.AudioMeterInformation.MasterPeakValue * 100, 1);
            Console.WriteLine(volume.ToString());
            if (waveWriter == null) return;

            if (volume < 1)
            {
                counter++;
            }
            else
            {
                waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
                waveWriter.Flush();
                counter = 0;
                mCounter++;
            }

            if (counter > 30)
            {
                long a1 = sw.ElapsedMilliseconds;
                int c = mCounter;

                counter = 0;
                sourceStream.DataAvailable -= this.SourceStreamDataAvailable;
                StopRecording();
                eventStopRecording(this, new EventArgs());
            }
        }

        //public void Recording()
        //{
        //    string audioFile = "sound.wav";
        //    WasapiLoopbackCapture captureInstance = new WasapiLoopbackCapture();
        //    WaveFileWriter writer = new WaveFileWriter(audioFile, captureInstance.WaveFormat);

        //    captureInstance.DataAvailable += (s, a) =>
        //    {
        //        // Write buffer into the file of the writer instance
        //        writer.Write(a.Buffer, 0, a.BytesRecorded);
        //    };

        //    // When the Capturer Stops, dispose instances of the capturer and writer
        //    captureInstance.RecordingStopped += (s, a) =>
        //    {
        //        writer.Dispose();
        //        writer = null;
        //        captureInstance.Dispose();
        //    };

        //    // Start audio recording !
        //    captureInstance.StartRecording();
        //}

        //[STAThread]
        //public void GetVoice()
        //{
        //    WaveIn waveIn = new WaveIn();
        //    waveIn.WaveFormat = new WaveFormat(44100, 2);
        //    int num = waveIn.DeviceNumber;

        //    waveIn.BufferMilliseconds = 1000;
        //    waveIn.DataAvailable += new EventHandler<WaveInEventArgs>(SendCaptureSamples);
        //    waveIn.StartRecording();
        //}

        //static void SendCaptureSamples(object sender, WaveInEventArgs e)
        //{
        //    Console.WriteLine("Bytes recorded: {0}", e.BytesRecorded);
        //}
    }
} 
