using System;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Baran_uncu_121620211030_proje_gorsel_programlama
{
    public class FormKayit : Form
    {
        private Panel pnlUst;
        private Panel pnlForm;
        private Label lblBaslik;
        private Label lblAdSoyad;
        private TextBox txtAdSoyad;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblTelefon;
        private TextBox txtTelefon;
        private Label lblSifre;
        private TextBox txtSifre;
        private Label lblSifreTekrar;
        private TextBox txtSifreTekrar;
        private Button btnKayitOl;
        private Label lblHata;
        private Label lblBasari;

        public FormKayit()
        {
            this.Text = "Sanal Market - Kayit Ol";
            this.Size = new Size(460, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            TasarimiOlustur();
        }

        // SHA-256 Şifreleme Yardımcı Metodu
        public static string SifreHashle(string sifre)
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
            pnlUst = new Panel { BackColor = Color.FromArgb(26, 115, 58), Dock = DockStyle.Top, Height = 100 };

            lblBaslik = new Label { Text = "Yeni Hesap Olustur", Font = new Font("Segoe UI", 18F, FontStyle.Bold), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter, Location = new Point(0, 0), Size = new Size(460, 100) };
            pnlUst.Controls.Add(lblBaslik);

            pnlForm = new Panel { BackColor = Color.White, Location = new Point(0, 100), Size = new Size(460, 560) };

            lblAdSoyad = new Label { Text = "Ad Soyad *", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(80, 80, 80), Location = new Point(50, 20), Size = new Size(360, 20) };
            txtAdSoyad = new TextBox { Font = new Font("Segoe UI", 11F), Location = new Point(50, 43), Size = new Size(360, 30), BorderStyle = BorderStyle.FixedSingle };

            lblEmail = new Label { Text = "E-posta Adresi *", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(80, 80, 80), Location = new Point(50, 85), Size = new Size(360, 20) };
            txtEmail = new TextBox { Font = new Font("Segoe UI", 11F), Location = new Point(50, 108), Size = new Size(360, 30), BorderStyle = BorderStyle.FixedSingle };
            txtEmail.Leave += new EventHandler(txtEmail_Leave);

            lblTelefon = new Label { Text = "Telefon (05XX XXX XX XX)", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(80, 80, 80), Location = new Point(50, 150), Size = new Size(360, 20) };
            txtTelefon = new TextBox { Font = new Font("Segoe UI", 11F), Location = new Point(50, 173), Size = new Size(360, 30), BorderStyle = BorderStyle.FixedSingle, MaxLength = 11 };

            lblSifre = new Label { Text = "Sifre * (en az 6 karakter)", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(80, 80, 80), Location = new Point(50, 215), Size = new Size(360, 20) };
            txtSifre = new TextBox { Font = new Font("Segoe UI", 11F), Location = new Point(50, 238), Size = new Size(360, 30), BorderStyle = BorderStyle.FixedSingle, PasswordChar = '*' };

            lblSifreTekrar = new Label { Text = "Sifre Tekrar *", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(80, 80, 80), Location = new Point(50, 280), Size = new Size(360, 20) };
            txtSifreTekrar = new TextBox { Font = new Font("Segoe UI", 11F), Location = new Point(50, 303), Size = new Size(360, 30), BorderStyle = BorderStyle.FixedSingle, PasswordChar = '*' };

            btnKayitOl = new Button { Text = "KAYIT OL", Font = new Font("Segoe UI", 11F, FontStyle.Bold), BackColor = Color.FromArgb(26, 115, 58), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(50, 355), Size = new Size(360, 48), Cursor = Cursors.Hand };
            btnKayitOl.FlatAppearance.BorderSize = 0;
            btnKayitOl.Click += new EventHandler(btnKayitOl_Click);

            lblHata = new Label { Text = "", Font = new Font("Segoe UI", 9F), ForeColor = Color.Red, Location = new Point(50, 415), Size = new Size(360, 40), TextAlign = ContentAlignment.MiddleCenter };
            lblBasari = new Label { Text = "", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(26, 115, 58), Location = new Point(50, 460), Size = new Size(360, 25), TextAlign = ContentAlignment.MiddleCenter };

            pnlForm.Controls.AddRange(new Control[] { lblAdSoyad, txtAdSoyad, lblEmail, txtEmail, lblTelefon, txtTelefon, lblSifre, txtSifre, lblSifreTekrar, txtSifreTekrar, btnKayitOl, lblHata, lblBasari });

            this.Controls.AddRange(new Control[] { pnlUst, pnlForm });
        }

        private void txtEmail_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                if (!EmailGecerliMi(txtEmail.Text))
                {
                    lblHata.Text = "Gecersiz e-posta! Ornek: ad@ornek.com";
                    txtEmail.BackColor = Color.FromArgb(255, 235, 235);
                }
                else
                {
                    lblHata.Text = "";
                    txtEmail.BackColor = Color.White;
                }
            }
        }

        private bool EmailGecerliMi(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private void btnKayitOl_Click(object sender, EventArgs e)
        {
            lblHata.Text = "";
            lblBasari.Text = "";

            if (string.IsNullOrWhiteSpace(txtAdSoyad.Text) || string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtSifre.Text) || string.IsNullOrWhiteSpace(txtSifreTekrar.Text))
            {
                lblHata.Text = "* ile isaretli alanlar zorunludur!";
                return;
            }

            if (!EmailGecerliMi(txtEmail.Text))
            {
                lblHata.Text = "Gecersiz e-posta formati!";
                txtEmail.BackColor = Color.FromArgb(255, 235, 235);
                return;
            }

            if (txtSifre.Text != txtSifreTekrar.Text)
            {
                lblHata.Text = "Şifreler eşleşmiyor!";
                return;
            }

            try
            {
                
                string hashliSifre = SifreHashle(txtSifre.Text.Trim());

                DatabaseHelper.ExecuteNonQuery("INSERT INTO Kullanicilar (AdSoyad, Email, Telefon, Sifre, Yetki) VALUES (@ad, @mail, @tel, @sifre, 0)",
                    new SqlParameter[] {
                        new SqlParameter("@ad", txtAdSoyad.Text.Trim()),
                        new SqlParameter("@mail", txtEmail.Text.Trim()),
                        new SqlParameter("@tel", txtTelefon.Text.Trim()),
                        new SqlParameter("@sifre", hashliSifre)
                    });

                MessageBox.Show("Hesabınız başarıyla oluşturuldu!\nArtık giriş yapabilirsiniz.", "Aramıza Hoş Geldiniz! 🎉", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                lblHata.Text = "Kayıt sırasında bir hata oluştu: " + ex.Message;
            }
        }
    }
}