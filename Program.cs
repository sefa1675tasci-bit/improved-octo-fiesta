using ConsoleApp31.Data;
using ConsoleApp31.Models;
using Microsoft.EntityFrameworkCore;

class Program
{
    static void Main()
    {
        using var context = new BlogContext();

        // Yazar ekle 
        var author = new Author
        {
            Name = "Ahmet Yılmaz",
            Email = "ahmet@email.com"
        };
        context.Authors.Add(author);

        // Makale ekle 
        var article = new Article
        {
            Title = "EF Core Kullanımı",
            Content = "Entity Framework Core ile veritabanı işlemleri...",
            Author = author
        };
        context.Articles.Add(article);

        // Yorum ekle 
        var comment = new Comment
        {
            Text = "Harika bir yazı!",
            CommenterName = "Ziyaretçi",
            Article = article
        };
        context.Comments.Add(comment);

        context.SaveChanges();
        Console.WriteLine("Blog verileri eklendi!");

        // Verileri oku 
        var articles = context.Article
            .Include(a => a.Author)
            .Include(a => a.Comments)
            .ToList();

        foreach (var art in articles)
        {
            Console.WriteLine($"Başlık: {art.Title}");
            Console.WriteLine($"Yazar: {art.Author.Name}");
            Console.WriteLine($"Yorum Sayısı: {art.Comments.Count}");
            Console.WriteLine("---");
        }
    }
}
