using System;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Baran_uncu_121620211030_proje_gorsel_programlama
{
    public partial class FormGiris : Form
    {
        private TextBox txtEmail, txtSifre;
        private Button btnGiris, btnKayit; 

        public FormGiris()
        {
            this.Text = "Sanal Market - Giriş";
            this.Size = new Size(400, 460); 
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            TasarimiOlustur();
        }

        private string SifreHashle(string sifre)
        {
            using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(sifre));
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        private void TasarimiOlustur()
        {
            Label lbl = new Label { Text = "Sanal Market", Font = new Font("Segoe UI", 20F, FontStyle.Bold), ForeColor = Color.FromArgb(26, 115, 58), Location = new Point(50, 40), Size = new Size(300, 50), TextAlign = ContentAlignment.MiddleCenter };

            Label lblEmail = new Label { Text = "E-posta Adresiniz:", Location = new Point(50, 110), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            txtEmail = new TextBox { Location = new Point(50, 130), Size = new Size(300, 35), Font = new Font("Segoe UI", 12F) };

            Label lblSifre = new Label { Text = "Şifreniz:", Location = new Point(50, 175), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            txtSifre = new TextBox { Location = new Point(50, 195), Size = new Size(300, 35), Font = new Font("Segoe UI", 12F), PasswordChar = '●' };

            
            btnGiris = new Button { Text = "GİRİŞ YAP", BackColor = Color.FromArgb(26, 115, 58), ForeColor = Color.White, Font = new Font("Segoe UI", 11F, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Location = new Point(50, 260), Size = new Size(300, 45), Cursor = Cursors.Hand };
            btnGiris.FlatAppearance.BorderSize = 0;
            btnGiris.Click += new EventHandler(BtnGiris_Click);

            
            btnKayit = new Button { Text = "HESAP OLUŞTUR (KAYIT OL)", BackColor = Color.White, ForeColor = Color.FromArgb(26, 115, 58), Font = new Font("Segoe UI", 10F, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Location = new Point(50, 320), Size = new Size(300, 45), Cursor = Cursors.Hand };
            btnKayit.FlatAppearance.BorderColor = Color.FromArgb(26, 115, 58);
            btnKayit.FlatAppearance.BorderSize = 1;
            btnKayit.Click += new EventHandler(btnKayit_Click);

            this.Controls.AddRange(new Control[] { lbl, lblEmail, txtEmail, lblSifre, txtSifre, btnGiris, btnKayit });
        }

        private void btnKayit_Click(object sender, EventArgs e)
        {
            FormKayit kayit = new FormKayit();
            kayit.ShowDialog(); 
        }

        private void BtnGiris_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtSifre.Text))
            {
                MessageBox.Show("Lütfen alanları boş bırakmayın!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string hashliGirisSifresi = SifreHashle(txtSifre.Text.Trim());

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Kullanicilar WHERE Email=@m AND Sifre=@s",
                new SqlParameter[] {
                    new SqlParameter("@m", txtEmail.Text.Trim()),
                    new SqlParameter("@s", hashliGirisSifresi)
                });

            if (dt.Rows.Count > 0)
            {
                DataRow r = dt.Rows[0];
                bool isAdmin = r["Yetki"] != DBNull.Value && Convert.ToBoolean(r["Yetki"]);

                if (isAdmin)
                {
                    MessageBox.Show($"Sistem Yetkilisi: {r["AdSoyad"]}\n\nYönetim paneline tam yetkiyle erişim sağlandı.", "Yönetici Girişi Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    new FormAdminPanel().Show();
                }
                else
                {
                    MessageBox.Show($"Hoş geldin, {r["AdSoyad"]}! Keyifli alışverişler dileriz.", "Giriş Başarılı ✅", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    new FormAnaMenu(Convert.ToInt32(r["KullaniciID"]), r["AdSoyad"].ToString(), isAdmin).Show();
                }
                this.Hide();
            }
            else
            {
                MessageBox.Show("E-posta veya şifre hatalı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}