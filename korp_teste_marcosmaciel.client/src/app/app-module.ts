import { HttpClientModule } from '@angular/common/http';
import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { NotaFiscalPageComponent } from './features/notas-fiscais/nota-fiscal-page.component';
import { ProductPageComponent } from './features/products/product-page.component';
import { ToastContainerComponent } from './toast-container.component';

@NgModule({
  declarations: [
    App,
    ProductPageComponent,
    NotaFiscalPageComponent,
    ToastContainerComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
  ],
  bootstrap: [App]
})
export class AppModule { }
