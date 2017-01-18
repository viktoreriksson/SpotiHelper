using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SpotiHelper
{
    public partial class frmMain : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", SetLastError=true, CharSet=CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            txtLocation.Text = Properties.Settings.Default.txtLocation;
        }

        public string lastWritten = null;       //keep track of last song written to txt file, for minimal disk usage:)
        private void tmrSong_Tick(object sender, EventArgs e)
        {
            IntPtr spotify = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "SpotifyMainWindow", null);
            int spotifyTitleLength = GetWindowTextLength(spotify);

            StringBuilder spotifyTitle = new StringBuilder(spotifyTitleLength);
            GetWindowText(spotify, spotifyTitle, spotifyTitleLength + 1);                       //title of spotify, current song title

            string song = null;
            MatchCollection spotifyTitleMatch = Regex.Matches(spotifyTitle.ToString(), "^(.+) - (.+)$");
            if (spotifyTitleMatch.Count > 0)                                                    //song playing, not an ad or is paused
            {
                song = lblSong.Text = spotifyTitle.ToString();
            }
            else
            {
                song = lblSong.Text = "No song currently playing";
            }

            if(song != lastWritten)     //check if current song was the last song written to txt, if not, write to disk
            {
                if (chkLog.Checked)     //but not before asking user for permission!
                {
                    File.WriteAllText(txtLocation.Text, lblSong.Text);
                    lastWritten = song;
                }
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.InitialDirectory = Environment.CurrentDirectory;
                sfd.Filter = "Text Files|*.txt";
                if (sfd.ShowDialog() == DialogResult.Cancel) return;
                txtLocation.Text = sfd.FileName;
                Properties.Settings.Default.txtLocation = sfd.FileName;
                Properties.Settings.Default.Save();
            }
        }
    }
}
