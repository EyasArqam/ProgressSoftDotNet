import { Component, OnInit, signal } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Gender } from '../../../shared/enums';

@Component({
  selector: 'app-add-business-card',
  templateUrl: './add-business-card.component.html',
  styleUrl: './add-business-card.component.css'
})
export class AddBusinessCardComponent implements OnInit {

  readonly panelOpenState = signal(true);
  businessCardForm: FormGroup = new FormGroup({});
  genders = Object.values(Gender);
  photoBase64: string | null = null;

  ngOnInit(): void {
    this.businessCardForm = new FormGroup({
      Name: new FormControl('', [Validators.required]),
      DateOfBirth: new FormControl(''),
      Email: new FormControl('', [Validators.required, Validators.email]),
      Phone: new FormControl('', [Validators.required]),
      Gender: new FormControl(''),
      Address: new FormControl(''),
      Photo: new FormControl('')
    });

  }

  selectedImage: string | ArrayBuffer | null = '';

  
  onPhotoChange(event: any) {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.photoBase64 = e.target.result;
      };
      reader.readAsDataURL(file);
    }
  }

  onSubmit() {
    if (this.businessCardForm.valid) {

    }
  }

}
