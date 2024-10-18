import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BusinessCardsRoutingModule } from './business-cards-routing.module';
import { AddBusinessCardComponent } from './add-business-card/add-business-card.component';
import { ListBusinessCardsComponent } from './list-business-cards/list-business-cards.component';



@NgModule({
  declarations: [
    AddBusinessCardComponent,
    ListBusinessCardsComponent
  ],
  imports: [
    CommonModule,
    BusinessCardsRoutingModule
  ]
})
export class BusinessCardsModule { }
