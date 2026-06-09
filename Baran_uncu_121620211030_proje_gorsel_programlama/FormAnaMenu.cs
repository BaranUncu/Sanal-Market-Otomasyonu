using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Baran_uncu_121620211030_proje_gorsel_programlama
{
    public partial class FormAnaMenu : Form
    {
        private int kullaniciID;
        private string kullaniciAd;
        private bool _isAdmin;
        private DataTable sepet;
        private int aktifKategoriID = 0; 

        private Panel pnlUst;
        private Panel pnlSol;
        private Panel pnlSag;
        private Label lblBaslik;
        private Label lblKullanici;
        private Button btnSepet;
        private Label lblKategoriler;
        private FlowLayoutPanel flpUrunler;
        private TextBox txtArama;
        private Button btnArama;

        public FormAnaMenu(int kullaniciID, string adSoyad, bool isAdmin)
        {
            this.kullaniciID = kullaniciID;
            this.kullaniciAd = adSoyad;
            this._isAdmin = isAdmin;

            this.sepet = new DataTable();
            this.sepet.Columns.Add("UrunID", typeof(int));
            this.sepet.Columns.Add("UrunAdi", typeof(string));
            this.sepet.Columns.Add("Fiyat", typeof(decimal));
            this.sepet.Columns.Add("Miktar", typeof(int));

            this.Text = "Sanal Market";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.MinimumSize = new Size(1000, 700);

            TasarimiOlustur();
            KategorileriYukle();
        }

        private void TasarimiOlustur()
        {
            pnlUst = new Panel { Width = this.Width, BackColor = Color.FromArgb(26, 115, 58), Dock = DockStyle.Top, Height = 65 };

            lblBaslik = new Label { Text = "🛒 SANAL MARKET", Font = new Font("Segoe UI", 18F, FontStyle.Bold), ForeColor = Color.White, Location = new Point(20, 15), Size = new Size(300, 40) };

            lblKullanici = new Label { Text = "Merhaba, " + kullaniciAd, Font = new Font("Segoe UI", 10F), ForeColor = Color.FromArgb(200, 255, 200), AutoSize = false, Size = new Size(350, 40), Location = new Point(350, 15), TextAlign = ContentAlignment.MiddleRight, Anchor = AnchorStyles.Top | AnchorStyles.Right };

            Button btnHesap = new Button { Text = "Hesabim", Font = new Font("Segoe UI", 10F, FontStyle.Bold), BackColor = Color.White, ForeColor = Color.FromArgb(26, 115, 58), FlatStyle = FlatStyle.Flat, Location = new Point(720, 15), Size = new Size(110, 38), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnHesap.FlatAppearance.BorderSize = 0;
            btnHesap.Click += (s, e) => {
                new FormHesap(kullaniciID).ShowDialog();
                DataTable dtGuncel = DatabaseHelper.ExecuteQuery("SELECT AdSoyad FROM Kullanicilar WHERE KullaniciID=" + kullaniciID);
                if (dtGuncel.Rows.Count > 0)
                {
                    kullaniciAd = dtGuncel.Rows[0]["AdSoyad"].ToString();
                    lblKullanici.Text = "Merhaba, " + kullaniciAd;
                }
            };

            Button btnAdmin = new Button { Text = "Admin", Font = new Font("Segoe UI", 10F, FontStyle.Bold), BackColor = Color.FromArgb(40, 40, 40), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(975, 15), Size = new Size(80, 38), Cursor = Cursors.Hand };
            btnAdmin.FlatAppearance.BorderSize = 0;
            btnAdmin.Click += (s, e) => { new FormAdminPanel().ShowDialog(); };
            btnAdmin.Visible = _isAdmin;
            pnlUst.Controls.Add(btnAdmin);

            btnSepet = new Button { Text = "🛒 Sepetim (0)", Font = new Font("Segoe UI", 10F, FontStyle.Bold), BackColor = Color.FromArgb(255, 165, 0), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(850, 15), Size = new Size(120, 38), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnSepet.FlatAppearance.BorderSize = 0;
            btnSepet.Click += new EventHandler(btnSepet_Click);

            pnlUst.Controls.Add(lblBaslik);

            txtArama = new TextBox { Font = new Font("Segoe UI", 11F), Location = new Point(330, 18), Size = new Size(270, 35), BorderStyle = BorderStyle.FixedSingle, Text = "Urun ara...", ForeColor = Color.Gray };
            txtArama.Enter += (s, e) => { if (txtArama.Text == "Urun ara...") { txtArama.Text = ""; txtArama.ForeColor = Color.Black; } };
            txtArama.Leave += (s, e) => { if (txtArama.Text == "") { txtArama.Text = "Urun ara..."; txtArama.ForeColor = Color.Gray; } };
            txtArama.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) AramaYap(); };

            btnArama = new Button { Text = "Ara", Font = new Font("Segoe UI", 10F, FontStyle.Bold), BackColor = Color.FromArgb(255, 165, 0), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(608, 15), Size = new Size(60, 38), Cursor = Cursors.Hand };
            btnArama.FlatAppearance.BorderSize = 0;
            btnArama.Click += (s, e) => AramaYap();

            pnlUst.Controls.Add(txtArama);
            pnlUst.Controls.Add(btnArama);
            pnlUst.Controls.Add(lblKullanici);
            pnlUst.Controls.Add(btnHesap);
            pnlUst.Controls.Add(btnSepet);

            pnlSol = new Panel { BackColor = Color.White, Dock = DockStyle.Left, Width = 200, Padding = new Padding(10) };
            lblKategoriler = new Label { Text = "KATEGORİLER", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(26, 115, 58), Location = new Point(10, 10), Size = new Size(180, 25) };
            pnlSol.Controls.Add(lblKategoriler);

            pnlSag = new Panel { BackColor = Color.FromArgb(245, 245, 245), Dock = DockStyle.Fill, Padding = new Padding(10), AutoScroll = true };
            flpUrunler = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, FlowDirection = FlowDirection.LeftToRight, WrapContents = true, Padding = new Padding(5) };
            pnlSag.Controls.Add(flpUrunler);

            this.Controls.Add(pnlSag);
            this.Controls.Add(pnlSol);
            this.Controls.Add(pnlUst);
        }

        private void KategorileriYukle()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Kategoriler");

            Button btnTumu = new Button { Text = "Tümü", Font = new Font("Segoe UI", 10F), BackColor = Color.FromArgb(26, 115, 58), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(175, 40), Location = new Point(10, 45), Cursor = Cursors.Hand, Tag = 0 };
            btnTumu.FlatAppearance.BorderSize = 0;
            btnTumu.Click += new EventHandler(btnKategori_Click);
            pnlSol.Controls.Add(btnTumu);

            int yPos = 95;
            foreach (DataRow row in dt.Rows)
            {
                Button btn = new Button { Text = row["KategoriAdi"].ToString(), Font = new Font("Segoe UI", 10F), BackColor = Color.White, ForeColor = Color.FromArgb(50, 50, 50), FlatStyle = FlatStyle.Flat, Size = new Size(175, 40), Location = new Point(10, yPos), Cursor = Cursors.Hand, Tag = row["KategoriID"] };
                btn.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
                btn.Click += new EventHandler(btnKategori_Click);
                pnlSol.Controls.Add(btn);
                yPos += 48;
            }

            UrunleriYukle(0);
        }

        private void btnKategori_Click(object sender, EventArgs e)
        {
            Button secilenBtn = (Button)sender;
            foreach (Control ctrl in pnlSol.Controls)
            {
                if (ctrl is Button)
                {
                    Button btn = (Button)ctrl;
                    btn.BackColor = Color.White;
                    btn.ForeColor = Color.FromArgb(50, 50, 50);
                }
            }
            secilenBtn.BackColor = Color.FromArgb(26, 115, 58);
            secilenBtn.ForeColor = Color.White;

            aktifKategoriID = Convert.ToInt32(secilenBtn.Tag);
            UrunleriYukle(aktifKategoriID);
        }

        private void UrunleriYukle(int kategoriID)
        {
            flpUrunler.Controls.Clear();
            string query = kategoriID == 0 ? "SELECT * FROM Urunler WHERE Stok > 0" : "SELECT * FROM Urunler WHERE KategoriID=" + kategoriID + " AND Stok > 0";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                flpUrunler.Controls.Add(UrunKartiOlustur(row));
            }
        }

        
        private int GetSepetMiktar(int urunID)
        {
            foreach (DataRow row in sepet.Rows)
            {
                if (Convert.ToInt32(row["UrunID"]) == urunID)
                    return Convert.ToInt32(row["Miktar"]);
            }
            return 0;
        }

        private Panel UrunKartiOlustur(DataRow row)
        {
            int urunID = Convert.ToInt32(row["UrunID"]);
            string urunAdi = row["UrunAdi"].ToString();
            decimal fiyat = Convert.ToDecimal(row["Fiyat"]);

            Panel kart = new Panel { Size = new Size(200, 300), BackColor = Color.White, Margin = new Padding(8), Cursor = Cursors.Hand };

            PictureBox pb = new PictureBox { Location = new Point(10, 10), Size = new Size(180, 100), SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.FromArgb(245, 245, 245) };
            Bitmap placeholder = new Bitmap(180, 100);
            using (Graphics g = Graphics.FromImage(placeholder))
            {
                g.Clear(Color.FromArgb(240, 240, 240));
                g.DrawString("Resim", new Font("Segoe UI", 9F), Brushes.Gray, new RectangleF(0, 0, 180, 100), new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
            pb.Image = placeholder;

            Label lblAd = new Label { Text = urunAdi, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(40, 40, 40), Location = new Point(10, 118), Size = new Size(180, 35) };
            Label lblAciklama = new Label { Text = row["Aciklama"].ToString(), Font = new Font("Segoe UI", 8F), ForeColor = Color.Gray, Location = new Point(10, 153), Size = new Size(180, 20) };
            Label lblFiyat = new Label { Text = fiyat.ToString("C2", new System.Globalization.CultureInfo("tr-TR")), Font = new Font("Segoe UI", 13F, FontStyle.Bold), ForeColor = Color.FromArgb(26, 115, 58), Location = new Point(10, 175), Size = new Size(180, 30) };

            
            Button btnEkle = new Button { Text = "+ Sepete Ekle", Font = new Font("Segoe UI", 9F, FontStyle.Bold), BackColor = Color.FromArgb(26, 115, 58), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(10, 215), Size = new Size(180, 38), Cursor = Cursors.Hand };
            btnEkle.FlatAppearance.BorderSize = 0;

            
            Panel pnlMiktar = new Panel { Location = new Point(10, 215), Size = new Size(180, 38), BackColor = Color.White };

            Button btnEksi = new Button { Text = "-", Font = new Font("Segoe UI", 12F, FontStyle.Bold), BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(0, 0), Size = new Size(40, 38), Cursor = Cursors.Hand };
            btnEksi.FlatAppearance.BorderSize = 0;

            Label lblMiktar = new Label { Text = "1", Font = new Font("Segoe UI", 12F, FontStyle.Bold), ForeColor = Color.FromArgb(40, 40, 40), Location = new Point(40, 0), Size = new Size(100, 38), TextAlign = ContentAlignment.MiddleCenter, BorderStyle = BorderStyle.FixedSingle };

            Button btnArti = new Button { Text = "+", Font = new Font("Segoe UI", 12F, FontStyle.Bold), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(140, 0), Size = new Size(40, 38), Cursor = Cursors.Hand };
            btnArti.FlatAppearance.BorderSize = 0;

            pnlMiktar.Controls.AddRange(new Control[] { btnEksi, lblMiktar, btnArti });

            kart.Controls.AddRange(new Control[] { pb, lblAd, lblAciklama, lblFiyat, btnEkle, pnlMiktar });

            
            int miktar = GetSepetMiktar(urunID);
            if (miktar > 0)
            {
                btnEkle.Visible = false;
                pnlMiktar.Visible = true;
                lblMiktar.Text = miktar.ToString();
            }
            else
            {
                btnEkle.Visible = true;
                pnlMiktar.Visible = false;
            }

            
            btnEkle.Click += (s, e) =>
            {
                sepet.Rows.Add(urunID, urunAdi, fiyat, 1);
                SepetGuncelle();
                btnEkle.Visible = false; 
                pnlMiktar.Visible = true; 
                lblMiktar.Text = "1";
            };

            btnArti.Click += (s, e) =>
            {
                foreach (DataRow r in sepet.Rows)
                {
                    if (Convert.ToInt32(r["UrunID"]) == urunID)
                    {
                        int yeniMiktar = Convert.ToInt32(r["Miktar"]) + 1;
                        r["Miktar"] = yeniMiktar;
                        lblMiktar.Text = yeniMiktar.ToString();
                        SepetGuncelle();
                        break;
                    }
                }
            };

            btnEksi.Click += (s, e) =>
            {
                for (int i = sepet.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow r = sepet.Rows[i];
                    if (Convert.ToInt32(r["UrunID"]) == urunID)
                    {
                        int mevcut = Convert.ToInt32(r["Miktar"]);
                        if (mevcut > 1)
                        {
                            r["Miktar"] = mevcut - 1;
                            lblMiktar.Text = (mevcut - 1).ToString();
                        }
                        else
                        {
                            sepet.Rows.RemoveAt(i); 
                            pnlMiktar.Visible = false; 
                            btnEkle.Visible = true;    
                        }
                        SepetGuncelle();
                        break;
                    }
                }
            };

            return kart;
        }

        private void SepetGuncelle()
        {
            int toplamUrun = 0;
            foreach (DataRow row in sepet.Rows) toplamUrun += Convert.ToInt32(row["Miktar"]);
            btnSepet.Text = "🛒 Sepetim (" + toplamUrun + ")";
        }

        private void btnSepet_Click(object sender, EventArgs e)
        {
            if (sepet.Rows.Count == 0) { MessageBox.Show("Sepetiniz boş!", "Sepet", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            FormSepet formSepet = new FormSepet(kullaniciID, sepet);
            formSepet.ShowDialog();

            
            SepetGuncelle();
            UrunleriYukle(aktifKategoriID);
        }

        private void AramaYap()
        {
            string aranan = txtArama.Text.Trim();
            if (aranan == "Urun ara..." || string.IsNullOrEmpty(aranan)) { UrunleriYukle(0); return; }

            flpUrunler.Controls.Clear();
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Urunler WHERE UrunAdi LIKE @Ara AND Stok > 0", new SqlParameter[] { new SqlParameter("@Ara", "%" + aranan + "%") });

            if (dt.Rows.Count == 0)
            {
                flpUrunler.Controls.Add(new Label { Text = "'" + aranan + "' icin sonuc bulunamadi.", Font = new Font("Segoe UI", 12F), ForeColor = Color.Gray, Size = new Size(500, 30), Margin = new Padding(20) });
                return;
            }
            foreach (DataRow row in dt.Rows) flpUrunler.Controls.Add(UrunKartiOlustur(row));
        }
    }
}