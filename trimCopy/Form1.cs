using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace trimCopy
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        IntPtr nextClipboardViewer;

        Boolean trimWorking = true;

        //hide form from Alt-Tab dialog
        protected override CreateParams CreateParams
        {
            get
            {
                // Turn on WS_EX_TOOLWINDOW style bit
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80;
                return cp;
            }
        }

        //form invisible
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            this.Visible = false;
        }

        public Form1()
        {
            InitializeComponent();
            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);
            //this.ShowInTaskbar = false; //不顯示在底下工具列(改設定在form屬性)
            this.Hide();    //隱藏視窗
            this.notifyIcon1.ContextMenu = new ContextMenu();
            this.notifyIcon1.ContextMenu.MenuItems.Add(new MenuItem("EXIT",  new EventHandler(Exit)));
            notifyIcon1.Icon = Properties.Resources.checkIcon;
        }


        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            // defined in winuser.h
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:                
                    SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    TrimClipboardText();
                    break;

                case WM_CHANGECBCHAIN:
                    if (m.WParam == nextClipboardViewer)
                        nextClipboardViewer = m.LParam;
                    else
                        SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        void TrimClipboardText()
        {
            IDataObject iData = new DataObject();
            iData = Clipboard.GetDataObject();

            if (iData != null && trimWorking == true)
            {
                try
                {
                  if (iData.GetDataPresent(DataFormats.UnicodeText))
                  {
                      Clipboard.SetText(((string)iData.GetData(DataFormats.UnicodeText)).Trim(), TextDataFormat.UnicodeText);
                  }              
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
        }


        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (trimWorking == true)
            {
                trimWorking = false;
                notifyIcon1.Icon = Properties.Resources.unCheckIcon;
                notifyIcon1.ShowBalloonTip(100, null, "Stop Working", ToolTipIcon.Warning);
            }
            else if (trimWorking == false)
            {
                trimWorking = true;
                notifyIcon1.Icon = Properties.Resources.checkIcon;
                notifyIcon1.ShowBalloonTip(100, null, "Start Working", ToolTipIcon.Warning);
            }    
        }

        private void Exit(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
