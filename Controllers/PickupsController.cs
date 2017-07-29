using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AppointyAPI.Controllers
{
    [Route("api/[controller]")]
    public class PickupsController : Controller
    {
        public static Dictionary<int, Pickups> pickupList = new Dictionary<int, Pickups>();
        public static int pickupCount = 0;
        // GET api/pickups
        [HttpGet]
        public Pickups[] Get()
        {
            return pickupList.Values.ToArray();
        }

        // GET api/pickups/5
        [HttpGet("{id}")]
        public Pickups Get(int id)
        {
            return pickupList[id];
        }

        // POST api/pickups
        [HttpPost]
        public Pickups Post([FromBody]Pickups p)
        {
            p.id = pickupCount++;
            pickupList.Add(p.id, p);
            return p;
        }

        // PATCH api/pickups/5
        [HttpPatch("{id}")]
        public Pickups Patch(int id, [FromBody]Pickups p)
        {
            pickupList[id] = p;
            return p;
        }
        
        // PATCH api/pickups/status/5?pickup=postponed
        [HttpPatch("status/{id}")]
        public Pickups PatchAgent(int id, [FromBody]Pickups p)
        {
            var pickup = this.Request.Query["pickup"];
            if(pickup.Equals("scheduled")) {
                p.status = PickupStatus.scheduled;
            }
            else if(pickup.Equals("success")) {
                p.status = PickupStatus.success;
                AgentsController.agentList[p.agentId].capacity -= OrdersController.orderList[p.orderId].volume;
                AgentsController.agentList[p.agentId].address = CustomersController.customerList[p.custId].address;
                AgentsController.agentList[p.agentId].available = true;
            }
            else if(pickup.Equals("postponed")) {
                p.status = PickupStatus.postponed;
                AgentsController.agentList[p.agentId].address = CustomersController.customerList[p.custId].address;
                AgentsController.agentList[p.agentId].available = true;
            }
            pickupList[id] = p;
            return p;
        }

        // DELETE api/pickups/5
        [HttpDelete("{id}")]
        public Pickups Delete(int id)
        {
            Pickups p = pickupList[id];
            pickupList.Remove(id);
            return p;
        }
    }
    public class Pickups {
        public int id;
        public int custId;
        public int orderId;
        public int agentId;
        public PickupStatus status;

        public Pickups() {
            id = -1;
            custId = -1;
            orderId = -1;
            agentId = -1;
            status = 0;
        }
    }
    public enum PickupStatus
    {
        scheduled, success, postponed
    }
}
