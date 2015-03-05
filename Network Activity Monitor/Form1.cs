using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Management.Instrumentation;
using System.Collections.Specialized;
using System.Threading;

namespace Network_Activity_Monitor
{
    public partial class Network : Form
    {
        NotifyIcon netAct;
        Icon netTx;
        Icon netRx;
        Icon netTxRx;
        Icon netIdle;
        Thread netActMonWorker;
        public Network()
        {
            netTx = new Icon("TX.ico");
            netRx = new Icon("RX.ico");
            netTxRx = new Icon("TXRX.ico");
            netIdle = new Icon("net.ico");
            InitializeComponent();

            netAct = new NotifyIcon();
            netAct.Icon = netIdle; //Set the icon to show the idle image
            netAct.Visible = true; //turn on visability of the icon

            MenuItem exitTheProgram = new MenuItem("Quit");
            ContextMenu contextMenu = new ContextMenu();
            netAct.ContextMenu = contextMenu;
            contextMenu.MenuItems.Add(exitTheProgram);
            exitTheProgram.Click += exitTheProgram_Click;

            netActMonWorker = new Thread(new ThreadStart(monitor));
            netActMonWorker.Start();
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false; // Don't show a window


        }//main method
        void exitTheProgram_Click(object sender, EventArgs e)
        { //Cleanup icon then Exit the program is the quit button is pressed
            netAct.Dispose(); //Tray Icon
            netActMonWorker.Abort(); //Thread
            this.Close();
        }//exit method
    public void monitor(){
        ManagementClass networkDataClass = new ManagementClass("Win32_PerfFormattedData_Tcpip_NetworkInterface");
        try
        {
            while (true)
            {
                ManagementObjectCollection networkDataClassCollection = networkDataClass.GetInstances();
                foreach (ManagementObject obj in networkDataClassCollection)
                {

                        if (Convert.ToInt32(obj["PacketsReceivedPerSec"]) > 0 & Convert.ToInt32(obj["PacketsSentPerSec"])>0) // If Packets are coming in and going out
                        {
                            netAct.Icon = netTxRx;
                        }
                        else if (Convert.ToInt32(obj["PacketsSentPerSec"]) > 0)
                        {
                            netAct.Icon = netTx;
                        }
                        else if (Convert.ToInt32(obj["PacketsReceivedPerSec"]) > 0)
                        {
                            netAct.Icon = netRx;
                            Thread.Sleep(150);
                        }

                        else //no network activity
                        {
                            netAct.Icon = netIdle;
                        }
                    
                }
                Thread.Sleep(30);
            }
        }
        catch (ThreadAbortException x)
        {
            //cleanup
        }
     }//Net Mon Thread
    }//Class

}//namespace