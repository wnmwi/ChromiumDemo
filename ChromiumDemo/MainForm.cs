using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace ChromiumDemo
{
    public partial class MainForm : Form
    {
        private ChromiumWebBrowser chromeBrowser;
        //delegate void ChromeBrowserLoadingStateChangedCallBack(bool isLoading);
        //ChromeBrowserLoadingStateChangedCallBack chromeBrowserLoadingStateChangedCallBack;
        delegate void ChromeBrowserFrameLoadEndCallBack();
        ChromeBrowserFrameLoadEndCallBack chromeBrowserFrameLoadEndCallBack;

        public MainForm()
        {
            InitializeComponent();

            //实例一个Cef设置，设置其中的本地缓存路径
            CefSettings Settings = new CefSettings();
            Settings.CachePath = System.IO.Directory.GetCurrentDirectory() + @"\Cache";
            //将Cef设置传进去，实例一个web_Auto
            Cef.Initialize(Settings);

            //chromeBrowserLoadingStateChangedCallBack = new ChromeBrowserLoadingStateChangedCallBack(ChromeBrowserLoadingStateChangedEvent);
            chromeBrowserFrameLoadEndCallBack = new ChromeBrowserFrameLoadEndCallBack(chromeBrowserFrameLoadEndEvent);

            // Start the browser after initialize global component
            InitializeChromium("https://cn.bing.com/");
        }

        public void InitializeChromium(string url)
        {
            //CefSettings settings = new CefSettings();
            //settings.Locale = "zh-CN";
            // Initialize cef with the provided settings
            //Cef.Initialize(settings);
            // Create a browser component
            chromeBrowser = new ChromiumWebBrowser(url);//http://ourcodeworld.com http://html5test.com
            BrowserSettings browserSettings = new BrowserSettings();
            browserSettings.AcceptLanguageList = "zh_CN";
            chromeBrowser.BrowserSettings = browserSettings;
            // Add it to the form and fill it to the form window.
            this.panel1.Controls.Add(chromeBrowser);
            chromeBrowser.Dock = DockStyle.Fill;
            //chromeBrowser.LoadingStateChanged += new EventHandler<LoadingStateChangedEventArgs>(chromeBrowser_LoadingStateChanged);
            chromeBrowser.FrameLoadEnd += new EventHandler<FrameLoadEndEventArgs>(chromeBrowser_FrameLoadEnd);
        }

        private void chromeBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            //string html = await chromeBrowser.GetSourceAsync();
            this.BeginInvoke(chromeBrowserFrameLoadEndCallBack);
        }

        private void chromeBrowserFrameLoadEndEvent()
        {
            if (!urlTextBox.Focused) urlTextBox.Text = chromeBrowser.Address;
            //string title = GetHtmlTitle(html);
            //if (title != string.Empty) this.Text = title;
        }

        private string GetHtmlTitle(string html)
        {
            html = html.ToLower();
            int startPos = html.IndexOf(@"<title>") + 7;
            int endPos= html.IndexOf(@"</title>");
            int length = endPos - startPos;
            if(startPos>=7)
            {
                return html.Substring(startPos, length);
            }
            else
            {
                return string.Empty;
            }
        }

        //private void ChromeBrowserLoadingStateChangedEvent(bool isLoading)
        //{
        //    if(!isLoading)
        //    {
        //        if(!urlTextBox.Focused) urlTextBox.Text = chromeBrowser.Address;
        //    }
        //}

        //private void chromeBrowser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        //{
        //    this.BeginInvoke(chromeBrowserLoadingStateChangedCallBack, e.IsLoading);
        //}

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown(); //不能在FormClosed里写这句，会偶发卡死
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            ChromeBrowserNavigateByUrlTextBox();
        }
        
        void ChromeBrowserNavigateByUrlTextBox()
        {
            string url = urlTextBox.Text.Trim();
            chromeBrowser.Load(url);
        }

        private void urlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void reloadButton_Click(object sender, EventArgs e)
        {
            chromeBrowser.Reload();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if(chromeBrowser.CanGoBack) chromeBrowser.Back();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(chromeBrowser.CanGoForward) chromeBrowser.Forward();
        }

        private void urlTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(13))
            {
                goButton.Focus();
                ChromeBrowserNavigateByUrlTextBox();
                e.Handled = true;
            }
        }
    }
}
