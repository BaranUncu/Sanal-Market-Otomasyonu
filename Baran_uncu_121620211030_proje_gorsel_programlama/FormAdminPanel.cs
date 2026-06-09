using System;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Baran_uncu_121620211030_proje_gorsel_programlama
{
	public partial class FormAdminPanel : Form
	{
		private TabControl tabAdmin;
		private TabPage tabUrunler, tabUyeler, tabSiparisler, tabIstatistik; // Istatistik Eklendi

		// Ürünler Sekmesi Araçları
		private DataGridView dgvUrunler;
		private TextBox txtUrunAdi, txtFiyat, txtStok;
		private ComboBox cmbKategori;
		private Label lblAdminBaslik;
		private int secilenUrunID = -1;

		// Üyeler ve Siparişler Araçları
		private DataGridView dgvUyeler, dgvTumSiparisler, dgvSiparisDetay;

		// İstatistik Araçları
		private DataGridView dgvIstatistik, dgvPopuler, dgvDusukStok;

		public FormAdminPanel()
		{
			this.Text = "Sanal Market - Yönetim Paneli";
			this.Size = new Size(1100, 700);
			this.StartPosition = FormStartPosition.CenterScreen;
			this.BackColor = Color.White;

			TasarimiOlustur();
			VerileriYukle();
		}

		private void TasarimiOlustur()
		{
			// Üst Panel
			Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(44, 62, 80) };
			lblAdminBaslik = new Label { Text = "📦 YÖNETİCİ KONTROL MERKEZİ", ForeColor = Color.White, Font = new Font("Segoe UI", 16F, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
			pnlHeader.Controls.Add(lblAdminBaslik);

			// Sekme Yapısı
			tabAdmin = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) }; // HATA BURADAYDI, ÖNCE BU OLUŞTURULMALI!
			tabUrunler = new TabPage(" Ürün Yönetimi ");
			tabUyeler = new TabPage(" Üye Listesi ");
			tabSiparisler = new TabPage(" Sipariş Takibi ");
			tabIstatistik = new TabPage(" İstatistikler "); // YENİ

			tabAdmin.TabPages.AddRange(new TabPage[] { tabUrunler, tabUyeler, tabSiparisler, tabIstatistik });

			// 1. ÜRÜN SEKMESİ
			dgvUrunler = new DataGridView { Location = new Point(10, 10), Size = new Size(680, 560), SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, AllowUserToOrderColumns = false, AllowUserToResizeColumns = false, AllowUserToResizeRows = false };
			dgvUrunler.CellClick += DgvUrunler_CellClick;
			tabUrunler.Controls.Add(dgvUrunler);

			FlowLayoutPanel flpUrun = new FlowLayoutPanel { Location = new Point(700, 10), Size = new Size(360, 560), AutoScroll = true };
			txtUrunAdi = ModernInput(flpUrun, "Ürün Adı:");
			txtFiyat = ModernInput(flpUrun, "Fiyat (TL):");
			txtStok = ModernInput(flpUrun, "Stok Miktarı:");

			flpUrun.Controls.Add(new Label { Text = "Kategori:", Font = new Font("Segoe UI", 9F, FontStyle.Bold), Margin = new Padding(0, 10, 0, 0), AutoSize = true });
			cmbKategori = new ComboBox { Width = 320, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
			flpUrun.Controls.Add(cmbKategori);

			Button btnE = ModernButton(flpUrun, "YENİ ÜRÜN EKLE", Color.FromArgb(46, 204, 113)); btnE.Click += BtnEkle_Click;
			Button btnG = ModernButton(flpUrun, "BİLGİLERİ GÜNCELLE", Color.FromArgb(52, 152, 219)); btnG.Click += BtnGuncelle_Click;
			Button btnS = ModernButton(flpUrun, "ÜRÜNÜ SİL", Color.FromArgb(231, 76, 60)); btnS.Click += BtnSil_Click;
			Button btnT = ModernButton(flpUrun, "KUTULARI TEMİZLE", Color.Gray); btnT.Click += (s, e) => FormuTemizle();
			tabUrunler.Controls.Add(flpUrun);

			// 2. ÜYELER SEKMESİ
			dgvUyeler = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, AllowUserToOrderColumns = false, AllowUserToResizeColumns = false, AllowUserToResizeRows = false };
			tabUyeler.Controls.Add(dgvUyeler);

			// 3. SİPARİŞLER SEKMESİ
			Label l1 = new Label { Text = "Tüm Siparişler", Font = new Font("Segoe UI", 12F, FontStyle.Bold), Location = new Point(10, 10), AutoSize = true };
			dgvTumSiparisler = new DataGridView { Location = new Point(10, 40), Size = new Size(1050, 240), SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, AllowUserToOrderColumns = false, AllowUserToResizeColumns = false, AllowUserToResizeRows = false };
			dgvTumSiparisler.SelectionChanged += (s, e) => SiparisDetayYukle();

			Label l2 = new Label { Text = "Sipariş Detayı (Ürünler)", Font = new Font("Segoe UI", 12F, FontStyle.Bold), Location = new Point(10, 290), AutoSize = true };
			dgvSiparisDetay = new DataGridView { Location = new Point(10, 320), Size = new Size(1050, 240), ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, AllowUserToOrderColumns = false, AllowUserToResizeColumns = false, AllowUserToResizeRows = false };
			tabSiparisler.Controls.AddRange(new Control[] { l1, dgvTumSiparisler, l2, dgvSiparisDetay });

			// 4. İSTATİSTİKLER SEKMESİ
			Label lblKatIstat = new Label { Text = "Kategori İstatistikleri:", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(26, 115, 58), Location = new Point(10, 10), Size = new Size(300, 25) };
			dgvIstatistik = new DataGridView { Location = new Point(10, 38), Size = new Size(1050, 150), BackgroundColor = Color.White, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false, AllowUserToAddRows = false, AllowUserToOrderColumns = false, AllowUserToResizeColumns = false, AllowUserToResizeRows = false };

			Label lblPopuler = new Label { Text = "En Çok Satan Ürünler:", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(26, 115, 58), Location = new Point(10, 200), Size = new Size(300, 25) };
			dgvPopuler = new DataGridView { Location = new Point(10, 228), Size = new Size(500, 280), BackgroundColor = Color.White, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false, AllowUserToAddRows = false, AllowUserToOrderColumns = false, AllowUserToResizeColumns = false, AllowUserToResizeRows = false };

			Label lblDusuk = new Label { Text = "Düşük Stok Uyarısı (Son 10):", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(200, 50, 50), Location = new Point(530, 200), Size = new Size(300, 25) };
			dgvDusukStok = new DataGridView { Location = new Point(530, 228), Size = new Size(530, 280), BackgroundColor = Color.White, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false, AllowUserToAddRows = false, AllowUserToOrderColumns = false, AllowUserToResizeColumns = false, AllowUserToResizeRows = false };

			tabIstatistik.Controls.AddRange(new Control[] { lblKatIstat, dgvIstatistik, lblPopuler, dgvPopuler, lblDusuk, dgvDusukStok });

			this.Controls.Add(tabAdmin);
			this.Controls.Add(pnlHeader);
		}

		private void VerileriYukle()
		{
			try
			{
				// Ürünler, Kategoriler ve Üyeler
				dgvUrunler.DataSource = DatabaseHelper.ExecuteQuery("SELECT UrunID, UrunAdi, Fiyat, Stok FROM Urunler");
				cmbKategori.DataSource = DatabaseHelper.ExecuteQuery("SELECT KategoriID, KategoriAdi FROM Kategoriler");
				cmbKategori.DisplayMember = "KategoriAdi";
				cmbKategori.ValueMember = "KategoriID";
				dgvUyeler.DataSource = DatabaseHelper.ExecuteQuery("SELECT KullaniciID, AdSoyad, Email, Telefon, Yetki FROM Kullanicilar");

				// Siparişler
				dgvTumSiparisler.DataSource = DatabaseHelper.ExecuteQuery(@"SELECT s.SiparisID, k.AdSoyad as Musteri, s.ToplamTutar, s.SiparisTarihi, s.Durum, s.OdemeYontemi FROM Siparisler s JOIN Kullanicilar k ON s.KullaniciID = k.KullaniciID ORDER BY s.SiparisTarihi DESC");

				// İstatistikler (Senin oluşturduğun SQL View'ları kullanıyoruz)
				dgvIstatistik.DataSource = DatabaseHelper.ExecuteQuery("SELECT * FROM v_KategoriIstatistik");
				try { dgvPopuler.DataSource = DatabaseHelper.ExecuteQuery("SELECT * FROM v_PopulerUrunler"); } catch { }
				dgvDusukStok.DataSource = DatabaseHelper.ExecuteQuery("SELECT * FROM v_DusukStok");
			}
			catch (Exception ex) { MessageBox.Show("Veri yükleme hatası: " + ex.Message); }
		}

		private void SiparisDetayYukle()
		{
			if (dgvTumSiparisler.SelectedRows.Count == 0) return;
			int sid = Convert.ToInt32(dgvTumSiparisler.SelectedRows[0].Cells["SiparisID"].Value);
			dgvSiparisDetay.DataSource = DatabaseHelper.ExecuteQuery(@"SELECT u.UrunAdi, sd.Miktar, sd.BirimFiyat FROM SiparisDetay sd JOIN Urunler u ON sd.UrunID = u.UrunID WHERE sd.SiparisID = " + sid);
		}

		private void DgvUrunler_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0) return;
			var row = dgvUrunler.Rows[e.RowIndex];
			secilenUrunID = (int)row.Cells["UrunID"].Value;
			txtUrunAdi.Text = row.Cells["UrunAdi"].Value.ToString();
			txtFiyat.Text = row.Cells["Fiyat"].Value.ToString();
			txtStok.Text = row.Cells["Stok"].Value.ToString();
			lblAdminBaslik.Text = "🛠 Düzenleniyor: " + txtUrunAdi.Text;
		}

		private void BtnEkle_Click(object sender, EventArgs e)
		{
			try
			{
				decimal fiyat = decimal.Parse(txtFiyat.Text.Replace(".", ","));
				int stok = int.Parse(txtStok.Text);
				DatabaseHelper.ExecuteNonQuery("INSERT INTO Urunler (UrunAdi, Fiyat, Stok, KategoriID) VALUES (@ad, @f, @s, @k)",
					new SqlParameter[] { new SqlParameter("@ad", txtUrunAdi.Text), new SqlParameter("@f", fiyat), new SqlParameter("@s", stok), new SqlParameter("@k", cmbKategori.SelectedValue) });
				VerileriYukle(); FormuTemizle();
				MessageBox.Show("Ürün başarıyla eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
		}

		private void BtnGuncelle_Click(object sender, EventArgs e)
		{
			if (secilenUrunID == -1) return;
			try
			{
				decimal fiyat = decimal.Parse(txtFiyat.Text.Replace(".", ","));
				DatabaseHelper.ExecuteNonQuery("UPDATE Urunler SET UrunAdi=@ad, Fiyat=@f, Stok=@s, KategoriID=@k WHERE UrunID=@id",
					new SqlParameter[] { new SqlParameter("@ad", txtUrunAdi.Text), new SqlParameter("@f", fiyat), new SqlParameter("@s", txtStok.Text), new SqlParameter("@k", cmbKategori.SelectedValue), new SqlParameter("@id", secilenUrunID) });
				VerileriYukle();
				MessageBox.Show("Ürün güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
		}

		private void BtnSil_Click(object sender, EventArgs e)
		{
			if (secilenUrunID == -1) return;
			if (MessageBox.Show("Silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				DatabaseHelper.ExecuteNonQuery("DELETE FROM Urunler WHERE UrunID=" + secilenUrunID);
				VerileriYukle(); FormuTemizle();
			}
		}

		private void FormuTemizle()
		{
			secilenUrunID = -1; txtUrunAdi.Clear(); txtFiyat.Clear(); txtStok.Clear();
			lblAdminBaslik.Text = "📦 YÖNETİCİ KONTROL MERKEZİ";
		}

		private TextBox ModernInput(FlowLayoutPanel flp, string label)
		{
			flp.Controls.Add(new Label { Text = label, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Margin = new Padding(0, 10, 0, 0), AutoSize = true });
			TextBox t = new TextBox { Width = 320, Font = new Font("Segoe UI", 11F) };
			flp.Controls.Add(t);
			return t;
		}

		private Button ModernButton(FlowLayoutPanel flp, string text, Color c)
		{
			Button b = new Button { Text = text, Width = 320, Height = 40, BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(0, 10, 0, 0), Cursor = Cursors.Hand, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
			b.FlatAppearance.BorderSize = 0;
			flp.Controls.Add(b);
			return b;
		}
	}
}