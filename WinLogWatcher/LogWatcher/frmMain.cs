using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonLib;

namespace LogWatcher
{
    public partial class frmMain : Form
    {

        private System.Threading.Thread oThread;
        private System.Threading.Thread oUDPServerThread;
        private System.Net.Sockets.UdpClient oUDPClient;
        private int UDPLoggerMaxLines = 0;

        private delegate void delReportLog(Hashtable oHashTable);

        private void StartListener(object oState)
        {
            try
            {
                oUDPClient = new System.Net.Sockets.UdpClient(Convert.ToInt32(CDLLReadSetting.ReadSetting("//configuration/appSettings/add[@key='MulticastPort']")));
                oUDPClient.Client.ReceiveBufferSize = Convert.ToInt32(CDLLReadSetting.ReadSetting("//configuration/appSettings/add[@key='UDPBufferSize']"));

                System.Net.IPEndPoint oRemoteIPEndPoint = new System.Net.IPEndPoint(
                    System.Net.IPAddress.Parse(CDLLReadSetting.ReadSetting("//configuration/appSettings/add[@key='MulticastIPAddress']")),
                    0);
                
                oUDPClient.DontFragment = true;

                oUDPClient.JoinMulticastGroup(System.Net.IPAddress.Parse(CDLLReadSetting.ReadSetting("//configuration/appSettings/add[@key='MulticastIPAddress']")));

                while (true)
                {
                    try
                    {
                        Byte[] arTemp = oUDPClient.Receive(ref oRemoteIPEndPoint);
                        System.IO.MemoryStream oMemoryStream = new System.IO.MemoryStream(arTemp.Length);
                        oMemoryStream.Write(arTemp, 0, arTemp.Length);

                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter oBinary = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        oMemoryStream.Seek(0, System.IO.SeekOrigin.Begin);

                        ReportLog((Hashtable)oBinary.Deserialize(oMemoryStream));

                    }
                    catch (System.Threading.ThreadAbortException ex)
                    {
                        Console.Write(ex.Message);
                        //MessageBox.Show(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                        //MessageBox.Show(ex.Message);
                    }

                    System.Windows.Forms.Application.DoEvents();

                    if (oUDPClient.Available == 0)
                    {
                        System.Threading.Thread.Sleep(1);
                    }
                }

            }

            catch (System.Threading.ThreadAbortException ex)
            {
                Console.Write(ex.Message);
                //MessageBox.Show(ex.Message);
            }

            catch (Exception ex)
            {
                Console.Write(ex.Message);
                //MessageBox.Show(ex.Message);
            }
        }

        private void StartUDPServer(object oState)
        {
            try
            {
                CommonLib.CUdpServer.receiveMessages();
                while (true)
                {
                }
            }

            catch (System.Threading.ThreadAbortException ex)
            {
                Console.Write(ex.Message);
                //MessageBox.Show(ex.Message);
            }

            catch (Exception ex)
            {
                Console.Write(ex.Message);
                //MessageBox.Show(ex.Message);
            }
        }

        private void ReportLog(Hashtable oHashtable)
        {
            try
            {
                if (ListViewLog.InvokeRequired)
                {
                    delReportLog d = new delReportLog(ReportLog);
                    ListViewLog.Invoke(d, new object[] { oHashtable });
                }
                else
                {
                    lock (oHashtable)
                    {
                        ListViewItem oListViewItem = new ListViewItem(ListViewLog.Items.Count.ToString() + " - " + oHashtable["DATETIME"].ToString());
                        oListViewItem.SubItems.Add(oHashtable["MESSAGE"].ToString());
                        oListViewItem.SubItems.Add(oHashtable["TYPE"].ToString());
                        oListViewItem.SubItems.Add(oHashtable["MODULE"].ToString() + "BS: " + oUDPClient.Client.ReceiveBufferSize.ToString());
                        oListViewItem.SubItems.Add(oHashtable["METHOD"].ToString());

                        switch (oHashtable["TYPE"].ToString().ToUpper())
                        {
                            case "ERROR":
                                {
                                    oListViewItem.ForeColor = Color.Red;
                                    break;                                
                                }
                            case "TRACE":
                                {
                                    oListViewItem.ForeColor = Color.DarkGray;
                                    break;                                                                
                                }
                            case "METHOD":
                                { 
                                    oListViewItem.ForeColor = Color.Gray;
                                    oListViewItem.Font = new System.Drawing.Font(oListViewItem.Font, FontStyle.Italic);
                                    break;
                                }
                            default:
                                {
                                    break;
                                    // Do nothing
                                }
                        }

                        oListViewItem.SubItems.Add(oHashtable["THREADID"].ToString());
                        oListViewItem.SubItems.Add(oHashtable["PROCID"].ToString());
                        oListViewItem.SubItems.Add(oHashtable["PROCNAME"].ToString());
                        oListViewItem.Tag = oHashtable;

                        ListViewLog.Items.Add(oListViewItem);

                        if (checkBoxVisible.Checked) oListViewItem.EnsureVisible();

                        if (ListViewLog.Items.Count > UDPLoggerMaxLines)
                        {
                            ListViewLog.Items[0].Remove();
                            System.Threading.Thread.Sleep(1);                            
                        }

                        ListViewLog.Update();                        
                        Application.DoEvents();                 
                    }               
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
               // MessageBox.Show(ex.Message);
            }
        }


        public void ClearLog(object sender, EventArgs e)
        {
            try
            {
                ListViewLog.Items.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message);
            }

        }

        public frmMain()
        {
            InitializeComponent();

            UDPLoggerMaxLines = Convert.ToInt32(CDLLReadSetting.ReadSetting("//configuration/appSettings/add[@key='UDPLoggerMaxLines']"));
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                btnStart.Enabled = false;
                btnStop.Enabled = true;

                if (oThread == null)
                {
                    oThread = new System.Threading.Thread(StartListener);
                    oThread.IsBackground = true;
                    oThread.Start();
                }

                if (oUDPServerThread == null)
                {
                    oUDPServerThread = new System.Threading.Thread(StartUDPServer);
                    oUDPServerThread.IsBackground = true;
                    oUDPServerThread.Start();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                btnStart.Enabled = !btnStart.Enabled;
                btnStop.Enabled = !btnStop.Enabled;
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                btnStart.Enabled = true;
                btnStop.Enabled = false;

                if (oUDPServerThread != null)
                {
                    if (oUDPServerThread.IsAlive)
                    {
                        oUDPServerThread.Abort();
                        oUDPServerThread = null;
                    }
                }


                if (oThread != null) 
                {
                    if (oThread.IsAlive)
                    {
                        oThread.Abort();
                        oThread = null;
                    }
                }

                if (oUDPClient != null)
                {
                    oUDPClient.Close();
                    oUDPClient = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                btnStart.Enabled = !btnStart.Enabled;
                btnStop.Enabled = !btnStop.Enabled;
                //MessageBox.Show(ex.Message);
            }

        }

        private void timerGC_Tick(object sender, EventArgs e)
        {
            try
            {
                GC.Collect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //MessageBox.Show(ex.Message);
            }
        }

        private void checkBoxTopMost_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = checkBoxTopMost.Checked;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void checkBoxVisible_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message);
            }    
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ListViewLog.Items.Clear();
        }

    }
}
