using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Net.Http;

namespace SpeechToText
{
    public partial class FormMain : Form
    {
        private Audio audio = new Audio();
        private enum STATUS { BUNDLE_NO, WAREHOUSING_CNT, LOCATION, FINISH }
        private STATUS mCurStatus = STATUS.FINISH;

        public FormMain()
        {
            InitializeComponent();
            audio.eventStopRecording += new EventHandler(StopRecording);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //mCurStatus = STATUS.BUNDLE_NO;
            //StartRecording();
            //lbStatus.Text = "작업지시번호를 말해주세요";


            //request();
            Post();

        }

        private void Post()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:3000/api/courses");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(new result("4444444", "99"));
                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                Console.WriteLine(result.ToString());
            }
        }

        private void Get()
        {
            WebClient client = new WebClient();

            //특정 요청 헤더값을 추가해준다. 

            using (Stream data = client.OpenRead("http://localhost:3000/api/courses/1"))
            {
                using (StreamReader reader = new StreamReader(data))
                {
                    string s = reader.ReadToEnd();

                    //string jsonString = JsonSerializer.Serialize(weatherForecast);

                    JObject jObject = JObject.Parse(s);
                    result result = JsonConvert.DeserializeObject<result>(jObject["result"].ToString());
                    reader.Close();
                    data.Close();
                }
            }
        }

        private void Put()
        {

        }

        private void Delete()
        {

        }

        class result
        {
            public string no { get; set; }
            public string skelp { get; set; }

            public result(string no, string skelp)
            {
                this.no = no;
                this.skelp = skelp;
            }
        }


        private void AddDataToListBox1(string strData)
        {
            // 현재 시간
            DateTime cur_time = DateTime.Now;
            listBox1.Items.Insert(0, cur_time.ToString("yyyy-MM-dd HH:mm:ss") + " : " + strData);
            listBox1.SetSelected(listBox1.Items.Count - 1, false);
            if (listBox1.Items.Count > 100) listBox1.Items.Clear();
        }

        private void StartRecording()
        {
            audio.StartRecording();
            lbStatus.Visible = true;
        }

        private void StopRecording(object sender, EventArgs e)
        {
            STT stt = new STT();
            string data = stt.Trans();
            AddDataToListBox1(data);

            lbStatus.Visible = false;

            switch (mCurStatus)
            {
                case STATUS.BUNDLE_NO:
                    mCurStatus = STATUS.WAREHOUSING_CNT;
                    SetData(txtBundleNo, data);
                    lbStatus.Text = "작업지시순번을 말해주세요";
                    StartRecording();
                    break;
                case STATUS.WAREHOUSING_CNT:
                    mCurStatus = STATUS.LOCATION;
                    SetData(txtWarehousingCount, data);
                    lbStatus.Text = "SKELP 수량을 말해주세요";
                    StartRecording();
                    break;
                case STATUS.LOCATION:
                    mCurStatus = STATUS.FINISH;
                    SetData(txtLocation, data);
                    lbStatus.Text = "취소하려면 취소라고 말해주세요";
                    StartRecording();
                    break;
                case STATUS.FINISH:
                    SetConfirmkData(data);
                    break;
                default:
                    break;
            }
        }

        private void SetData(TextBox textBox, string data)
        {
            textBox.Text = data;
            AddDataToListBox1(data);
        }

        private void SetOrderData(string data)
        {
            string[] splitData = data.Split(new string[] { "그리고" }, StringSplitOptions.RemoveEmptyEntries);
            if(splitData.Length >= 3)
            {
                txtBundleNo.Text = splitData[0];
                txtWarehousingCount.Text = splitData[1];
                txtLocation.Text = splitData[2];
                AddDataToListBox1("틀리면 '취소' 라고 말씀해주시고 제대로 입력되었을 경우 3초간 기다리면 저장됩니다.");
                StartRecording();
            }
            else
            {
                AddDataToListBox1("다시한번 말씀해주세요 (" + data + ")");
            }
        }

        private void SetConfirmkData(string data)
        {
            if(data.Contains("취소"))
            {
                txtBundleNo.Focus();
            }
            else
            {
                ListViewItem item;
                item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd"));
                item.SubItems.Add(txtBundleNo.Text);
                item.SubItems.Add(txtWarehousingCount.Text);
                item.SubItems.Add(txtLocation.Text);
                listView1.Items.Insert(0, item);
            }
        }

        private void ClearControls()
        {
            txtBundleNo.Text = "";
            txtLocation.Text = "";
            txtWarehousingCount.Text = "";
        }
    }
}
