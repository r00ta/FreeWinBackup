namespace FreeWinBackup.Setup
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
            this.lblInstallPath = new System.Windows.Forms.Label();
            this.txtInstallPath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.chkAutoStart = new System.Windows.Forms.CheckBox();
            this.chkDesktopShortcut = new System.Windows.Forms.CheckBox();
            this.chkLaunchAfterInstall = new System.Windows.Forms.CheckBox();
            this.btnInstall = new System.Windows.Forms.Button();
            this.btnUninstall = new System.Windows.Forms.Button();
            this.btnOpenInstallFolder = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblInstallPath
            // 
            this.lblInstallPath.AutoSize = true;
            this.lblInstallPath.Location = new System.Drawing.Point(12, 15);
            this.lblInstallPath.Name = "lblInstallPath";
            this.lblInstallPath.Size = new System.Drawing.Size(90, 15);
            this.lblInstallPath.TabIndex = 0;
            this.lblInstallPath.Text = "Install location:";
            // 
            // txtInstallPath
            // 
            this.txtInstallPath.Location = new System.Drawing.Point(108, 12);
            this.txtInstallPath.Name = "txtInstallPath";
            this.txtInstallPath.Size = new System.Drawing.Size(356, 23);
            this.txtInstallPath.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(470, 11);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(87, 25);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse…";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // chkAutoStart
            // 
            this.chkAutoStart.AutoSize = true;
            this.chkAutoStart.Location = new System.Drawing.Point(15, 53);
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.Size = new System.Drawing.Size(187, 19);
            this.chkAutoStart.TabIndex = 3;
            this.chkAutoStart.Text = "Start FreeWinBackup at logon";
            this.chkAutoStart.UseVisualStyleBackColor = true;
            // 
            // chkDesktopShortcut
            // 
            this.chkDesktopShortcut.AutoSize = true;
            this.chkDesktopShortcut.Location = new System.Drawing.Point(15, 78);
            this.chkDesktopShortcut.Name = "chkDesktopShortcut";
            this.chkDesktopShortcut.Size = new System.Drawing.Size(187, 19);
            this.chkDesktopShortcut.TabIndex = 4;
            this.chkDesktopShortcut.Text = "Create desktop shortcut (user)";
            this.chkDesktopShortcut.UseVisualStyleBackColor = true;
            // 
            // chkLaunchAfterInstall
            // 
            this.chkLaunchAfterInstall.AutoSize = true;
            this.chkLaunchAfterInstall.Location = new System.Drawing.Point(224, 53);
            this.chkLaunchAfterInstall.Name = "chkLaunchAfterInstall";
            this.chkLaunchAfterInstall.Size = new System.Drawing.Size(187, 19);
            this.chkLaunchAfterInstall.TabIndex = 5;
            this.chkLaunchAfterInstall.Text = "Launch FreeWinBackup now";
            this.chkLaunchAfterInstall.UseVisualStyleBackColor = true;
            // 
            // btnInstall
            // 
            this.btnInstall.Location = new System.Drawing.Point(15, 111);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(120, 27);
            this.btnInstall.TabIndex = 6;
            this.btnInstall.Text = "Install";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // btnUninstall
            // 
            this.btnUninstall.Location = new System.Drawing.Point(141, 111);
            this.btnUninstall.Name = "btnUninstall";
            this.btnUninstall.Size = new System.Drawing.Size(120, 27);
            this.btnUninstall.TabIndex = 7;
            this.btnUninstall.Text = "Uninstall";
            this.btnUninstall.UseVisualStyleBackColor = true;
            this.btnUninstall.Click += new System.EventHandler(this.btnUninstall_Click);
            // 
            // btnOpenInstallFolder
            // 
            this.btnOpenInstallFolder.Location = new System.Drawing.Point(267, 111);
            this.btnOpenInstallFolder.Name = "btnOpenInstallFolder";
            this.btnOpenInstallFolder.Size = new System.Drawing.Size(170, 27);
            this.btnOpenInstallFolder.TabIndex = 8;
            this.btnOpenInstallFolder.Text = "Open install folder";
            this.btnOpenInstallFolder.UseVisualStyleBackColor = true;
            this.btnOpenInstallFolder.Click += new System.EventHandler(this.btnOpenInstallFolder_Click);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(15, 154);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(542, 220);
            this.txtLog.TabIndex = 9;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 395);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnOpenInstallFolder);
            this.Controls.Add(this.btnUninstall);
            this.Controls.Add(this.btnInstall);
            this.Controls.Add(this.chkLaunchAfterInstall);
            this.Controls.Add(this.chkDesktopShortcut);
            this.Controls.Add(this.chkAutoStart);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtInstallPath);
            this.Controls.Add(this.lblInstallPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FreeWinBackup Setup";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblInstallPath;
        private System.Windows.Forms.TextBox txtInstallPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.CheckBox chkAutoStart;
        private System.Windows.Forms.CheckBox chkDesktopShortcut;
        private System.Windows.Forms.CheckBox chkLaunchAfterInstall;
        private System.Windows.Forms.Button btnInstall;
        private System.Windows.Forms.Button btnUninstall;
        private System.Windows.Forms.Button btnOpenInstallFolder;
        private System.Windows.Forms.TextBox txtLog;
    }
}
