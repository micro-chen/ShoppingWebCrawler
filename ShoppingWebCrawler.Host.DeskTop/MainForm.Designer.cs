namespace ShoppingWebCrawler.Host.DeskTop
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menu_Cookies = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Test_Http = new System.Windows.Forms.ToolStripMenuItem();
            this.verisonCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.newToolButton = new System.Windows.Forms.ToolStripButton();
            this.addressTextBox = new ShoppingWebCrawler.Host.DeskTop.ToolStripSpringTextBox();
            this.goButton = new System.Windows.Forms.ToolStripButton();
            this.tabContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newTabContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeTabContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.taobaoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.tabContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_Cookies,
            this.menu_Test_Http,
            this.verisonCheckToolStripMenuItem,
            this.showSourceToolStripMenuItem,
            this.taobaoToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(770, 25);
            this.menuStrip.TabIndex = 1;
            // 
            // menu_Cookies
            // 
            this.menu_Cookies.Name = "menu_Cookies";
            this.menu_Cookies.Size = new System.Drawing.Size(94, 21);
            this.menu_Cookies.Text = "Load Cookie";
            this.menu_Cookies.Click += new System.EventHandler(this.menu_Cookies_Click);
            // 
            // menu_Test_Http
            // 
            this.menu_Test_Http.Name = "menu_Test_Http";
            this.menu_Test_Http.Size = new System.Drawing.Size(78, 21);
            this.menu_Test_Http.Text = "Test HTTP";
            this.menu_Test_Http.Click += new System.EventHandler(this.menu_Test_Http_Click);
            // 
            // verisonCheckToolStripMenuItem
            // 
            this.verisonCheckToolStripMenuItem.Name = "verisonCheckToolStripMenuItem";
            this.verisonCheckToolStripMenuItem.Size = new System.Drawing.Size(103, 21);
            this.verisonCheckToolStripMenuItem.Text = "Verison Check";
            this.verisonCheckToolStripMenuItem.Click += new System.EventHandler(this.verisonCheckToolStripMenuItem_Click);
            // 
            // showSourceToolStripMenuItem
            // 
            this.showSourceToolStripMenuItem.Name = "showSourceToolStripMenuItem";
            this.showSourceToolStripMenuItem.Size = new System.Drawing.Size(91, 21);
            this.showSourceToolStripMenuItem.Text = "ShowSource";
            this.showSourceToolStripMenuItem.Click += new System.EventHandler(this.showSourceToolStripMenuItem_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 572);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(770, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(73, 17);
            this.statusLabel.Text = "statusLabel";
            // 
            // tabControl
            // 
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 50);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.ShowToolTips = true;
            this.tabControl.Size = new System.Drawing.Size(770, 522);
            this.tabControl.TabIndex = 3;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            this.tabControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tabControl_MouseUp);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolButton,
            this.addressTextBox,
            this.goButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 25);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(770, 25);
            this.toolStrip.TabIndex = 4;
            this.toolStrip.Text = "toolStrip1";
            // 
            // newToolButton
            // 
            this.newToolButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.newToolButton.Image = ((System.Drawing.Image)(resources.GetObject("newToolButton.Image")));
            this.newToolButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newToolButton.Name = "newToolButton";
            this.newToolButton.Size = new System.Drawing.Size(38, 22);
            this.newToolButton.Text = "&New";
            this.newToolButton.Click += new System.EventHandler(this.newTabAction);
            // 
            // addressTextBox
            // 
            this.addressTextBox.Name = "addressTextBox";
            this.addressTextBox.Size = new System.Drawing.Size(670, 25);
            this.addressTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.addressTextBox_KeyDown);
            // 
            // goButton
            // 
            this.goButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.goButton.Image = ((System.Drawing.Image)(resources.GetObject("goButton.Image")));
            this.goButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(29, 22);
            this.goButton.Text = "Go";
            this.goButton.Click += new System.EventHandler(this.goAddressAction);
            // 
            // tabContextMenu
            // 
            this.tabContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newTabContextMenuItem,
            this.closeTabContextMenuItem});
            this.tabContextMenu.Name = "tabContextMenu";
            this.tabContextMenu.Size = new System.Drawing.Size(132, 48);
            // 
            // newTabContextMenuItem
            // 
            this.newTabContextMenuItem.Name = "newTabContextMenuItem";
            this.newTabContextMenuItem.Size = new System.Drawing.Size(131, 22);
            this.newTabContextMenuItem.Text = "New tab";
            this.newTabContextMenuItem.Click += new System.EventHandler(this.newTabAction);
            // 
            // closeTabContextMenuItem
            // 
            this.closeTabContextMenuItem.Name = "closeTabContextMenuItem";
            this.closeTabContextMenuItem.Size = new System.Drawing.Size(131, 22);
            this.closeTabContextMenuItem.Text = "Close tab";
            this.closeTabContextMenuItem.Click += new System.EventHandler(this.closeTabAction);
            // 
            // taobaoToolStripMenuItem
            // 
            this.taobaoToolStripMenuItem.Name = "taobaoToolStripMenuItem";
            this.taobaoToolStripMenuItem.Size = new System.Drawing.Size(65, 21);
            this.taobaoToolStripMenuItem.Text = "Taobao";
            this.taobaoToolStripMenuItem.Click += new System.EventHandler(this.taobaoToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 594);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "CrawlerDesktopClient";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.tabContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton newToolButton;
        private System.Windows.Forms.ContextMenuStrip tabContextMenu;
        private System.Windows.Forms.ToolStripMenuItem newTabContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeTabContextMenuItem;
        private ToolStripSpringTextBox addressTextBox;
        private System.Windows.Forms.ToolStripButton goButton;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripMenuItem menu_Cookies;
        private System.Windows.Forms.ToolStripMenuItem menu_Test_Http;
        private System.Windows.Forms.ToolStripMenuItem verisonCheckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSourceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem taobaoToolStripMenuItem;
    }
}