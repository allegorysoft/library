using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace MyCompanyName.MyProjectName.Admin.Samples;

[Area(MyProjectNameAdminRemoteServiceConsts.ModuleName)]
[RemoteService(Name = MyProjectNameAdminRemoteServiceConsts.RemoteServiceName)]
[Route("api/MyProjectName/admin/sample")]
public class SampleController : MyProjectNameAdminController, ISampleAppService
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
