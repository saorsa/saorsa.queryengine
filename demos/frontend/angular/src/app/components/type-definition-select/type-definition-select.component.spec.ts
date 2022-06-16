import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TypeDefinitionSelectComponent } from './type-definition-select.component';

describe('TypeDefinitionSelectComponent', () => {
  let component: TypeDefinitionSelectComponent;
  let fixture: ComponentFixture<TypeDefinitionSelectComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TypeDefinitionSelectComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TypeDefinitionSelectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
