import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TypeDefinitionViewComponent } from "./components/type-definition-view/type-definition-view.component";
import { TypeDefinitionSelectComponent } from "./components/type-definition-select/type-definition-select.component";


const routes: Routes = [
  {
    title: '',
    path: '',
    component: TypeDefinitionSelectComponent,
    children: [
      {
        title: ':type',
        path: ':type',
        component: TypeDefinitionViewComponent,
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
