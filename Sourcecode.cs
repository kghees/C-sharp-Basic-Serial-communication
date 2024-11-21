using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _240907Serial
{
    public partial class Serial_Communication : Form
    {
        public Serial_Communication()
        {
            InitializeComponent();
        }

        void addMsg(string x)
        {
            //BeginInvokr 현재 UI 스레드에서 안전하게 작업을 처리하도록 하는 메서드
            //Cross Thread 충돌 Error 방지해줌
            this.BeginInvoke(new Action(() =>
            {
                //RichTextBox에 메시지를 띄운다.
                rtBox.AppendText(x + '\n');
                //새롭게 추가된 메시지가 화면에 보이도록 스크롤을 이동
                rtBox.ScrollToCaret();
            }));
        }

        private void btn_open_Click(object sender, EventArgs e)
        {
            //시리얼 포트가 열려 있지 않다면
            if(this.serialPort1.IsOpen == false)
            {
                //tbPort에 적혀 있는 번호로 포트 번호 설정
                this.serialPort1.PortName = tbPort.Text;
                //tbBaud에 적혀 있는 값을 정수로 바꾸고 속도 설정
                this.serialPort1.BaudRate = int.Parse(tbBaud.Text);
                //시리얼 포트를 연다
                this.serialPort1.Open();
                //시리얼 포트가 열렸으므로 열렸다고 RichTextBox에 띄워주기
                addMsg("Open");
            }
            //시리얼 포트가 열려 있다면
            else
            {
                //이미 열려있다고 띄우기
                addMsg("Already Open");
            }
        }

        //시리얼 포트에서 데이터가 수신될 때 발생하는 데이터 수신 이벤트 핸들러
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            //시리얼 포트에서 읽을 수 있는 바이트 수 저장(데이터의 크기)
            var readcnt = serialPort1.BytesToRead;
            //읽을 데이터를 저장할 바이트 배열 생성
            var buffer = new byte[readcnt];
            //시리얼 포트에서 데이터 읽어와서 배열에 저장(readcnt 만큼 읽어옴)
            this.serialPort1.Read(buffer, 0, readcnt);
            //수신한 바이트 배열을 UTF-8문자열로 변환
            var recvmsg = System.Text.Encoding.UTF8.GetString(buffer);
            addMsg("<" + recvmsg);
        }

        private void btn_send_Click(object sender, EventArgs e)
        {
            //tbMSg에 입력된 데이터를 UTF-8형식의 바이트 배열로 변환
            var sendmsg = System.Text.Encoding.UTF8.GetBytes(tbMsg.Text);
            //변환된 배열을 시리얼 포트로 전송
            this.serialPort1.Write(sendmsg, 0, sendmsg.Length);
            addMsg(">" +  sendmsg);
            tbMsg.Clear();
        }

       
        private void tbMsg_KeyDown(object sender, KeyEventArgs e)
        {
            //엔터치면 바로 데이터 전송되게끔
            if(e.KeyCode == Keys.Enter)
            {
                //tbMSg에 입력된 데이터를 UTF-8형식의 바이트 배열로 변환
                var sendmsg = System.Text.Encoding.UTF8.GetBytes(tbMsg.Text);
                //변환된 배열을 시리얼 포트로 전송
                this.serialPort1.Write(sendmsg, 0, sendmsg.Length);
                addMsg(">" + sendmsg);
                tbMsg.Clear();
            }
        }
    }
}
