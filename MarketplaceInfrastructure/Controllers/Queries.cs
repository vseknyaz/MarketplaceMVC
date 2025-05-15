using Microsoft.AspNetCore.Mvc;
using MarketplaceInfrastructure;
using System;
using System.Linq;

namespace MarketplaceInfrastructure.Controllers
{
    public class QueriesController : Controller
    {
        private readonly MarketplaceContext _context;

        public QueriesController(MarketplaceContext context)
        {
            _context = context;
        }

        // GET: /Queries
        public IActionResult Index()
        {
            return View();
        }

        // 1. Адреси магазинів із товарами дорожче X
        [HttpPost]
        public IActionResult Query1(decimal priceThreshold)
        {
            var addresses = _context.Shops
                .Where(s => s.Products.Any(p => p.Price > priceThreshold))
                .Select(s => s.Address)
                .Distinct()
                .ToList();

            ViewBag.PriceThreshold = priceThreshold;
            ViewBag.Query1Result = addresses;
            return View("Index");
        }

        // 2. Клієнти, які зробили замовлення після дати X
        [HttpPost]
        public IActionResult Query2(DateTime orderDateAfter)
        {
            var clients = _context.Clients
                .Where(c => c.Orders.Any(o => o.OrderDate > orderDateAfter))
                .Select(c => c.FullName)
                .Distinct()
                .ToList();

            ViewBag.OrderDateAfter = orderDateAfter;
            ViewBag.Query2Result = clients;
            return View("Index");
        }

        // 3. Товари з категорії X
        [HttpPost]
        public IActionResult Query3(string categoryNameProducts)
        {
            var products = _context.Products
                .Where(p => p.CategoryId == categoryNameProducts)
                .Select(p => p.ProductName)
                .ToList();

            ViewBag.CategoryNameProducts = categoryNameProducts;
            ViewBag.Query3Result = products;
            return View("Index");
        }

        // 4. Замовлення з кількістю товарів більше X
        [HttpPost]
        public IActionResult Query4(int quantityThreshold)
        {
            var orders = _context.OrderProducts
                .GroupBy(op => op.OrderId)
                .Select(g => new
                {
                    OrderId = g.Key,
                    TotalQuantity = g.Sum(op => op.Quantity)
                })
                .Where(x => x.TotalQuantity > quantityThreshold)
                .ToList();

            ViewBag.QuantityThreshold = quantityThreshold;
            ViewBag.Query4Result = orders;
            return View("Index");
        }

        // 5. Магазини з товарами на складі менше X
        [HttpPost]
        public IActionResult Query5(int stockThreshold)
        {
            var shops = _context.Shops
                .Where(s => s.Products.Sum(p => p.Stock) < stockThreshold)
                .Select(s => s.ShopName)
                .Distinct()
                .ToList();

            ViewBag.StockThreshold = stockThreshold;
            ViewBag.Query5Result = shops;
            return View("Index");
        }

        // 6. Клієнти, які замовили всі товари з категорії X
        [HttpPost]
        public IActionResult Query6(string categoryNameClients)
        {
            var productIds = _context.Products
                .Where(p => p.CategoryId == categoryNameClients)
                .Select(p => p.ProductId)
                .ToList();

            var clients = _context.Clients
                .Where(c => productIds.All(pid =>
                    c.Orders.SelectMany(o => o.OrderProducts)
                     .Any(op => op.ProductId == pid)))
                .Select(c => c.FullName)
                .ToList();

            ViewBag.CategoryNameClients = categoryNameClients;
            ViewBag.Query6Result = clients;
            return View("Index");
        }

        // 7. Магазини, що продають ті самі товари, що й магазин X
        [HttpPost]
        public IActionResult Query7(int shopId)
        {
            var baseIds = _context.Products
                .Where(p => p.ShopId == shopId)
                .Select(p => p.ProductId)
                .ToList();

            var shops = _context.Shops
                .Where(s => s.ShopId != shopId
                            && baseIds.All(id => s.Products.Select(p => p.ProductId).Contains(id))
                            && s.Products.Select(p => p.ProductId).All(id => baseIds.Contains(id)))
                .Select(s => s.ShopName)
                .ToList();

            ViewBag.ShopId = shopId;
            ViewBag.Query7Result = shops;
            return View("Index");
        }

        // 8. Пари клієнтів, які замовили однакові товари
        [HttpPost]
        public IActionResult Query8()
        {
            var pairs = (from c1 in _context.Clients
                         from c2 in _context.Clients
                         where c1.ClientId < c2.ClientId
                            && _context.OrderProducts
                                .Where(op => op.Order.ClientId == c1.ClientId)
                                .Select(op => op.ProductId)
                                .Intersect(
                                    _context.OrderProducts
                                        .Where(op => op.Order.ClientId == c2.ClientId)
                                        .Select(op => op.ProductId)
                                ).Any()
                         select new
                         {
                             Client1 = c1.FullName,
                             Client2 = c2.FullName
                         })
                        .ToList();

            ViewBag.Query8Result = pairs;
            return View("Index");
        }
    }
}
