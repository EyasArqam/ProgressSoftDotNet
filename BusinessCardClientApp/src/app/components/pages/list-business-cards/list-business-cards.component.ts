import { Component, HostListener, inject, OnInit } from '@angular/core';
import { BusinessCard } from '../../../data/models/BusinessCard';
import { BackendService } from '../../../shared/services/backend.service';
import { FormControl, FormGroup } from '@angular/forms';
import { UrlHelper } from '@utils/url-helper';
import { Gender } from 'app/shared/enums';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-list-business-cards',
  templateUrl: './list-business-cards.component.html',
  styleUrl: './list-business-cards.component.css'
})
export class ListBusinessCardsComponent implements OnInit {
  constructor(public datepipe: DatePipe) { }

  @HostListener('window:resize', ['$event'])
  
  onResize(event: any): void {
    this.defaultWidth = this.calculateWidth(event.target.innerWidth);
  }
  cols: number = 2;
  businessCards: BusinessCard[] = [];
  _backend = inject(BackendService);
  IsAction = false;
  formFilter = new FormGroup({
    Name: new FormControl(''),
    Gender: new FormControl(''),
    Email: new FormControl(''),
    Phone: new FormControl(''),
    DOB: new FormControl(''),
  });
  gender = Object.values(Gender)
  defaultWidth = 600;
  defaultHeight = 340;

  ngOnInit(): void {
    this.loadBusinessCards();

    this.defaultWidth = this.calculateWidth(window.innerWidth);

  }

  private calculateWidth(innerWidth: number): number {
    console.log(innerWidth);
    
    if (innerWidth < 1600) {
      return 400;

    } else if(innerWidth < 1200){
      return 300;
    }else{
      return 600;
    }

}


  loadBusinessCards() {
    this._backend.get("BusinesCards/GetFilteredBusinessCards").then((res) => {
      if (res.ok) {
        this.businessCards = res.body;
      }
    });
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

  exportXml(Id: number) {
    this._backend.ExportXml("BusinesCards/ExportXml", Id).then((res) => {
      if (res.ok) {

      }
    });

  }

  exportCsv(Id: number) {
    this._backend.ExportCsv("BusinesCards/ExportCsv", Id).then((res) => {
      if (res.ok) {

      }
    });

  }

  Search() {

    let dob = this.formFilter.controls.DOB?.value;
    if (dob) {
      var dateTransformed = this.datepipe.transform(dob, 'MM/dd/YYYY');
      this.formFilter.controls.DOB.patchValue(dateTransformed);
    }


    var paramsURL = UrlHelper.toUrlwithParams(
      "BusinesCards/GetFilteredBusinessCards",
      this.formFilter.value
    );

    this._backend.get(paramsURL).then((res) => {
      if (res.ok) {
        this.businessCards = res.body;
      }
    });
  }

}
