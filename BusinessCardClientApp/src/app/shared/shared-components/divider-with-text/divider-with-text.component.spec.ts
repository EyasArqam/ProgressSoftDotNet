import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DividerWithTextComponent } from './divider-with-text.component';

describe('DividerWithTextComponent', () => {
  let component: DividerWithTextComponent;
  let fixture: ComponentFixture<DividerWithTextComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DividerWithTextComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DividerWithTextComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
