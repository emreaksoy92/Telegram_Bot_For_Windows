using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram_Bot.Request;
using Google.Maps.Geocoding;
using Google.Maps;
using System.Threading;

namespace Telegram_Bot
{
    public partial class Form1 : Form
    {
        int ChatID = -8513858;
        //int ChatID = 58811658;

        delegate void SetTextCallBack(string text);
        TelegramRequest Tr = new TelegramRequest(Properties.Settings.Default.Token);
        Method m = new Method(Properties.Settings.Default.Token);

        public Form1()
        {                     
            InitializeComponent();
            Tr.MessageText += Tr_MessageText;
            Tr.MessageSticker += Tr_MessageSticker;
            Tr.MessagePhoto += Tr_MessagePhoto;
            Tr.MessageVideo += Tr_MessageVideo;
            Tr.MessageDocument += Tr_MessageDocument;
            Tr.MessageLocation += Tr_MessageLocation;
            Tr.MessageContact += Tr_MessageContact;
            Tr.MessageVoice += Tr_MessageVoice;
            //Tr.GetUpdates();


            void Tr_MessageText(object sendr, MessageText e)
            {
                //Console.WriteLine("Message ID: {0} \n Message From ID={1} \n Username={2} \n First Name={3} \n Last Name={4} \n Date={5} \n Text={6}",
                //    e.message_id, e.from.id, e.from.username, e.from.first_name, e.from.last_name, e.date, e.text);
                if (e.text.StartsWith("/location") == true) ;
                {
                    string adress = e.text.ToString();
                    adress.Split(' ');
                    GoogleSigned.AssignAllServices(new GoogleSigned(Properties.Settings.Default.GoogleToken));

                    var request = new GeocodingRequest
                    {
                        Address = adress,
                        Sensor = false
                    };
                    System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
                    customCulture.NumberFormat.NumberDecimalSeparator = ".";
                    System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
                    var response = new GeocodingService().GetResponse(request);
                    var result = response.Results.First();
                    float log = Convert.ToSingle(result.Geometry.Location.Longitude);
                    float lan = Convert.ToSingle(result.Geometry.Location.Latitude);
                    txtLan.Text = lan.ToString();
                    txtLon.Text = log.ToString();

                    m.SendLocation(ChatID, lan, log);
                }

                SetText(e.text.ToString());
                
            }
            void Tr_MessageSticker(object sendr, MessageSticker e)
            {

                //Console.WriteLine("Message ID: {0} \n Message From ID={1} \n Username={2} \n First Name={3} \n Last Name={4} \n Date={5} \n Width:{6}  \n Height:{7}\n Emoji:{8} \n Thumb File ID:{9} \n Thumb File Size:{10} \n Thumb Width:{11} \n Thumb Height:{12} \n File ID:{13} \n File Size:{14}",
                //    e.message_id, e.from.id, e.from.username, e.from.first_name, e.from.last_name, e.date, e.width, e.height, e.emoji, e.thumb.file_id, e.thumb.file_size, e.thumb.width, e.thumb.height, e.file_id, e.file_size);
                txt_messageRecieved.Text = e.emoji.ToString();
            }
            void Tr_MessagePhoto(object sendr, MessagePhoto e)
            {
                Console.WriteLine("Message ID: {0} \n Message From ID={1} \n Username={2} \n First Name From={3} \n Last Name From={4} \n Date={5} \n  Caption:{6}",
                    e.message_id, e.from.id, e.from.username, e.from.first_name, e.from.last_name, e.date, e.caption);

                for (int i = 0; i < e.photo.Count; i++)
                {
                    Console.WriteLine("Photo No:{0}", i + 1);
                    Console.WriteLine("ID Photo File ID:{0}\n Photo Size:{1} \nPhoto Width:{2} \n Photo Height:{3}",
                        e.photo[i].file_id, e.photo[i].file_size, e.photo[i].width, e.photo[i].height);
                }
                Method m = new Method(Properties.Settings.Default.Token);
                m.SendMessage("Send Me a Photo!", ChatID);
                m.SendPhotoLink(ChatID, e.photo[e.photo.Count - 1].file_id, e.caption);
            }
            void Tr_MessageVideo(object sendr, MessageVideo e)
            {
                Console.WriteLine("Message ID: {0} \n Message From ID={1} \n Username={2} \n First Name From={3} \n Last Name From={4} \n Date={5} \n File ID:{6} \n Width:{7} \n Height:{8} \n Duration:{9} \n File Size:{10} \n File ID:{11} \n Thumb Width:{12} \n Thumb Height:{13} \n Thumb Size:{14} \n Mime Type:{15}",
                     e.message_id, e.from.id, e.from.username, e.from.first_name, e.from.last_name, e.date, e.file_id, e.width, e.height, e.duration, e.file_size, e.thumb.file_id, e.thumb.width, e.thumb.height, e.thumb.file_size, e.mime_type);
                Console.WriteLine();
            }
            void Tr_MessageDocument(object sendr, MessageDocument e)
            {
                Console.WriteLine("Message ID: {0} \n Message From ID={1} \n Username={2} \n First Name ={3} \n Last Name ={4} \n Date={5} \n File Name:{6} \n Mime Type:{7}\n File ID:{8}\n File Size:{9}mb",
                    e.message_id, e.from.id, e.from.username, e.from.first_name, e.from.last_name, e.date, e.file_name, e.mime_type, e.file_id, e.file_size);
                Console.WriteLine("Thumb File ID:{0} \n Thumb File Size:{1} \n Thumb Width:{2} \n Thumb Height:{3}]", e.thumb.file_id, e.thumb.file_size, e.thumb.width, e.thumb.height);
                Console.WriteLine();
            }
            void Tr_MessageLocation(object sendr, MessageLocation e)
            {
                Console.WriteLine("Message ID: {0} \n Message From ID={1} \n Username={2} \n First Name ={3} \n Last Name ={4} \n Date={5} \n Latitude:{6} \n Longitude:{7}",
                    e.message_id, e.from.id, e.from.username, e.from.first_name, e.from.last_name, e.date, e.latitude, e.longitude);
                Console.WriteLine();
            }
            void Tr_MessageContact(object sendr, MessageContact e)
            {
                Console.WriteLine("Message ID: {0} \n Message From ID={1} \n Username={2} \n First Name From={3} \n Last Name From={4} \n Date={5}\n Phone Number:{6} \n First Name:{7} \n Last Name:{8}\n User ID:{9}",
                     e.message_id, e.from.id, e.from.username, e.from.first_name, e.from.last_name, e.date, e.phone_number, e.first_name, e.last_name, e.user_id);
                Console.WriteLine();
            }
            void Tr_MessageVoice(object sendr, MessageVoice e)
            {
                Console.WriteLine("Message ID: {0} \n Message From ID={1} \n Username={2} \n First Name ={3} \n Last Name ={4} \n Date={5} \n File ID:{6} \n Duration:{7} \n Mime Type:{8}\n File Size:{9}",
                      e.message_id, e.from.id, e.from.username, e.from.first_name, e.from.last_name, (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(e.date), e.file_id, e.duration, e.mime_type, e.file_size);
                Console.WriteLine();
            }

        }

        

        private void btnSend_Click(object sender, EventArgs e)
        {
            string message = txtSend.Text;
            m.SendMessage(message, ChatID);
            txtSend.Text = "";
        }

        private void btnSendPic_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();           
            dialog.Multiselect = false;
            
            if (dialog.ShowDialog() == DialogResult.OK) 
            {
               
                String path = dialog.FileName;          
                lblImage.Text = path.ToString();
                
            }
        }

        private async void btnPicSend_Click(object sender, EventArgs e)
        {
            string path = lblImage.Text;
            string caption = txtCaption.Text;
            await m.SendPhotoIputFile(ChatID, path, caption);
            lblImage.ResetText();
        }

        private void btnSendLocation_Click(object sender, EventArgs e)
        {
            GoogleSigned.AssignAllServices(new GoogleSigned(Properties.Settings.Default.GoogleToken));
            string adress = txtAdress.Text;

            var request = new GeocodingRequest
            {
                Address = adress,
                Sensor = false
            };
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            var response = new GeocodingService().GetResponse(request);
            var result = response.Results.First();
            float log = Convert.ToSingle(result.Geometry.Location.Longitude);
            float lan = Convert.ToSingle(result.Geometry.Location.Latitude);
            txtLan.Text = lan.ToString();
            txtLon.Text = log.ToString();

            m.SendLocation(ChatID,lan,log);
            
        }

        private void txtSend_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                btnSend.PerformClick();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtAdress.Text = "";
            txtLan.Text = "";
            txtLon.Text = "";
        }

        System.Threading.Thread t;
        private void Form1_Load(object sender, EventArgs e)
        {
            t = new System.Threading.Thread(Loop);
            t.Start();
        }

        public void Loop()
        {
            while (true)
            {
                Tr.GetUpdates();
                MethodInvoker mi = delegate () { this.Text = DateTime.Now.ToString(); };
                this.Invoke(mi);
            }
        }

        private void SetText(string text)
        {        
            if (this.txt_messageRecieved.InvokeRequired)
            {
                SetTextCallBack d = new SetTextCallBack(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.txt_messageRecieved.Text = text;
            }
        }
    }
}
