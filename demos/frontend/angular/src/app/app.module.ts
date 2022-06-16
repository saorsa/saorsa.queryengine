import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ApiService } from "./services/api.service";
import { HttpClientModule} from "@angular/common/http";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HealthService} from "./services/health.service";
import { ErrorViewComponent } from './components/common/error-view/error-view.component';
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MetaService } from "./services/meta.service";
import { TypeDefinitionSelectComponent } from './components/type-definition-select/type-definition-select.component';
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatFormFieldModule } from "@angular/material/form-field";
import { ReactiveFormsModule } from "@angular/forms";
import { MatInputModule } from "@angular/material/input";
import { MatSelectModule } from "@angular/material/select";
import { DummyComponent } from './components/common/dummy/dummy.component';
import { TypeDefinitionViewComponent } from './components/type-definition-view/type-definition-view.component';
import { PropertyFilterBlockViewComponent } from './components/property-filter-block-view/property-filter-block-view.component';
import { FilterArgArrayComponent } from './components/filter-arg-array/filter-arg-array.component';
import { PropertyFilterViewComponent } from './components/property-filter-view/property-filter-view.component';

@NgModule({
  declarations: [
    AppComponent,
    ErrorViewComponent,
    TypeDefinitionSelectComponent,
    DummyComponent,
    TypeDefinitionViewComponent,
    PropertyFilterBlockViewComponent,
    FilterArgArrayComponent,
    PropertyFilterViewComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NgbModule,
    HttpClientModule,
    BrowserAnimationsModule,
    MatAutocompleteModule,
    MatProgressSpinnerModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatInputModule,
    MatSelectModule,
  ],
  providers: [
    ApiService,
    HealthService,
    MetaService,
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
