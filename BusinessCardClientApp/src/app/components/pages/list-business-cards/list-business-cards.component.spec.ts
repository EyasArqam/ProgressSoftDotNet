import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListBusinessCardsComponent } from './list-business-cards.component';

describe('ListBusinessCardsComponent', () => {
  let component: ListBusinessCardsComponent;
  let fixture: ComponentFixture<ListBusinessCardsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ListBusinessCardsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ListBusinessCardsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
