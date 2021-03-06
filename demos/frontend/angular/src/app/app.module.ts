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
import { PropertyFilterBlockControlComponent } from './components/property-filter-block-view/property-filter-block-control.component';
import { PropertyArgumentArrayControlComponent } from './components/property-argument-array-control/property-argument-array-control.component';
import { PropertyFilterControlComponent } from './components/property-filter-control/property-filter-control.component';
import { MatCardModule } from "@angular/material/card";
import { QueryEngineTypeSystemService } from "./services/query-engine-type-system.service";
import { FlexModule } from "@angular/flex-layout";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { MatTooltipModule } from "@angular/material/tooltip";
import { InspectButtonComponent } from './components/common/inspect-button/inspect-button.component';
import { MatDialogModule } from "@angular/material/dialog";
import { InspectViewComponent } from './components/common/inspect-view/inspect-view.component';
import { MatToolbarModule } from "@angular/material/toolbar";
import { FormsHelperService } from "./services/forms-helper.service";
import { QueryEngineUsersService } from "./services/query-engine-users.service";
import { DataGeneratorComponent } from './components/data-generator/data-generator.component';
import { LayoutComponent } from './components/layout/layout.component';
import { MatMenuModule } from "@angular/material/menu";
import { DataGeneratorService } from "./services/data-generator.service";
import {QueryEngineCategoriesService} from "./services/query-engine-categories.service";


@NgModule({
  declarations: [
    AppComponent,
    ErrorViewComponent,
    TypeDefinitionSelectComponent,
    DummyComponent,
    TypeDefinitionViewComponent,
    PropertyFilterBlockControlComponent,
    PropertyArgumentArrayControlComponent,
    PropertyFilterControlComponent,
    InspectButtonComponent,
    InspectViewComponent,
    DataGeneratorComponent,
    LayoutComponent,
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
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatDialogModule,
    FlexModule,
    MatToolbarModule,
    MatMenuModule,
  ],
  providers: [
    ApiService,
    HealthService,
    MetaService,
    QueryEngineTypeSystemService,
    QueryEngineUsersService,
    FormsHelperService,
    DataGeneratorService,
    QueryEngineCategoriesService,
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
