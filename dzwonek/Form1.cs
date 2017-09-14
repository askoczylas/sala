using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public int show = 1;

        private void Form1_Load(object sender, EventArgs e)
        {
            notifyIcon1.Text = "ddd";
            ShowText("1", new Font("Tahoma", 10, FontStyle.Regular), Color.Black);
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.RunWorkerAsync();
        }

       
        private void  GetMulticastFrame()
        {
   //Configuration
            var interfaceIp = IPAddress.Parse("192.168.1.143");
            var interfaceEndPoint = new IPEndPoint(interfaceIp, 3671);
            var multicastIp = IPAddress.Parse("224.0.23.12");
            var multicastEndPoint = new IPEndPoint(multicastIp, 3671);

            //initialize the socket
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.ExclusiveAddressUse = false;
            socket.MulticastLoopback = false;
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            MulticastOption option = new MulticastOption(multicastEndPoint.Address, interfaceIp);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, option);

            //bind on a network interface
            socket.Bind(interfaceEndPoint);

            //initialize args for sending packet on the multicast channel
            var sockArgs = new SocketAsyncEventArgs();
            sockArgs.RemoteEndPoint = multicastEndPoint;
            sockArgs.SetBuffer(new byte[1234], 0, 1234);

            //send an empty packet of size 1234 every 3 seconds
            while (true)
            {
               // socket.SendToAsync(sockArgs);
                byte[] b = new byte[1024];

                socket.Receive (b);
                show = 1;
               
                Thread.Sleep(3000);
            }
            //byte[] b = new byte[1024];
            //s.Receive(b);
            //string str = System.Text.Encoding.ASCII.GetString(b, 0, b.Length);
            //Console.WriteLine(str.Trim());

        }

        public void ShowText(string text, Font font, Color col)
        {

            Brush brush = new SolidBrush(col);



            // Create a bitmap and draw text on it

            Bitmap bitmap = new Bitmap(16, 16);
            Font m_font = new Font("Tahoma",15,FontStyle.Regular);

            Graphics graphics = Graphics.FromImage(bitmap);

            graphics.DrawString(text, m_font, brush, 0, 0);



            // Convert the bitmap with text to an Icon

            IntPtr hIcon = bitmap.GetHicon();

            Icon icon = Icon.FromHandle(hIcon);

            notifyIcon1.Icon = icon;

        }
        
        private void GetFrame()
        {
            string sourceUrl = "http://192.168.0.56/jpg/image.jpg";
            byte[] buffer = new byte[1280 * 800];
            int read, total = 0;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sourceUrl);
            req.Credentials = new NetworkCredential("admin", "1234");
            
            WebResponse resp = req.GetResponse();

            Stream stream = resp.GetResponseStream();

            while ((read=stream.Read(buffer,total,1000)) !=0)
            {
                total+=read;
            }
            Bitmap bmp = (Bitmap)Bitmap.FromStream(new MemoryStream(buffer, 0, total));

            pictureBox1.Image = bmp;

        
        
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {

              //  GetMulticastFrame();

             if (show==1) GetFrame();

                Thread.Sleep(500);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                show = 0;
                Hide();
            };
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            GetMulticastFrame();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = show.ToString();
            timer1.Enabled = false;
            if (show == 1)
            {
                Show();
                this.WindowState = FormWindowState.Normal;
            };

            timer1.Enabled = true;

        }
    
    
    }
}
