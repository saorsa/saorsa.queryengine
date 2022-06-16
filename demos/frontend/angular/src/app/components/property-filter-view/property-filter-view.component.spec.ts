import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PropertyFilterViewComponent } from './property-filter-view.component';

describe('PropertyFilterComponent', () => {
  let component: PropertyFilterViewComponent;
  let fixture: ComponentFixture<PropertyFilterViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PropertyFilterViewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PropertyFilterViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
