using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AppointyAPI.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AgentsController : Controller
    {
        public static Dictionary<int, Agents> agentList = new Dictionary<int, Agents>();
        public static int agentCount = 0;
        // GET api/agents
        [HttpGet]
        public Agents[] Get()
        {
            return agentList.Values.ToArray();
        }

        //GET api/agents/5
        [HttpGet("{id}")]
        public Agents Get(int id)
        {
            return agentList[id];
        }

        //GET api/agents/available
        [HttpGet]
        [Route("available")]
        public Agents[] GetAvailable()
        {
            List<Agents> agents = new List<Agents>();
            var aList = agentList.Values;
            foreach (var a in aList)
            {
                if(a.available) {
                    agents.Add(a);
                }
            }
            return agents.ToArray();
        }

        // POST api/agents
        [HttpPost]
        public Agents Post([FromBody]Agents agent)
        {
            agent.id = agentCount++;
            agentList.Add(agent.id, agent);
            return agent;
        }

        // PATCH api/agents/5
        [HttpPatch("{id}")]
        public Agents Patch(int id, [FromBody]Agents agent)
        {
            agentList[id] = agent;
            return agent;
        }

        // PATCH api/agents/avail/5?avail=true
        [HttpPatch("avail/{id}")]
        public Agents PatchAvail(int id)
        {
            Agents agent = agentList[id];
            bool avail = this.Request.Query["avail"].Equals("true");
            agent.available = avail;
            agentList[id] = agent;
            return agent;
        }

        // DELETE api/agents/5
        [HttpDelete("{id}")]
        public Agents Delete(int id)
        {
            Agents a = agentList[id];
            a.status = AgentStatus.inactive;
            agentList.Remove(id);
            agentList.Add(id, a);
            return a;
        }
    }
    public class Agents {
       public int id;
       public string name;
       public string address;
       public double capacity = 5; 
       public bool available;
       public AgentStatus status;
    }
    public enum  AgentStatus
    {
        active, inactive
    };
}
