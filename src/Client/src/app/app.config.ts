import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, withRouterConfig } from '@angular/router';

import { routes } from './app.routes';
import {
  provideHttpClient,
  withFetch,
  withInterceptors,
} from '@angular/common/http';
import { API_CONFIG, DEFAULT_API_CONFIG } from './shared/configs/api.config';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { apiInterceptor } from './core/interceptors/api.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(
      routes,
      withRouterConfig({ canceledNavigationResolution: 'computed' }),
    ),
    provideHttpClient(
      withFetch(),
      withInterceptors([apiInterceptor, errorInterceptor]),
    ),
    {
      provide: API_CONFIG,
      useValue: DEFAULT_API_CONFIG,
    },
    provideAnimationsAsync(),
  ],
};
