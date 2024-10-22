import { Component, HostListener, inject, OnInit } from '@angular/core';
import { BusinessCard } from '../../../data/models/BusinessCard';
import { BackendService } from '../../../shared/services/backend.service';

@Component({
  selector: 'app-list-business-cards',
  templateUrl: './list-business-cards.component.html',
  styleUrl: './list-business-cards.component.css'
})
export class ListBusinessCardsComponent implements OnInit {

  @HostListener('window:resize', ['$event'])
  logoBase64: string = "";
  cols: number = 2;
  businessCards: BusinessCard[] = [];
  _backend = inject(BackendService);
  IsAction = false;


  ngOnInit(): void {
    this.updateCols(window.innerWidth);

    this.loadBusinessCards();

  }

  loadBusinessCards() {
    this._backend.get("BusinesCards/GetAllBusinessCards").then((res) => {
      if (res.ok) {
        this.businessCards = res.body;
      }
    });
  }

  onResize(event: Event) {
    const target = event.target as Window;
    this.updateCols(target.innerWidth);
  }

  updateCols(width: number) {
    if (width < 650) {
      this.cols = 1;
    } else {
      this.cols = 2;
    }
  }

  getCols() {
    return this.cols;
  }


  activeActions() {
    this.IsAction = !this.IsAction;
  }

  deleteCard(Id: number) {
    if (!Id) {
      return;
    }

    this._backend.delete("BusinesCards/DeleteBusinessCard/" + Id).then((res) => {
      if (res?.ok) {
        this.loadBusinessCards();
      }
    });

  }
  
  exportXml(Id: number){
    this._backend.ExportXml("BusinesCards/ExportXml", Id).then((res) => {
      if (res.ok) {
        
      }
    });

  }

  exportCsv(Id: number){
    this._backend.ExportCsv("BusinesCards/ExportCsv", Id).then((res) => {
      if (res.ok) {
        
      }
    });

  }

}
