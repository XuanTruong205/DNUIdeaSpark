using DNU.IdeaSpark.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;

    public HomeController(ApplicationDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        var ideas = _db.Ideas
            .Include(i => i.Category) // Eager loading danh mục
            .OrderByDescending(i => i.CreatedAt)
            .Take(10)
            .ToList();

        return View(ideas);
    }
}