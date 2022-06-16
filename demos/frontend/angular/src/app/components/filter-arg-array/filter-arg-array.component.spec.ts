import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FilterArgArrayComponent } from './filter-arg-array.component';

describe('FilterArgArrayComponent', () => {
  let component: FilterArgArrayComponent;
  let fixture: ComponentFixture<FilterArgArrayComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FilterArgArrayComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FilterArgArrayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
