using System;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Baran_uncu_121620211030_proje_gorsel_programlama
{
    public class FormSepet : Form
    {
        private int kullaniciID;
        private DataTable sepet;
        private Panel pnlUst;
        private Label lblBaslik;
        private DataGridView dgvSepet;
        private ComboBox cmbAdresler;
        private RadioButton rbKrediKarti, rbNakit, rbHavale;
        private Label lblToplamFiyat;
        private Button btnSiparisVer, btnGeriDon;

        public FormSepet(int kullaniciID, DataTable sepet)
        {
            this.kullaniciID = kullaniciID;
            this.sepet = sepet;
            this.Text = "Sepetim - Güvenli Ödeme";
            this.Size = new Size(650, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            TasarimiOlustur();
            SepetYukle();
            AdresleriYukle();
        }

        private void TasarimiOlustur()
        {
            pnlUst = new Panel { BackColor = Color.FromArgb(26, 115, 58), Dock = DockStyle.Top, Height = 65 };
            lblBaslik = new Label { Text = "🛒 Sepetim", Font = new Font("Segoe UI", 18F, FontStyle.Bold), ForeColor = Color.White, Location = new Point(20, 15), Size = new Size(300, 40) };
            pnlUst.Controls.Add(lblBaslik);

            dgvSepet = new DataGridView
            {
                Location = new Point(20, 85),
                Size = new Size(600, 220),
                BackgroundColor = Color.White,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                AllowUserToOrderColumns = false,    
                AllowUserToResizeColumns = false,  
                AllowUserToResizeRows = false       
            };

            Label lblAdres = new Label { Text = "Teslimat Adresi Seçin:", Font = new Font("Segoe UI", 10F, FontStyle.Bold), Location = new Point(20, 320), Size = new Size(200, 20) };
            cmbAdresler = new ComboBox { Location = new Point(20, 345), Size = new Size(600, 30), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };

            Label lblOdeme = new Label { Text = "Ödeme Yöntemi Belirleyin:", Font = new Font("Segoe UI", 10F, FontStyle.Bold), Location = new Point(20, 390), Size = new Size(200, 20) };
            rbKrediKarti = new RadioButton { Text = "Kredi/Banka Kartı", Location = new Point(20, 415), Size = new Size(150, 20), Checked = true };
            rbNakit = new RadioButton { Text = "Kapıda Nakit", Location = new Point(180, 415), Size = new Size(120, 20) };
            rbHavale = new RadioButton { Text = "Havale / EFT", Location = new Point(310, 415), Size = new Size(120, 20) };

            lblToplamFiyat = new Label { Text = "Toplam: 0,00 TL", Font = new Font("Segoe UI", 14F, FontStyle.Bold), ForeColor = Color.FromArgb(26, 115, 58), Location = new Point(20, 480), Size = new Size(300, 35) };

            btnSiparisVer = new Button { Text = "SİPARİŞİ ONAYLA", Font = new Font("Segoe UI", 11F, FontStyle.Bold), BackColor = Color.FromArgb(26, 115, 58), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(370, 475), Size = new Size(250, 45), Cursor = Cursors.Hand };
            btnSiparisVer.Click += BtnSiparisVer_Click;

            btnGeriDon = new Button { Text = "← Alışverişe Devam Et", Font = new Font("Segoe UI", 10F), FlatStyle = FlatStyle.Flat, Location = new Point(20, 530), Size = new Size(250, 38), Cursor = Cursors.Hand };
            btnGeriDon.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { pnlUst, dgvSepet, lblAdres, cmbAdresler, lblOdeme, rbKrediKarti, rbNakit, rbHavale, lblToplamFiyat, btnSiparisVer, btnGeriDon });
        }

        private void AdresleriYukle()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT AdresID, AdresBasligi + ' (' + Sehir + ')' as Gorunum FROM Adresler WHERE KullaniciID=" + kullaniciID);
            cmbAdresler.DataSource = dt;
            cmbAdresler.DisplayMember = "Gorunum";
            cmbAdresler.ValueMember = "AdresID";
        }

        private void SepetYukle()
        {
            dgvSepet.DataSource = null;
            dgvSepet.Columns.Clear();
            dgvSepet.DataSource = sepet;

            if (dgvSepet.Columns.Contains("UrunID")) dgvSepet.Columns["UrunID"].Visible = false;
            if (dgvSepet.Columns.Contains("UrunAdi")) dgvSepet.Columns["UrunAdi"].HeaderText = "Ürün Adı";
            if (dgvSepet.Columns.Contains("Fiyat")) dgvSepet.Columns["Fiyat"].HeaderText = "Birim Fiyat";
            if (dgvSepet.Columns.Contains("Miktar")) dgvSepet.Columns["Miktar"].HeaderText = "Adet";

            
            DataGridViewButtonColumn btnArti = new DataGridViewButtonColumn();
            btnArti.Name = "colArti";
            btnArti.HeaderText = ""; 
            btnArti.Text = "+";
            btnArti.UseColumnTextForButtonValue = true;
            btnArti.Width = 40;
            btnArti.FlatStyle = FlatStyle.Flat;
            dgvSepet.Columns.Add(btnArti);

            
            DataGridViewButtonColumn btnEksi = new DataGridViewButtonColumn();
            btnEksi.Name = "colEksi";
            btnEksi.HeaderText = ""; // Başlığı boş bıraktık
            btnEksi.Text = "-";
            btnEksi.UseColumnTextForButtonValue = true;
            btnEksi.Width = 40;
            btnEksi.FlatStyle = FlatStyle.Flat;
            dgvSepet.Columns.Add(btnEksi);

            FiyatiGuncelle();

            
            dgvSepet.CellContentClick -= DgvSepet_CellContentClick;
            dgvSepet.CellContentClick += DgvSepet_CellContentClick;
        }

        private void FiyatiGuncelle()
        {
            decimal toplam = 0;
            foreach (DataRow r in sepet.Rows)
                toplam += Convert.ToDecimal(r["Fiyat"]) * Convert.ToInt32(r["Miktar"]);
            lblToplamFiyat.Text = "Toplam: " + toplam.ToString("C2", new System.Globalization.CultureInfo("tr-TR"));
        }

        private void DgvSepet_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; 
            string colName = dgvSepet.Columns[e.ColumnIndex].Name;

            if (colName == "colArti")
            {
                sepet.Rows[e.RowIndex]["Miktar"] = Convert.ToInt32(sepet.Rows[e.RowIndex]["Miktar"]) + 1;
                FiyatiGuncelle();
            }
            else if (colName == "colEksi")
            {
                int mevcutMiktar = Convert.ToInt32(sepet.Rows[e.RowIndex]["Miktar"]);
                if (mevcutMiktar > 1)
                {
                    sepet.Rows[e.RowIndex]["Miktar"] = mevcutMiktar - 1;
                }
                else
                {
                    sepet.Rows.RemoveAt(e.RowIndex); 
                }
                FiyatiGuncelle();
            }
        }

        private void BtnSiparisVer_Click(object sender, EventArgs e)
        {
            if (sepet.Rows.Count == 0) { MessageBox.Show("Sepetiniz boş!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (cmbAdresler.SelectedValue == null) { MessageBox.Show("Lütfen bir adres seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            string odeme = rbKrediKarti.Checked ? "Kredi Kartı" : rbNakit.Checked ? "Kapıda Nakit" : "Havale";
            decimal toplam = 0;
            foreach (DataRow r in sepet.Rows) toplam += Convert.ToDecimal(r["Fiyat"]) * Convert.ToInt32(r["Miktar"]);

            string sipQuery = "INSERT INTO Siparisler (KullaniciID, ToplamTutar, Durum, AdresID, OdemeYontemi) VALUES (@kid, @tutar, @durum, @aid, @odeme); SELECT SCOPE_IDENTITY();";
            int sid = Convert.ToInt32(DatabaseHelper.ExecuteScalar(sipQuery, new SqlParameter[] {
                new SqlParameter("@kid", kullaniciID), new SqlParameter("@tutar", toplam),
                new SqlParameter("@durum", "Hazırlanıyor"), new SqlParameter("@aid", cmbAdresler.SelectedValue),
                new SqlParameter("@odeme", odeme)
            }));

            foreach (DataRow r in sepet.Rows)
            {
                DatabaseHelper.ExecuteNonQuery("INSERT INTO SiparisDetay (SiparisID, UrunID, Miktar, BirimFiyat) VALUES (" + sid + "," + r["UrunID"] + "," + r["Miktar"] + ",@fiyat)",
                    new SqlParameter[] { new SqlParameter("@fiyat", r["Fiyat"]) });
                DatabaseHelper.ExecuteNonQuery("UPDATE Urunler SET Stok = Stok - " + r["Miktar"] + " WHERE UrunID=" + r["UrunID"]);
            }

            sepet.Rows.Clear();
            MessageBox.Show($"Siparişiniz başarıyla oluşturuldu!\n\nÖdeme Yöntemi: {odeme}\nDurum: Hazırlanıyor\n\nBizi tercih ettiğiniz için teşekkür ederiz.", "Sipariş Onaylandı ✅", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}