import { Component, HostListener, OnInit } from '@angular/core';
import { BusinessCard } from '../../../data/models/BusinessCard';

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



  ngOnInit(): void {
    this.updateCols(window.innerWidth);


    this.businessCards.push(new BusinessCard(
      'John Doe',
      1, 
      new Date('1990-01-01'),
      'john@example.com',
      '123-456-7890',
      '123 Main St, New York, NY',
    ));

    this.businessCards.push(new BusinessCard(
      'Jane Smith',
      2, 
      new Date('1992-05-15'),
      'jane@example.com',
      '987-654-3210',
      '456 Elm St, Los Angeles, CA',
    ));
    this.businessCards.push(new BusinessCard(
      'Jane Smith',
      2, 
      new Date('1992-05-15'),
      'jane@example.com',
      '987-654-3210',
      '456 Elm St, Los Angeles, CA',
    ));
    this.businessCards.push(new BusinessCard(
      'Jane Smith',
      2, 
      new Date('1992-05-15'),
      'jane@example.com',
      '987-654-3210',
      '456 Elm St, Los Angeles, CA',
    ));
    this.businessCards.push(new BusinessCard(
      'Jane Smith',
      2, 
      new Date('1992-05-15'),
      'jane@example.com',
      '987-654-3210',
      '456 Elm St, Los Angeles, CA',
    ));
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
}
