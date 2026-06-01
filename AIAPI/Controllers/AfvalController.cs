using Microsoft.AspNetCore.Mvc;

namespace P4LU1API.Controllers;

[ApiController]
[Route("[controller]")]
public class AfvalController : ControllerBase
{

    [HttpGet(Name = "GetAfval")]
    public IEnumerable<Afval> Get()
    {
        var afval = //await SelectAsync();
        return Ok(afval);
    }

    [HttpGet(Name = "PostAfval")]
    public IEnumerable<Afval> Post()
    {
        return new Afval[0];
    }
}
