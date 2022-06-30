import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TypeDefinitionViewComponent } from "./components/type-definition-view/type-definition-view.component";
import { TypeDefinitionSelectComponent } from "./components/type-definition-select/type-definition-select.component";
import { DataGeneratorComponent } from "./components/data-generator/data-generator.component";
import { LayoutComponent } from "./components/layout/layout.component";


const routes: Routes = [
  {
    title: 'Index',
    path: '',
    component: LayoutComponent,
    children: [
      {
        title: 'Generate Data',
        path: 'generator',
        component: DataGeneratorComponent,
      },
      {
        title: 'Query Engine',
        path: 'query',
        component: TypeDefinitionSelectComponent,
        children: [
          {
            title: 'Query By Type',
            path: ':type',
            component: TypeDefinitionViewComponent,
          },
        ]
      }
    ]
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
