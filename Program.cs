using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

// Uygulamanın başlangıç noktası
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // API kontrolcülerini ekleme
        builder.Services.AddControllers();

        // Veritabanı bağlamını (DbContext) servislere ekleme
        // Bağlantı dizesini appsettings.json dosyasından okur
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<MobilyaDbContext>(options =>
            options.UseSqlServer(connectionString));

        var app = builder.Build();

        // HTTP istek hattını yapılandırma
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}

// Veritabanı için Entity Framework Core bağlam sınıfı
public class MobilyaDbContext : DbContext
{
    public MobilyaDbContext(DbContextOptions<MobilyaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Siparis> Siparisler { get; set; }
    public DbSet<SiparisKalemi> SiparisKalemleri { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // İlişkileri tanımlama
        modelBuilder.Entity<Siparis>()
            .HasMany(s => s.SiparisKalemleri)
            .WithOne(sk => sk.Siparis)
            .HasForeignKey(sk => sk.SiparisId);
    }
}

// Veritabanı modeli: Sipariş
public class Siparis
{
    public int SiparisId { get; set; }
    public int MusteriId { get; set; }
    public DateTime SiparisTarihi { get; set; }
    public ICollection<SiparisKalemi> SiparisKalemleri { get; set; }
}

// Veritabanı modeli: Sipariş Kalemi
public class SiparisKalemi
{
    public int SiparisKalemId { get; set; }
    public int SiparisId { get; set; }
    public string ModulId { get; set; }
    public string RenkKodu { get; set; }
    public int Adet { get; set; }
    public Siparis Siparis { get; set; }
}

// API'den veri almak için kullanılan DTO (Veri Transfer Nesnesi)
public class SiparisDto
{
    public int MusteriId { get; set; }
    public List<TasarimBilgisiDto> TasarimBilgileri { get; set; }
}

// Tasarım bilgilerini içeren DTO
public class TasarimBilgisiDto
{
    public string ModulId { get; set; }
    public string RenkKodu { get; set; }
    public int Adet { get; set; }
}

// API'yi yönetecek kontrolcü sınıfı
[ApiController]
[Route("api/[controller]")]
public class SiparisController : ControllerBase
{
    private readonly MobilyaDbContext _context;

    public SiparisController(MobilyaDbContext context)
    {
        _context = context;
    }

    // Yeni bir sipariş oluşturmak için POST metodu
    [HttpPost("olustur")]
    public async Task<IActionResult> SiparisOlustur([FromBody] SiparisDto siparisDto)
    {
        if (siparisDto == null)
        {
            return BadRequest("Geçersiz sipariş verisi.");
        }

        try
        {
            var yeniSiparis = new Siparis
            {
                MusteriId = siparisDto.MusteriId,
                SiparisTarihi = DateTime.Now,
                SiparisKalemleri = siparisDto.TasarimBilgileri.Select(x => new SiparisKalemi
                {
                    ModulId = x.ModulId,
                    RenkKodu = x.RenkKodu,
                    Adet = x.Adet
                }).ToList()
            };

            _context.Siparisler.Add(yeniSiparis);
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Sipariş başarıyla oluşturuldu.", siparisId = yeniSiparis.SiparisId });
        }
        catch (Exception ex)
        {
            // Hata yakalama ve loglama
            return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
        }
    }

    // Test amaçlı GET metodu
    [HttpGet]
    public string Get()
    {
        return "Siparis API çalışıyor!";
    }
}

