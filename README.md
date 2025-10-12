using System;
using Microsoft.Data.SqlClient;

namespace OkulUygulamasi
{
    internal class Program
    {
        // 🔗 Bağlantı bilgisi
        private static string connString = "Server=.;Database=OkulDB;Trusted_Connection=True;";

        static void Main()
        {
            Console.Title = "📚 Okul Yönetim Sistemi";
            bool devam = true;

            while (devam)
            {
                Console.Clear();
                Console.WriteLine("=== OKUL YÖNETİM SİSTEMİ ===");
                Console.WriteLine("1 - Sınıf Listele");
                Console.WriteLine("2 - Yeni Sınıf Ekle");
                Console.WriteLine("3 - Sınıf Güncelle");
                Console.WriteLine("0 - Çıkış");
                Console.Write("Seçiminiz: ");
                string secim = Console.ReadLine();

                switch (secim)
                {
                    case "1":
                        SinifListele();
                        break;
                    case "2":
                        SinifEkle();
                        break;
                    case "3":
                        SinifGuncelle();
                        break;
                    case "0":
                        devam = false;
                        Console.WriteLine("Programdan çıkılıyor...");
                        break;
                    default:
                        Console.WriteLine("❌ Geçersiz seçim!");
                        break;
                }

                if (devam)
                {
                    Console.WriteLine("\nDevam etmek için bir tuşa bas...");
                    Console.ReadKey();
                }
            }
        }

        // 🧾 Sınıf Listeleme
        static void SinifListele()
        {
            Console.Clear();
            Console.WriteLine("--- SINIF LİSTESİ ---");

            using (var conn = new SqlConnection(connString))
            {
                string sql = "SELECT SinifNo, SinifAdi, OkulAdi FROM Sinif";
                var cmd = new SqlCommand(sql, conn);

                try
                {
                    conn.Open();
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["SinifNo"],-5} | {reader["SinifAdi"],-15} | {reader["OkulAdi"],-15}");
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("⚠️ Hata: " + ex.Message);
                }
            }
        }

        // ➕ Yeni Sınıf Ekleme
        static void SinifEkle()
        {
            Console.Clear();
            Console.WriteLine("--- YENİ SINIF EKLE ---");

            Console.Write("Sınıf Adı: ");
            string sinifAdi = Console.ReadLine();

            Console.Write("Okul Adı: ");
            string okulAdi = Console.ReadLine();

            Console.Write("Sınıf No: ");
            if (!int.TryParse(Console.ReadLine(), out int sinifNo))
            {
                Console.WriteLine("⚠️ Hatalı numara girişi!");
                return;
            }

            string sql = "INSERT INTO Sinif (SinifNo, SinifAdi, OkulAdi) VALUES (@no, @adi, @okul)";

            using (var conn = new SqlConnection(connString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@no", sinifNo);
                cmd.Parameters.AddWithValue("@adi", sinifAdi);
                cmd.Parameters.AddWithValue("@okul", okulAdi);

                try
                {
                    conn.Open();
                    int etkilenen = cmd.ExecuteNonQuery();

                    if (etkilenen > 0)
                        Console.WriteLine("✅ Yeni sınıf başarıyla eklendi!");
                    else
                        Console.WriteLine("⚠️ Ekleme işlemi başarısız!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("⚠️ Hata: " + ex.Message);
                }
            }
        }

        // ✏️ Sınıf Güncelleme
        static void SinifGuncelle()
        {
            Console.Clear();
            Console.WriteLine("--- SINIF GÜNCELLE ---");

            Console.Write("Güncellenecek sınıf numarası: ");
            if (!int.TryParse(Console.ReadLine(), out int mevcutNo))
            {
                Console.WriteLine("⚠️ Hatalı numara girişi!");
                return;
            }

            Console.Write("Yeni Sınıf Adı: ");
            string yeniSinifAdi = Console.ReadLine();

            Console.Write("Yeni Okul Adı: ");
            string yeniOkulAdi = Console.ReadLine();

            string sql = @"
                UPDATE Sinif
                SET SinifAdi = @YeniAdi, OkulAdi = @YeniOkul
                WHERE SinifNo = @No";

            using (var conn = new SqlConnection(connString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@YeniAdi", yeniSinifAdi);
                cmd.Parameters.AddWithValue("@YeniOkul", yeniOkulAdi);
                cmd.Parameters.AddWithValue("@No", mevcutNo);

                try
                {
                    conn.Open();
                    int etkilenen = cmd.ExecuteNonQuery();

                    if (etkilenen > 0)
                        Console.WriteLine("✅ Kayıt başarıyla güncellendi!");
                    else
                        Console.WriteLine("⚠️ Güncelleme başarısız veya kayıt bulunamadı.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("⚠️ Hata: " + ex.Message);
                }
            }
        }
    }
}

