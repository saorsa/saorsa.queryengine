import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PropertyFilterBlockViewComponent } from './property-filter-block-view.component';

describe('PropertyFilterBlockViewComponent', () => {
  let component: PropertyFilterBlockViewComponent;
  let fixture: ComponentFixture<PropertyFilterBlockViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PropertyFilterBlockViewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PropertyFilterBlockViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
