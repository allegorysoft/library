using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace MyCompanyName.MyProjectName.Public.Samples;

[Area(MyProjectNamePublicRemoteServiceConsts.ModuleName)]
[RemoteService(Name = MyProjectNamePublicRemoteServiceConsts.RemoteServiceName)]
[Route("api/MyProjectName/public/sample")] //TODO can be bug ?? 
public class SampleController : MyProjectNamePublicController, ISampleAppService
{
    private readonly ISampleAppService _sampleAppService;

    public SampleController(ISampleAppService sampleAppService)
    {
        _sampleAppService = sampleAppService;
    }

    [HttpGet]
    public async Task<SampleDto> GetAsync()
    {
        return await _sampleAppService.GetAsync();
    }

    [HttpGet]
    [Route("authorized")]
    [Authorize]
    public async Task<SampleDto> GetAuthorizedAsync()
    {
        return await _sampleAppService.GetAsync();
    }
}
