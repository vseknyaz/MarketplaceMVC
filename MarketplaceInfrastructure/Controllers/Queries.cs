using Microsoft.AspNetCore.Mvc;
using MarketplaceInfrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MarketplaceDomain.Model;

namespace MarketplaceInfrastructure.Controllers
{
    public class QueriesController : Controller
    {
        private readonly MarketplaceContext _context;
        private readonly string _connString;

        public QueriesController(MarketplaceContext context)
        {
            _context = context;
            _connString = _context.Database.GetDbConnection().ConnectionString;
        }

        public IActionResult Index()
        {
            return View();
        }

        // 1. Адреси магазинів із товарами дорожче X
        [HttpPost]
        public IActionResult Query1(decimal priceThreshold)
        {
            var list = new List<string>();
            using var conn = new SqlConnection(_connString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT DISTINCT s.Address
                FROM Shops AS s
                JOIN Products AS p ON s.ShopID = p.ShopID
                WHERE p.Price > @price;
            ";
            cmd.Parameters.Add(new SqlParameter("@price", SqlDbType.Money) { Value = priceThreshold });
            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(r.GetString(0));

            ViewBag.PriceThreshold = priceThreshold;
            ViewBag.Query1Result = list;
            return View("Index");
        }

        // 2. Клієнти, які зробили замовлення після дати X
        [HttpPost]
        public IActionResult Query2(DateTime orderDateAfter)
        {
            var list = new List<string>();
            using var conn = new SqlConnection(_connString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT DISTINCT c.FullName
                FROM Clients AS c
                JOIN Orders AS o ON c.ClientID = o.ClientID
                WHERE o.OrderDate > @date;
            ";
            cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime) { Value = orderDateAfter });
            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(r.GetString(0));

            ViewBag.OrderDateAfter = orderDateAfter;
            ViewBag.Query2Result = list;
            return View("Index");
        }

        // 3. Товари з категорії X
        [HttpPost]
        public IActionResult Query3(string categoryNameProducts)
        {
            bool shopExists = _context.Categories.Any(s => s.CategoryName == categoryNameProducts);
            if (!shopExists)
            {
                ViewBag.Query4Message = "Не знайдено такої категорії";
                ViewBag.Query4Result = null;
                return View("Index");
            }
            var list = new List<string>();
            using var conn = new SqlConnection(_connString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT p.ProductName
                FROM Products AS p
                WHERE p.CategoryId = @cat;
            ";
            cmd.Parameters.Add(new SqlParameter("@cat", SqlDbType.NChar, 50) { Value = categoryNameProducts });
            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(r.GetString(0));

            ViewBag.CategoryNameProducts = categoryNameProducts;
            ViewBag.Query3Result = list;
            return View("Index");
        }

        // 4. Замовлення з кількістю товарів більше X
        [HttpPost]
        public IActionResult Query4(int quantityThreshold)
        {
            var list = new List<dynamic>();
            using var conn = new SqlConnection(_connString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT op.Order_ID, SUM(op.Quantity) AS TotalQuantity
                FROM OrderProducts AS op
                GROUP BY op.Order_ID
                HAVING SUM(op.Quantity) > @qty;
            ";
            cmd.Parameters.Add(new SqlParameter("@qty", SqlDbType.Int) { Value = quantityThreshold });
            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new
                {
                    OrderId = r.GetInt32(0),
                    TotalQuantity = r.GetInt32(1)
                });
            }

            ViewBag.QuantityThreshold = quantityThreshold;
            ViewBag.Query4Result = list;
            return View("Index");
        }

        // 5. Магазини з товарами на складі менше X
        [HttpPost]
        public IActionResult Query5(int stockThreshold)
        {
            var list = new List<string>();
            using var conn = new SqlConnection(_connString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT DISTINCT s.ShopName
                FROM Shops AS s
                JOIN Products AS p ON s.ShopID = p.ShopID
                GROUP BY s.ShopName, s.ShopID
                HAVING SUM(p.Stock) < @stk;
            ";
            cmd.Parameters.Add(new SqlParameter("@stk", SqlDbType.Int) { Value = stockThreshold });
            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(r.GetString(0));

            ViewBag.StockThreshold = stockThreshold;
            ViewBag.Query5Result = list;
            return View("Index");
        }

        // 6. Клієнти, які замовили всі товари з категорії X
        [HttpPost]
        public IActionResult Query6(string categoryNameClients)
        {
            var list = new List<string>();
            using var conn = new SqlConnection(_connString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT c.FullName
                FROM Clients AS c
                WHERE NOT EXISTS (
                    SELECT p.ProductID
                    FROM Products AS p
                    WHERE p.CategoryId = @cat
                      AND NOT EXISTS (
                          SELECT 1
                          FROM Orders AS o
                          JOIN OrderProducts AS op ON o.OrderID = op.Order_ID
                          WHERE o.ClientID = c.ClientID
                            AND op.Product_ID = p.ProductID
                      )
                );
            ";
            cmd.Parameters.Add(new SqlParameter("@cat", SqlDbType.NChar, 50) { Value = categoryNameClients });
            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(r.GetString(0));

            ViewBag.CategoryNameClients = categoryNameClients;
            ViewBag.Query6Result = list;
            return View("Index");
        }

        // 7. Магазини, що продають ті самі товари, що й магазин X
        [HttpPost]
        public IActionResult Query7(int shopId)
        {
            bool shopExists = _context.Shops.Any(s => s.ShopId == shopId);
            if (!shopExists)
            {
                ViewBag.Query7Message = "Магазин з таким ID не знайдено.";
                ViewBag.Query7Result = null;
                return View("Index");
            }

            var list = new List<string>();
            using var conn = new SqlConnection(_connString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                WITH BaseProducts AS (
                    SELECT ProductID
                    FROM Products
                    WHERE ShopID = @sid
                )
                SELECT s2.ShopName
                FROM Shops AS s2
                WHERE s2.ShopID <> @sid
                  AND NOT EXISTS (
                        SELECT bp.ProductID FROM BaseProducts AS bp
                        WHERE NOT EXISTS (
                            SELECT 1 FROM Products AS p2
                            WHERE p2.ShopID = s2.ShopID
                              AND p2.ProductID = bp.ProductID
                        )
                  )
                  AND NOT EXISTS (
                        SELECT p2.ProductID FROM Products AS p2
                        WHERE p2.ShopID = s2.ShopID
                          AND NOT EXISTS (
                              SELECT 1 FROM BaseProducts
                              WHERE ProductID = p2.ProductID
                          )
                  );
            ";
            cmd.Parameters.Add(new SqlParameter("@sid", SqlDbType.Int) { Value = shopId });
            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(r.GetString(0));

            ViewBag.ShopId = shopId;
            ViewBag.Query7Result = list;
            return View("Index");
        }

        // 8. Пари клієнтів, які замовили однакові товари
        [HttpPost]
        public IActionResult Query8()
        {
            var list = new List<(string Client1, string Client2)>();
            using var conn = new SqlConnection(_connString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT c1.FullName, c2.FullName
                FROM Clients AS c1
                JOIN Clients AS c2 ON c1.ClientID < c2.ClientID
                WHERE EXISTS (
                    SELECT 1
                    FROM Orders AS o1
                    JOIN OrderProducts AS op1 ON o1.OrderID = op1.Order_ID
                    JOIN Orders AS o2 ON o2.ClientID = c2.ClientID
                    JOIN OrderProducts AS op2 ON o2.OrderID = op2.Order_ID
                    WHERE o1.ClientID = c1.ClientID
                      AND op1.Product_ID = op2.Product_ID
                );
            ";
            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add((r.GetString(0), r.GetString(1)));

            ViewBag.Query8Result = list;
            return View("Index");
        }
    }
}
