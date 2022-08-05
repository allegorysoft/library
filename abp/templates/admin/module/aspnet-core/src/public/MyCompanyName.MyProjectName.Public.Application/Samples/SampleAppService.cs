﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MyCompanyName.MyProjectName.Public.Samples;

public class SampleAppService : MyProjectNamePublicAppService, ISampleAppService
{
    public Task<SampleDto> GetAsync()
    {
        return Task.FromResult(
            new SampleDto
            {
                Value = 42
            }
        );
    }

    [Authorize]
    public Task<SampleDto> GetAuthorizedAsync()
    {
        return Task.FromResult(
            new SampleDto
            {
                Value = 42
            }
        );
    }
}
