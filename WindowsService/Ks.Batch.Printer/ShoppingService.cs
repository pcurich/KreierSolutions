using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace Ks.Batch.Printer
{
    public class ShoppingService: JsonSerializer
    {
        public class ShoppingCart
        {
            [JsonProperty("_id")]
            public string Id { get; set; }

            [JsonProperty("ticketNumber")]
            public int TicketNumber { get; set; }
            
            [JsonProperty("cash")]
            public double Cash { get; set; }
            
            [JsonProperty("credit")]
            public double Credit { get; set; }

            [JsonProperty("change")]
            public double Change { get; set; }

            [JsonProperty("paymentType")] 
            public string PaymentType { get; set;}
            
            [JsonProperty("total")]
            public double Total { get; set; } 
            public Customer Customer { get; set; }
            public User User { get; set; }
            public List<Details> Details { get; set; }
            
        }

        public class Details: JsonSerializer
        {
            public double Quantity { get; set; }
            public Product Product { get; set; }
            public double Price { get; set; }
            public double SubTotal { get { return Price * Quantity; } }

        }

        public class Customer: JsonSerializer
        {
            [JsonProperty("name")]
            public string Name { get; set; }
            
            [JsonProperty("last_name")] 
            public string LastName { get; set; }
        }

        public class User :JsonSerializer
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class Product : JsonSerializer
        {
            [JsonProperty("long_name")]
            public string Name { get; set; }
        }

        public static ShoppingCart LoadJson( string fullPath)
        {
            ShoppingCart jsonData = null;
            try
            {
                using (StreamReader r = new StreamReader(fullPath))
                {
                    var json = r.ReadToEnd();
                    json = json.Replace("\\", "");
                    jsonData = JsonConvert.DeserializeObject<ShoppingCart>(json);
                }
            }catch(Exception e)
            {
                var error = e.Message;
            }
            return jsonData;
        }
    }
}
