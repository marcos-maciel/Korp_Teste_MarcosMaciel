import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotaFiscalPageComponent } from './features/notas-fiscais/nota-fiscal-page.component';
import { ProductPageComponent } from './features/products/product-page.component';

const routes: Routes = [
  { path: '', redirectTo: 'products', pathMatch: 'full' },
  { path: 'products', component: ProductPageComponent },
  { path: 'notas-fiscais', component: NotaFiscalPageComponent },
  { path: '**', redirectTo: 'products' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
