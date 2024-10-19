import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BusinessCardsRoutingModule } from './business-cards-routing.module';
import { AddBusinessCardComponent } from './add-business-card/add-business-card.component';
import { ListBusinessCardsComponent } from './list-business-cards/list-business-cards.component';
import { SharedModule } from '../../shared/shared-module/shared.module';
import { DividerWithTextComponent } from "../../shared/shared-components/divider-with-text/divider-with-text.component";
import { FileUploadModule } from '@iplab/ngx-file-upload';
import { ImportFileComponent } from "../../shared/shared-components/import-file/import-file.component";
import {MatGridListModule} from '@angular/material/grid-list';



@NgModule({
  declarations: [
    AddBusinessCardComponent,
    ListBusinessCardsComponent
  ],
  imports: [
    CommonModule,
    BusinessCardsRoutingModule,
    SharedModule,
    DividerWithTextComponent,
    FileUploadModule,
    ImportFileComponent,
    MatGridListModule
],
exports:[
  FileUploadModule
]
})
export class BusinessCardsModule { }
