using System.Web.Http;

using Microsoft.AspNet.SignalR;

namespace Web.Controllers
{
  [RoutePrefix("console")]
  public class ConsoleController : ApiController
  {
    [Route("{sessionId}")]
    public void Post(string sessionId, [FromBody] dynamic block)
    {
      GlobalHost.ConnectionManager.GetHubContext<ConsoleHub>().Clients.All.block(sessionId, block);
    }

    [Route("{sessionId}")]
    public void Delete(string sessionId)
    {
      GlobalHost.ConnectionManager.GetHubContext<ConsoleHub>().Clients.All.terminate(sessionId);
    }
  }
}
