using System;
using Telegram_Bot.Request;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Telegram_Bot
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());           
            //m.GetME();

            //string loc = Console.ReadLine();         

            //while (loc == "location")
            //{
            //    Console.WriteLine("Please type in the location you want to go: ");
            //    string adress = Console.ReadLine();
            //    ga.GetUpdates(adress);              
            //}
            //string message;

            //message = Console.ReadLine();
            //m.SendMessage(message, ChatID);
            //Console.ReadLine();

        }

       
    }
}
