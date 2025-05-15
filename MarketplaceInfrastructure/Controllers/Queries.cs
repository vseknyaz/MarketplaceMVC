using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarketplaceInfrastructure; // Припускаємо, що це ваш контекст БД
using System.Linq;
using System.Threading.Tasks;

namespace MarketplaceInfrastructure.Controllers
{
    public class QueriesController : Controller
    {
        private readonly MarketplaceContext _context;

        public QueriesController(MarketplaceContext context)
        {
            _context = context;
        }

        // GET: Відображення сторінки з формами
        public IActionResult Index()
        {
            return View();
        }

        // POST: Запит 1 - Міста магазинів із товарами дорожче X
        [HttpPost]
        public IActionResult Query1(decimal price)
        {
            var result = _context.Shops
                .Join(_context.Products, s => s.ShopId, p => p.ShopId, (s, p) => new { s, p })
                .Where(x => x.p.Price > price)
                .Select(x => x.s.Address)
                .Distinct()
                .ToList();
            ViewBag.Query1Result = result;
            ViewBag.Query1Price = price;
            return View("Index");
        }

        // POST: Запит 2 - Клієнти, які зробили замовлення після дати X
        [HttpPost]
        public IActionResult Query2(DateTime date)
        {
            var result = _context.Clients
                .Join(_context.Orders, c => c.ClientId, o => o.ClientId, (c, o) => new { c, o })
                .Where(x => x.o.OrderDate > date)
                .Select(x => x.c.FullName)
                .Distinct()
                .ToList();
            ViewBag.Query2Result = result;
            ViewBag.Query2Date = date;
            return View("Index");
        }

        // POST: Запит 3 - Товари з категорії X
        [HttpPost]
        public IActionResult Query3(string categoryName)
        {
            var result = _context.Products
                .Join(_context.Categories,
                      p => p.CategoryId,
                      c => c.CategoryName, // Використовуємо CategoryName, а не CategoryId
                      (p, c) => new { p, c })
                .Where(x => x.c.CategoryName == categoryName)
                .Select(x => x.p.ProductName)
                .ToList();
            ViewBag.Query3Result = result;
            ViewBag.Query3CategoryName = categoryName;
            return View("Index");
        }

        // POST: Запит 4 - Замовлення з кількістю товарів більше X
        [HttpPost]
        public IActionResult Query4(int quantity)
        {
            var result = _context.Orders
                .Join(_context.OrderProducts, o => o.OrderId, op => op.OrderId, (o, op) => new { o, op })
                .GroupBy(x => x.o.OrderId)
                .Select(g => new { OrderId = g.Key, TotalQuantity = g.Sum(x => x.op.Quantity) })
                .Where(x => x.TotalQuantity > quantity)
                .ToList();
            ViewBag.Query4Result = result;
            ViewBag.Query4Quantity = quantity;
            return View("Index");
        }

        // POST: Запит 5 - Магазини з товарами на складі менше X
        [HttpPost]
        public IActionResult Query5(int stock)
        {
            var result = _context.Shops
                .Join(_context.Products, s => s.ShopId, p => p.ShopId, (s, p) => new { s, p })
                .Where(x => x.p.Stock < stock)
                .Select(x => x.s.ShopName)
                .Distinct()
                .ToList();
            ViewBag.Query5Result = result;
            ViewBag.Query5Stock = stock;
            return View("Index");
        }

        // POST: Запит 6 - Клієнти, які замовили всі товари з категорії X
        [HttpPost]
        public IActionResult Query6(string categoryName)
        {
            var result = _context.Clients
                .Where(c => !_context.Products
                    .Where(p => p.CategoryId == categoryName)
                    .Any(p => !_context.Orders
                        .Join(_context.OrderProducts, o => o.OrderId, op => op.OrderId, (o, op) => new { o, op })
                        .Any(x => x.o.ClientId == c.ClientId && x.op.ProductId == p.ProductId)))
                .Select(c => c.FullName)
                .ToList();
            ViewBag.Query6Result = result;
            ViewBag.Query6CategoryName = categoryName;
            return View("Index");
        }

        // POST: Запит 7 - Магазини, що продають ті самі товари, що й магазин X
        [HttpPost]
        public IActionResult Query7(int shopId)
        {
            var result = _context.Shops
                .Where(s2 => s2.ShopId != shopId
                    && !_context.Products
                        .Where(p => p.ShopId == shopId)
                        .Any(p => !_context.Products
                            .Where(p2 => p2.ShopId == s2.ShopId)
                            .Select(p2 => p2.ProductId)
                            .Contains(p.ProductId))
                    && !_context.Products
                        .Where(p2 => p2.ShopId == s2.ShopId)
                        .Any(p2 => !_context.Products
                            .Where(p => p.ShopId == shopId)
                            .Select(p => p.ProductId)
                            .Contains(p2.ProductId)))
                .Select(s2 => s2.ShopName)
                .ToList();
            ViewBag.Query7Result = result;
            ViewBag.Query7ShopId = shopId;
            return View("Index");
        }

        // POST: Запит 8 - Пари клієнтів, які замовили однакові товари
        [HttpPost]
        public IActionResult Query8()
        {
            var result = (from c1 in _context.Clients
                          from c2 in _context.Clients
                          where c1.ClientId < c2.ClientId
                          && _context.Orders
                              .Join(_context.OrderProducts, o => o.OrderId, op => op.OrderId, (o, op) => new { o, op })
                              .Where(x => x.o.ClientId == c1.ClientId)
                              .Select(x => x.op.ProductId)
                              .Intersect(_context.Orders
                                  .Join(_context.OrderProducts, o => o.OrderId, op => op.OrderId, (o, op) => new { o, op })
                                  .Where(x => x.o.ClientId == c2.ClientId)
                                  .Select(x => x.op.ProductId))
                              .Any()
                          select new { Client1 = c1.FullName, Client2 = c2.FullName })
             .ToList();
            ViewBag.Query8Result = result;
            return View("Index");
        }
    }
}