using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using YourNamespace.Business;
using YourNamespace.Data;

namespace YourNamespace.Presentation
{
    public class MainForm : Form
    {
        private readonly UploadService _uploadService;
        private List<string> _selectedFiles = new List<string>();
        private ProgressBar _progressBar;
        private Label _statusLabel;
        private Button _btnSelectFiles;
        private Button _btnStartUpload;
        private Button _btnCancel;
        private ListBox _fileListBox;

        public MainForm()
        {
            InitializeComponents();
            _uploadService = new UploadService(new ApiClient("https://yourapi.com"));
        }

        private void InitializeComponents()
        {
            // Form setup
            this.Text = "File Upload Application";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            // File selection button
            _btnSelectFiles = new Button
            {
                Text = "Select Files",
                Location = new Point(20, 20),
                Size = new Size(100, 30)
            };
            _btnSelectFiles.Click += OnSelectFilesClicked;

            // Upload button
            _btnStartUpload = new Button
            {
                Text = "Start Upload",
                Location = new Point(130, 20),
                Size = new Size(100, 30),
                Enabled = false
            };
            _btnStartUpload.Click += OnStartUploadClicked;

            // Cancel button
            _btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(240, 20),
                Size = new Size(100, 30),
                Enabled = false
            };
            _btnCancel.Click += OnCancelClicked;

            // File list box
            _fileListBox = new ListBox
            {
                Location = new Point(20, 60),
                Size = new Size(450, 200)
            };

            // Progress bar
            _progressBar = new ProgressBar
            {
                Location = new Point(20, 270),
                Size = new Size(450, 30)
            };

            // Status label
            _statusLabel = new Label
            {
                Location = new Point(20, 310),
                Size = new Size(450, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Add controls to form
            this.Controls.Add(_btnSelectFiles);
            this.Controls.Add(_btnStartUpload);
            this.Controls.Add(_btnCancel);
            this.Controls.Add(_fileListBox);
            this.Controls.Add(_progressBar);
            this.Controls.Add(_statusLabel);
        }

        private void OnSelectFilesClicked(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Multiselect = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _selectedFiles.Clear();
                    _selectedFiles.AddRange(openFileDialog.FileNames);
                    _fileListBox.Items.Clear();
                    _fileListBox.Items.AddRange(openFileDialog.FileNames);
                    _btnStartUpload.Enabled = _selectedFiles.Count > 0;
                    _statusLabel.Text = $"Selected {_selectedFiles.Count} files";
                }
            }
        }

        private async void OnStartUploadClicked(object sender, EventArgs e)
        {
            _btnSelectFiles.Enabled = false;
            _btnStartUpload.Enabled = false;
            _btnCancel.Enabled = true;
            _progressBar.Value = 0;

            var progress = new Progress<int>(percent =>
            {
                _progressBar.Value = percent;
                _statusLabel.Text = $"Uploading... {percent}%";
            });

            try
            {
                await _uploadService.UploadFilesAsync(_selectedFiles, progress, new Progress<string>(UpdateStatus));
                _statusLabel.Text = "Upload completed successfully!";
            }
            catch (OperationCanceledException)
            {
                _statusLabel.Text = "Upload was cancelled";
            }
            catch (Exception ex)
            {
                _statusLabel.Text = $"Error: {ex.Message}";
            }
            finally
            {
                _btnSelectFiles.Enabled = true;
                _btnStartUpload.Enabled = _selectedFiles.Count > 0;
                _btnCancel.Enabled = false;
            }
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            _uploadService.CancelUpload();
        }

        private void UpdateStatus(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateStatus), message);
                return;
            }
            _statusLabel.Text = message;
        }
    }
}