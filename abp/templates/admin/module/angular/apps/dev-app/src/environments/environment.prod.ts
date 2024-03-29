import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

export const environment = {
  production: true,
  application: {
    baseUrl: 'http://localhost:4200/',
    name: 'MyProjectName',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://localhost:44301',
    redirectUri: baseUrl,
    clientId: 'MyProjectName_App',
    responseType: 'code',
    scope: 'offline_access MyProjectName',
    requireHttps: true
  },
  apis: {
    default: {
      url: 'https://localhost:44301',
      rootNamespace: 'MyCompanyName.MyProjectName',
    },
    MyProjectNameCommon: {
      url: 'https://localhost:44300',
      rootNamespace: 'MyCompanyName.MyProjectName.Common',
    },
    MyProjectNameAdmin: {
      url: 'https://localhost:44300',
      rootNamespace: 'MyCompanyName.MyProjectName.Admin',
    },
    MyProjectNamePublic: {
      url: 'https://localhost:44300',
      rootNamespace: 'MyCompanyName.MyProjectName.Public',
    },
  },
} as Environment;
