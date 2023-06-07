import { Environment } from '@abp/ng.core';

const baseUrl = 'http://saler.allegorysoft.com';

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'Saler',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'http://saler.api.allegorysoft.com',
    redirectUri: baseUrl,
    clientId: 'Saler_App',
    responseType: 'code',
    scope: 'offline_access Saler',
    requireHttps: false
  },
  apis: {
    default: {
      url: 'http://saler.api.allegorysoft.com',
      rootNamespace: 'Allegory.Saler',
    },
  },
} as Environment;
