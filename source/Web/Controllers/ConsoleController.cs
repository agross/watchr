using System.Web.Http;

using Microsoft.AspNet.SignalR;

namespace Web.Controllers
{
  public class ConsoleController : ApiController
  {
    public void Post([FromBody]string line)
    {
      GlobalHost.ConnectionManager.GetHubContext<ConsoleHub>().Clients.All.broadcast(line);
    }
  }
}