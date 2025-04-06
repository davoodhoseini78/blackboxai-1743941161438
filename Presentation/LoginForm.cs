using System;
using System.Drawing;
using System.Windows.Forms;
using YourNamespace.Business;

namespace YourNamespace.Presentation
{
    public partial class LoginForm : Form
    {
        private readonly AuthService _authService;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblStatus;

        public LoginForm()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        private void InitializeComponent()
        {
            // Form setup
            this.Text = "Login";
            this.ClientSize = new Size(300, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Username label and textbox
            var lblUsername = new Label
            {
                Text = "Username:",
                Location = new Point(20, 20),
                Size = new Size(80, 20)
            };

            txtUsername = new TextBox
            {
                Location = new Point(100, 20),
                Size = new Size(160, 20)
            };

            // Password label and textbox
            var lblPassword = new Label
            {
                Text = "Password:",
                Location = new Point(20, 50),
                Size = new Size(80, 20)
            };

            txtPassword = new TextBox
            {
                Location = new Point(100, 50),
                Size = new Size(160, 20),
                PasswordChar = '*'
            };

            // Login button
            btnLogin = new Button
            {
                Text = "Login",
                Location = new Point(100, 90),
                Size = new Size(100, 30)
            };
            btnLogin.Click += btnLogin_Click;

            // Status label
            lblStatus = new Label
            {
                Location = new Point(20, 130),
                Size = new Size(260, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Add controls to form
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(lblStatus);
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            lblStatus.Text = "Authenticating...";
            lblStatus.ForeColor = Color.Blue;

            try
            {
                var isAuthenticated = await _authService.LoginAsync(txtUsername.Text, txtPassword.Text);
                if (isAuthenticated)
                {
                    this.Hide();
                    var mainForm = new MainForm();
                    mainForm.Show();
                }
                else
                {
                    lblStatus.Text = "Invalid credentials";
                    lblStatus.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }
    }
}