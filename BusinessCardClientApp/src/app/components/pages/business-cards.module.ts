import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BusinessCardsRoutingModule } from './business-cards-routing.module';
import { AddBusinessCardComponent } from './add-business-card/add-business-card.component';
import { ListBusinessCardsComponent } from './list-business-cards/list-business-cards.component';
import { SharedModule } from '../../shared/shared-module/shared.module';
import { DividerWithTextComponent } from "../../shared/shared-components/divider-with-text/divider-with-text.component";



@NgModule({
  declarations: [
    AddBusinessCardComponent,
    ListBusinessCardsComponent
  ],
  imports: [
    CommonModule,
    BusinessCardsRoutingModule,
    SharedModule,
    DividerWithTextComponent
]
})
export class BusinessCardsModule { }
