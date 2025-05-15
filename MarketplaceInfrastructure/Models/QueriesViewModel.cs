// File: ViewModels/QueriesViewModel.cs
using System;
using System.Collections.Generic;

namespace YourNamespace.ViewModels
{
    public class OrderQuantityDto
    {
        public int OrderId { get; set; }
        public int TotalQuantity { get; set; }
    }

    public class ClientPairDto
    {
        public string Client1 { get; set; }
        public string Client2 { get; set; }
    }

    public class QueriesViewModel
    {
        // Query 1
        public decimal? PriceThreshold { get; set; }
        public List<string> Query1Result { get; set; }

        // Query 2
        public DateTime? OrderDateAfter { get; set; }
        public List<string> Query2Result { get; set; }

        // Query 3
        public string CategoryNameProducts { get; set; }
        public List<string> Query3Result { get; set; }

        // Query 4
        public int? QuantityThreshold { get; set; }
        public List<OrderQuantityDto> Query4Result { get; set; }

        // Query 5
        public int? StockThreshold { get; set; }
        public List<string> Query5Result { get; set; }

        // Query 6
        public string CategoryNameClients { get; set; }
        public List<string> Query6Result { get; set; }

        // Query 7
        public int? ShopId { get; set; }
        public List<string> Query7Result { get; set; }

        // Query 8
        public List<ClientPairDto> Query8Result { get; set; }
    }
}
