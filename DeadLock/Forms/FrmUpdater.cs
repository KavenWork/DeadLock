using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows.Forms;
using DeadLock.Classes;
using Syncfusion.Windows.Forms;

namespace DeadLock.Forms
{
    public partial class FrmUpdater : MetroForm
    {
        #region Variables
        private readonly Update _update;
        private readonly WebClient _webClient;
        private readonly Language _language;
        private readonly string _extension;
        #endregion

        /// <summary>
        /// Generate a new FrmUpdater form.
        /// </summary>
        /// <param name="update">The Update that should be loaded.</param>
        /// <param name="language">The current Language.</param>
        public FrmUpdater(Update update, Language language)
        {
            InitializeComponent();
            _update = update;
            _webClient = new WebClient();
            _language = language;

            _webClient.DownloadFileCompleted += Completed;
            _webClient.DownloadProgressChanged += ProgressChanged;

            _extension = _update.UpdateUrl.Substring(_update.UpdateUrl.Length - 4, 4);
        }

        /// <summary>
        /// Change the GUI to match the current Language.
        /// </summary>
        private void LoadLanguage()
        {
            Text = @"DeadLock - " + _language.TxtUpdater;

            lblPath.Text = _language.LblPath;
            lblProgress.Text = _language.LblProgress;

            btnCancel.Text = _language.BtnCancel;
            btnUpdate.Text = _language.BtnUpdate;
        }

        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog { Filter = _extension.ToUpper() + @" (*" + _extension + @")|*" + _extension };
            if (Directory.Exists(txtPath.Text))
            {
                sfd.InitialDirectory = txtPath.Text;
            }

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = sfd.FileName;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtPath.Text.Length == 0) return;
            DisableControls();
            _webClient.DownloadFileAsync(new Uri(_update.UpdateUrl), txtPath.Text);
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            pgbDownload.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// Enable the controls to allow the user to customize how the update proceeds.
        /// </summary>
        private void EnableControls()
        {
            txtPath.Enabled = true;
            btnSelectPath.Enabled = true;
            btnUpdate.Enabled = true;
            pgbDownload.Value = 0;
        }

        /// <summary>
        /// Disable the controls to disallow the user to customize how the update proceeds.
        /// </summary>
        private void DisableControls()
        {
            txtPath.Enabled = false;
            btnSelectPath.Enabled = false;
            btnUpdate.Enabled = false;
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                EnableControls();
            }
            else if (e.Error != null)
            {
                MessageBoxAdv.Show(e.Error.Message, "DeadLock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                EnableControls();
            }
            else
            {
                try
                {
                    System.Diagnostics.Process.Start(txtPath.Text);
                    Application.Exit();
                }
                catch (Exception ex)
                {
                    MessageBoxAdv.Show(ex.Message, "DeadLock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _webClient.CancelAsync();
        }

        /// <summary>
        /// Change the GUI to match the theme.
        /// </summary>
        private void LoadTheme()
        {
            try
            {
                BorderThickness = Properties.Settings.Default.BorderThickness;
                MetroColor = Properties.Settings.Default.MetroColor;
                BorderColor = Properties.Settings.Default.MetroColor;
                CaptionBarColor = Properties.Settings.Default.MetroColor;

                txtPath.Metrocolor = Properties.Settings.Default.MetroColor;
                btnSelectPath.MetroColor = Properties.Settings.Default.MetroColor;
                pgbDownload.GradientEndColor = Properties.Settings.Default.MetroColor;
                pgbDownload.GradientStartColor = Properties.Settings.Default.MetroColor;
                btnCancel.MetroColor = Properties.Settings.Default.MetroColor;
                btnUpdate.MetroColor = Properties.Settings.Default.MetroColor;
            }
            catch (Exception ex)
            {
                MessageBoxAdv.Show(ex.Message, "DeadLock", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FrmUpdater_Load(object sender, EventArgs e)
        {
            LoadLanguage();
            LoadTheme();

            txtPath.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\dl_setup" + _extension;
        }
    }
}