import { Component, HostListener, inject, OnInit } from '@angular/core';
import { BusinessCard } from '../../../data/models/BusinessCard';
import { BackendService } from '../../../shared/services/backend.service';
import { FormControl, FormGroup } from '@angular/forms';
import { UrlHelper } from '@utils/url-helper';
import { Gender } from 'app/shared/enums';
import { DatePipe } from '@angular/common';
import { SnackbarService } from 'app/shared/services/snackbar.service';

@Component({
  selector: 'app-list-business-cards',
  templateUrl: './list-business-cards.component.html',
  styleUrl: './list-business-cards.component.css'
})
export class ListBusinessCardsComponent implements OnInit {
  constructor(public datepipe: DatePipe) { }
  _snackbar = inject(SnackbarService);


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

  ngOnInit(): void {
    this.loadBusinessCards();


  }


  loadBusinessCards() {
    this._backend.get("BusinesCards/GetFilteredBusinessCards").then((res) => {
      if (res.ok) {
        this.businessCards = res.body;
      }else{
        this._snackbar.show("No data available.");
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
        this._snackbar.show("Business card deleted successfully.");
        this.loadBusinessCards();
      }else{
        this._snackbar.show("Failed to delete the business card. Please try again.");
      }
    });

  }

  exportXml(Id: number) {
    this._backend.ExportXml("BusinesCards/ExportXml", Id).then((res) => {
      if (res.ok) {
        this._snackbar.show("Business card exported successfully.");
      }else{
        this._snackbar.show("Failed to export the business card. Please try again.");
      }
    });

  }

  exportCsv(Id: number) {
    this._backend.ExportCsv("BusinesCards/ExportCsv", Id).then((res) => {
      if (res.ok) {
        this._snackbar.show("Business card exported successfully.");
      }else{
        this._snackbar.show("Failed to export the business card. Please try again.");
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
