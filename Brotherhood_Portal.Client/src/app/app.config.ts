import { ApplicationConfig, inject, provideAppInitializer, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors,  } from '@angular/common/http';
import { InitService } from '../core/services/init-service';
import { lastValueFrom } from 'rxjs';
import { AuthInterceptor } from '../core/interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }), 
    // provideExperimentalZonelessChangeDetection(),
    provideRouter(routes),
    
    provideHttpClient(
      withInterceptors([AuthInterceptor])
    ),
    
    provideAppInitializer(async () => {
      const initService = inject(InitService);
      try{
        return lastValueFrom(initService.init());
      }
      finally {
        const splash = document.getElementById('initial-splash')
        if(splash){
          splash.remove();
        }
      }
    }),
  ]
};
