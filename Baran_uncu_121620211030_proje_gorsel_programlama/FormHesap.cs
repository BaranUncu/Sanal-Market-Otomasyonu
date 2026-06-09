using System;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Baran_uncu_121620211030_proje_gorsel_programlama
{
    public class FormHesap : Form
    {
        
        public class AdresItem
        {
            public int ID { get; set; }
            public string Baslik { get; set; }
            public override string ToString() { return Baslik; }
        }

        private int kullaniciID;
        private TabControl tabControl;
        private TabPage tabBilgiler, tabAdresler, tabSiparisler;

        
        private TextBox txtAdSoyad, txtEmail, txtTelefon, txtSifreEski, txtSifreYeni, txtSifreYeniTekrar;

        
        private ListBox lstAdresler;
        private TextBox txtAdresBasligi, txtAdresTanimi;
        private ComboBox cmbSehir, cmbIlce;
        private Button btnAdresEkle, btnAdresSil;

        
        private DataGridView dgvSiparislerim;

        public FormHesap(int kullaniciID)
        {
            this.kullaniciID = kullaniciID;
            this.Text = "Hesap Ayarlarım ve Adreslerim";
            this.Size = new Size(600, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            TasarimiOlustur();
            VerileriYukle();
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
            
            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(26, 115, 58) };
            Label lblBaslik = new Label { Text = "👤 HESABIM", ForeColor = Color.White, Font = new Font("Segoe UI", 16F, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
            pnlHeader.Controls.Add(lblBaslik);

            
            tabControl = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
            tabBilgiler = new TabPage(" Kişisel Bilgiler ");
            tabAdresler = new TabPage(" Adreslerim ");
            tabSiparisler = new TabPage(" Geçmiş Siparişlerim ");

            tabControl.TabPages.AddRange(new TabPage[] { tabBilgiler, tabAdresler, tabSiparisler });

            
            FlowLayoutPanel flpBilgi = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(30), AutoScroll = true };
            txtAdSoyad = ModernInput(flpBilgi, "Ad Soyad:");
            txtEmail = ModernInput(flpBilgi, "E-posta:");
            txtTelefon = ModernInput(flpBilgi, "Telefon:");

            Button btnGuncelle = ModernButton(flpBilgi, "BİLGİLERİ GÜNCELLE", Color.FromArgb(46, 204, 113));
            btnGuncelle.Click += (s, e) => BilgileriGuncelle();

            flpBilgi.Controls.Add(new Label { Text = "--------------------------------------------------", Size = new Size(500, 30), ForeColor = Color.LightGray });

            txtSifreEski = ModernInput(flpBilgi, "Mevcut Şifre:"); txtSifreEski.PasswordChar = '●';
            txtSifreYeni = ModernInput(flpBilgi, "Yeni Şifre:"); txtSifreYeni.PasswordChar = '●';
            txtSifreYeniTekrar = ModernInput(flpBilgi, "Yeni Şifre Tekrar:"); txtSifreYeniTekrar.PasswordChar = '●';

            Button btnSifre = ModernButton(flpBilgi, "ŞİFREYİ DEĞİŞTİR", Color.FromArgb(52, 152, 219));
            btnSifre.Click += (s, e) => SifreDegistir();

            tabBilgiler.Controls.Add(flpBilgi);

            
            Panel pnlAdresSol = new Panel { Dock = DockStyle.Left, Width = 250, Padding = new Padding(10) };
            lstAdresler = new ListBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
            btnAdresSil = new Button { Text = "Seçili Adresi Sil", Dock = DockStyle.Bottom, Height = 40, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White };
            btnAdresSil.Click += (s, e) => AdresSil();
            pnlAdresSol.Controls.Add(lstAdresler);
            pnlAdresSol.Controls.Add(btnAdresSil);

            FlowLayoutPanel flpAdresSag = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            txtAdresBasligi = ModernInput(flpAdresSag, "Adres Başlığı (Örn: Evim, İş):");

            flpAdresSag.Controls.Add(new Label { Text = "Şehir:", Font = new Font("Segoe UI", 9F, FontStyle.Bold), Margin = new Padding(0, 10, 0, 0), AutoSize = true });
            cmbSehir = new ComboBox { Width = 280, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            cmbSehir.SelectedIndexChanged += cmbSehir_SelectedIndexChanged;
            flpAdresSag.Controls.Add(cmbSehir);

            flpAdresSag.Controls.Add(new Label { Text = "İlçe:", Font = new Font("Segoe UI", 9F, FontStyle.Bold), Margin = new Padding(0, 10, 0, 0), AutoSize = true });
            cmbIlce = new ComboBox { Width = 280, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            flpAdresSag.Controls.Add(cmbIlce);

            txtAdresTanimi = ModernInput(flpAdresSag, "Tam Adres Detayı:");
            txtAdresTanimi.Multiline = true; txtAdresTanimi.Height = 80;

            btnAdresEkle = ModernButton(flpAdresSag, "YENİ ADRES EKLE", Color.FromArgb(26, 115, 58));
            btnAdresEkle.Click += (s, e) => AdresEkle();

            tabAdresler.Controls.Add(flpAdresSag);
            tabAdresler.Controls.Add(pnlAdresSol);

            
            dgvSiparislerim = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false, AllowUserToOrderColumns = false, AllowUserToResizeColumns = false, AllowUserToResizeRows = false };
            tabSiparisler.Controls.Add(dgvSiparislerim);

            this.Controls.Add(tabControl);
            this.Controls.Add(pnlHeader);
        }

        private void VerileriYukle()
        {
            try
            {
                DataTable dtUser = DatabaseHelper.ExecuteQuery("SELECT * FROM Kullanicilar WHERE KullaniciID=" + kullaniciID);
                if (dtUser.Rows.Count > 0)
                {
                    txtAdSoyad.Text = dtUser.Rows[0]["AdSoyad"].ToString();
                    txtEmail.Text = dtUser.Rows[0]["Email"].ToString();
                    txtTelefon.Text = dtUser.Rows[0]["Telefon"].ToString();
                }

                cmbSehir.DataSource = DatabaseHelper.ExecuteQuery("SELECT Id, SehirAdi FROM Iller ORDER BY SehirAdi");
                cmbSehir.DisplayMember = "SehirAdi";
                cmbSehir.ValueMember = "Id";

                AdresleriYukle();
                dgvSiparislerim.DataSource = DatabaseHelper.ExecuteQuery("SELECT SiparisID, SiparisTarihi, ToplamTutar, Durum, OdemeYontemi FROM Siparisler WHERE KullaniciID=" + kullaniciID + " ORDER BY SiparisTarihi DESC");
            }
            catch (Exception ex) { MessageBox.Show("Veri hatası: " + ex.Message); }
        }

        private void cmbSehir_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSehir.SelectedValue != null && cmbSehir.SelectedValue is int sehirId)
            {
                cmbIlce.DataSource = DatabaseHelper.ExecuteQuery("SELECT Id, IlceAdi FROM Ilceler WHERE SehirId=" + sehirId + " ORDER BY IlceAdi");
                cmbIlce.DisplayMember = "IlceAdi";
                cmbIlce.ValueMember = "Id";
            }
        }

        private void AdresleriYukle()
        {
            lstAdresler.Items.Clear();
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Adresler WHERE KullaniciID=" + kullaniciID);
            foreach (DataRow row in dt.Rows)
            {
                lstAdresler.Items.Add(new AdresItem { ID = (int)row["AdresID"], Baslik = row["AdresBasligi"].ToString() });
            }
        }

        private void BilgileriGuncelle()
        {
            DatabaseHelper.ExecuteNonQuery("UPDATE Kullanicilar SET AdSoyad=@ad, Email=@mail, Telefon=@tel WHERE KullaniciID=@id",
                new SqlParameter[] { new SqlParameter("@ad", txtAdSoyad.Text), new SqlParameter("@mail", txtEmail.Text), new SqlParameter("@tel", txtTelefon.Text), new SqlParameter("@id", kullaniciID) });
            MessageBox.Show("Bilgileriniz başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SifreDegistir()
        {
            if (string.IsNullOrWhiteSpace(txtSifreEski.Text) || string.IsNullOrWhiteSpace(txtSifreYeni.Text)) { MessageBox.Show("Lütfen alanları boş bırakmayın!"); return; }
            if (txtSifreYeni.Text != txtSifreYeniTekrar.Text) { MessageBox.Show("Yeni şifreler eşleşmiyor!"); return; }

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT Sifre FROM Kullanicilar WHERE KullaniciID=" + kullaniciID);

            string hashliEskiSifre = SifreHashle(txtSifreEski.Text.Trim());
            if (dt.Rows[0]["Sifre"].ToString() != hashliEskiSifre) { MessageBox.Show("Mevcut şifreniz hatalı!"); return; }

            string hashliYeniSifre = SifreHashle(txtSifreYeni.Text.Trim());
            DatabaseHelper.ExecuteNonQuery("UPDATE Kullanicilar SET Sifre=@s WHERE KullaniciID=@id",
                new SqlParameter[] { new SqlParameter("@s", hashliYeniSifre), new SqlParameter("@id", kullaniciID) });

            MessageBox.Show("Şifreniz başarıyla değiştirildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtSifreEski.Clear(); txtSifreYeni.Clear(); txtSifreYeniTekrar.Clear();
        }

        private void AdresEkle()
        {
            if (string.IsNullOrEmpty(txtAdresBasligi.Text) || cmbIlce.SelectedValue == null) { MessageBox.Show("Lütfen tüm alanları doldurun!"); return; }

            DatabaseHelper.ExecuteNonQuery("INSERT INTO Adresler (KullaniciID, AdresBasligi, AdresTanimi, Sehir, Ilce) VALUES (@kid, @baslik, @tanim, @sehir, @ilce)",
                new SqlParameter[] {
                    new SqlParameter("@kid", kullaniciID),
                    new SqlParameter("@baslik", txtAdresBasligi.Text),
                    new SqlParameter("@tanim", txtAdresTanimi.Text),
                    new SqlParameter("@sehir", cmbSehir.Text),
                    new SqlParameter("@ilce", cmbIlce.Text)
                });

            txtAdresBasligi.Clear(); txtAdresTanimi.Clear();
            AdresleriYukle();
            MessageBox.Show("Adres kaydedildi.");
        }

        private void AdresSil()
        {
            if (lstAdresler.SelectedItem == null) return;
            int id = ((AdresItem)lstAdresler.SelectedItem).ID;
            DatabaseHelper.ExecuteNonQuery("DELETE FROM Adresler WHERE AdresID=" + id);
            AdresleriYukle();
        }

        private TextBox ModernInput(Control parent, string label)
        {
            parent.Controls.Add(new Label { Text = label, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Margin = new Padding(0, 10, 0, 0), AutoSize = true });
            TextBox t = new TextBox { Width = 280, Font = new Font("Segoe UI", 11F) };
            parent.Controls.Add(t);
            return t;
        }

        private Button ModernButton(Control parent, string text, Color c)
        {
            Button b = new Button { Text = text, Width = 280, Height = 40, BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(0, 15, 0, 0), Cursor = Cursors.Hand, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            b.FlatAppearance.BorderSize = 0;
            parent.Controls.Add(b);
            return b;
        }
    }
}