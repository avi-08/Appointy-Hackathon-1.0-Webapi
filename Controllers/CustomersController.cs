using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AppointyAPI.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CustomersController : Controller
    {
        public static Dictionary<int, Customers> customerList = new  Dictionary<int, Customers>();
        public static int customerCount = 0;
        // GET api/customers
        [HttpGet]
        public Customers[] Get()
        {
            return customerList.Values.ToArray();
        }

        // GET api/customers/5
        [HttpGet("{id}")]
        public Customers Get(int id)
        {
            return customerList[id];
        }

        // POST api/customers
        [HttpPost]
        public Customers Post([FromBody]Customers c)
        {
            c.id = customerCount++;
            customerList.Add(c.id, c);
            return c;
        }

        // PUT api/customers/5
        [HttpPut("{id}")]
        public Customers Put(int id, [FromBody]Customers c)
        {    
            customerList[id] = c;
            return c;
        }

        // PATCH api/customers/5
        [HttpPatch("{id}")]
        public Customers Patch(int id, [FromBody]Customers c)
        {    
            customerList[id] = c;
            return c;
        }

        // DELETE api/customers/5
        [HttpDelete("{id}")]
        public Customers Delete(int id)
        {
            Customers c = customerList[id];
            c.status = Status.inactive;
            customerList.Remove(id);
            customerList.Add(id, c);
            return c;
        }
    }
    public class Customers {
        public int id = -1;
        public string name;
        public string email;
        public string address;
        public Status status;
    }
    public enum Status
    {
        active, inactive
        
    };
}
