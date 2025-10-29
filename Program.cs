//using System;
//using Microsoft.EntityFrameworkCore;

//namespace StudentApp
//{
//    // Model sınıfı
//    public class Student
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }
//    }

//    // DbContext sınıfı
//    public class AppDbContext : DbContext
//    {
//        public DbSet<Student> Students { get; set; }

//        // Veritabanı bağlantısı (örnek olarak SQLite kullanıldı)
//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            optionsBuilder.UseSqlite("Data Source=students.db");
//        }
//    }

//    // Uygulama girişi
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            using (var context = new AppDbContext())
//            {
//                // Eğer veritabanı yoksa oluştur
//                context.Database.EnsureCreated();

//                // Yeni öğrenci nesnesi oluştur
//                var student = new Student
//                {
//                    Name = "Ahmet Yılmaz"
//                };

//                // Öğrenciyi DbSet'e ekle
//                context.Students.Add(student);

//                // Değişiklikleri veritabanına kaydet
//                context.SaveChanges();

//                Console.WriteLine("Yeni öğrenci başarıyla eklendi!");
//            }
//        }
//    }
//}




//using System;

//SaveChanges() metodunun görevi:

//SaveChanges() metodu, Entity Framework (EF) tarafından veritabanına yapılan değişiklikleri kalıcı hale getirmek için kullanılır.

//🔍 Detaylı açıklama:

//EF Core’da, sen Add(), Update(), Remove() gibi işlemler yaptığında, bu değişiklikler önce bellekte (context içinde) tutulur.
//Yani EF, senin yaptığın değişiklikleri hemen veritabanına göndermez; sadece hangi nesnelerin eklendiğini, güncellendiğini veya silindiğini izler (change tracking).

//İşte tam bu noktada:
//👉 SaveChanges() çağrıldığında EF, bellekteki bu değişiklikleri SQL komutlarına dönüştürür (örneğin INSERT, UPDATE, DELETE)
//ve bu komutları veritabanında uygular.

//🔧 Kısaca özetlersek:
//Aşama Ne olur?
//Add()	Yeni nesne bellekte "Added" durumuna alınır
//Update()	Nesne "Modified" durumuna alınır
//Remove()	Nesne "Deleted" durumuna alınır
//SaveChanges()	EF tüm bu değişiklikleri SQL komutlarına çevirip veritabanına gönderir
//🧠 Neden kullanılır?

//Çünkü EF, veritabanı ile doğrudan çalışmaz, bir ara katman (context) üzerinden değişiklikleri izler.

//SaveChanges() çağrılmadığı sürece, yapılan işlemler sadece uygulama belleğinde kalır; veritabanına yansımaz.

//📘 Örnek:
//using (var context = new AppDbContext())
//{
//    var student = new Student { Name = "Ayşe Demir" };
//    context.Students.Add(student); // EF bellekte "Added" olarak işaretler
//    context.SaveChanges(); // Bu anda veritabanına INSERT sorgusu gönderilir
//}


//🔹 Eğer SaveChanges() yazmazsan, bu kayıt veritabanına eklenmez — sadece programın RAM’inde var olur.


