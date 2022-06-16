import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TypeDefinitionViewComponent } from './type-definition-view.component';

describe('TypeDefinitionViewComponent', () => {
  let component: TypeDefinitionViewComponent;
  let fixture: ComponentFixture<TypeDefinitionViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TypeDefinitionViewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TypeDefinitionViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
