import { Component, HostListener, inject, OnInit, signal } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Gender } from '../../../shared/enums';
import { BackendService } from '../../../shared/services/backend.service';
import { SnackbarService } from 'app/shared/services/snackbar.service';

@Component({
  selector: 'app-add-business-card',
  templateUrl: './add-business-card.component.html',
  styleUrl: './add-business-card.component.css'
})
export class AddBusinessCardComponent implements OnInit {
  
  @HostListener('window:resize', ['$event'])
  onResize(event: any) {
    this.defaultWidth = this.calculateWidth(event.target.innerWidth);
  }

  _backend = inject(BackendService);
  _snackbar = inject(SnackbarService);
  readonly panelOpenState = signal(true);
  businessCardForm: FormGroup = new FormGroup({});
  genders = Object.values(Gender);
  photoBase64: string | null = null;
  public selectedFiles: File[] = [];
  defaultWidth =  600;

  
  ngOnInit(): void {
    this.businessCardForm = new FormGroup({
      Name: new FormControl('', [Validators.required]),
      DateOfBirth: new FormControl(null),
      Email: new FormControl('', [Validators.required, Validators.email]),
      Phone: new FormControl('', [Validators.required]),
      Gender: new FormControl('', [Validators.required]),
      Address: new FormControl(''),
      Photo: new FormControl('')
    });

    this.defaultWidth = this.calculateWidth(window.innerWidth);

  }



  private calculateWidth(innerWidth: number): number {
    if (innerWidth > 1200) {
      return 600; 
    } else if (innerWidth > 900) {
      return Math.floor(600 * (innerWidth / 1200));
    } else if (innerWidth > 600) {
      return Math.floor(innerWidth * 0.5); 
    } else {
      return Math.floor(innerWidth * 0.8); 
    }
  }

  onFilesSelected(files: File[]): void {
    this.selectedFiles = files;

    if (!files.length) {
      this._snackbar.show("Please upload at least one file.");
      return;
    }

    if (files.length == 1) {

      const file = files[0];
      const fileExtension = this.getFileExtension(file.name);

      this.processFileByExtension(fileExtension, files);

    }
  }

  onPhotoChange(event: any) {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.photoBase64 = e.target.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  onSubmit() {
    if (this.businessCardForm.valid) {
      const formData: any = {
        Name: this.businessCardForm.get('Name')?.value,
        DateOfBirth: this.businessCardForm.get('DateOfBirth')?.value ?? null, 
        Email: this.businessCardForm.get('Email')?.value,
        Phone: this.businessCardForm.get('Phone')?.value,
        Gender: this.businessCardForm.get('Gender')?.value, 
        Address: this.businessCardForm.get('Address')?.value,
        Photo: this.photoBase64
      };
  
      this._backend.post("BusinesCards/PostForm", formData).then((res) => {
        if (res.ok) {
          this._snackbar.show("Business card has been added successfully");
        }else{
          this._snackbar.show("An error occurred while adding business Card.");
        }
      });
    }
  }
  


  // #region File Handling Methods
  getFileExtension(fileName: string): string | undefined {
    return fileName.split('.').pop()?.toLowerCase();
  }

  processFileByExtension(extension: string | undefined, files: File[]): void {
    if (extension === 'xml') {
      this.postXmlFile(files);
    } else if (extension === 'csv') {
      this.postCsvFile(files);
    } else {
      console.error("Unsupported file type. Please upload an XML or CSV file.");
    }
  }

  postXmlFile(files: File[]): void {
    this._backend.postFiles("BusinesCards/PostXmlFile", files).then((res) => {
      this.handleResponse(res, 'The XML file has been processed successfully.');
    });
  }

  postCsvFile(files: File[]): void {
    this._backend.postFiles("BusinesCards/PostCsvFile", files).then((res) => {
      this.handleResponse(res, 'The CSV file has been processed successfully.');
    });
  }

  handleResponse(res: any, successMessage: string): void {
    if (res.ok) {
      console.log(successMessage);
      this._snackbar.show(successMessage);

    } else {
      console.error("Error processing file:", res.message);
      this._snackbar.show(res.message);

    }
  }
  // #endregion

}
