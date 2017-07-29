using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppointyAPI.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdersController : Controller
    {
        public static Dictionary<int, Orders> orderList = new Dictionary<int, Orders>();
        public static int orderCount = 0;
        // GET api/orders
        [HttpGet]
        public Orders[] Get()
        {
            return orderList.Values.ToArray();
        }

        // GET api/orders/5
        [HttpGet("{id}")]
        public Orders Get(int id)
        {
            return orderList[id];
        }

        // GET api/orders/return
        [HttpGet]
        [Route("return")]
        public Orders[] GetReturnRequested()
        {
            List<Orders> orders = new List<Orders>();
            var oList = orderList.Values;
            foreach (var o in oList)
            {
                if(o.status == DeliveryStatus.returnRequested) {
                    orders.Add(o);
                }
            }
            return orders.ToArray();
        }

        // POST api/orders
        [HttpPost]
        public Orders Post([FromBody]Orders order)
        {
            order.id = orderCount++;
            orderList.Add(order.id, order);
            return order;
        }

        // PATCH api/orders/5
        [HttpPatch("{id}")]
        public Orders Patch(int id, [FromBody]Orders order)
        {
            orderList[id] = order;
            return order;
        }

        // PATCH api/orders/return/5
        [HttpPatch("return/{id}")]
        public Orders PatchDeliveryStatus(int id, [FromBody]Orders order)
        {
            order.status = DeliveryStatus.returned;
            orderList[id] = order;
            return order;
        }

        // DELETE api/orders/5
        [HttpDelete("{id}")]
        public Orders Cancel(int id)
        {
            Orders order = orderList[id];
            order.status = DeliveryStatus.cancelled;
            return order;
        }

        // DELETE api/orders/return/5
        [HttpDelete("return/{id}")]
        public async Task<Orders> Return(int id)
        {
            Orders order = orderList[id];
            var destination = CustomersController.customerList[orderList[id].custId].address;
            var aList = AgentsController.agentList.Values;
            var min = 999999;
            var nearAgent = -1;
            foreach (var a in aList)
            {                
                if(a.available) {
                    var origin = a.address;
                    var requestUri = "https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial&origins=" + origin + "&destinations=" + destination +  "&key=AIzaSyBoLtMUKhkLxkBj9AraKaszNx4_zqCXJ9M";
                    var client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync(requestUri);
                    response.EnsureSuccessStatusCode();
                    var resultString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<JObject>(resultString);
                    var distance = (int)result["rows"][0]["elements"][0]["distance"]["value"];
                    if(distance < min && a.capacity >= order.volume) {
                        min = distance;
                        nearAgent = a.id;
                    }
                }
            }
            Pickups p = new Pickups();
            p.id = PickupsController.pickupCount++;
            p.custId = orderList[id].custId;
            p.orderId = id;
            p.agentId = nearAgent;
            PickupsController.pickupList.Add(p.id, p);
            AgentsController.agentList[nearAgent].available = false;
            order.status = DeliveryStatus.returnRequested;
            return order;
        }
    }
    public class Orders {
        public int id = -1;
        public int custId;
        public int prodId;
        public double volume;
        public double amount;
        public DeliveryStatus status; 
    }
    public enum DeliveryStatus
    {
        delivered, undelivered, returned, returnRequested, cancelled
    };
}
