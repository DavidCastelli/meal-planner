import { InjectionToken } from '@angular/core';

export interface ApiConfig {
  baseUrl: string;
  prefix: string;
  version: string;
}

export const DEFAULT_API_CONFIG: ApiConfig = {
  baseUrl: 'https://localhost:7256',
  prefix: 'api',
  version: '1',
};

export const API_CONFIG = new InjectionToken<ApiConfig>('API_CONFIG');
