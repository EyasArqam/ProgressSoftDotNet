import { Component, EventEmitter, Output } from '@angular/core';
import { FileUploadModule } from '@iplab/ngx-file-upload';
import { FormsModule } from '@angular/forms';
import {MatIconModule} from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';


@Component({
  standalone: true,
  selector: 'import-file',
  templateUrl: './import-file.component.html',
  styleUrl: './import-file.component.css',
  imports: [
    FormsModule,
    FileUploadModule,
    MatIconModule,
    MatButtonModule,
  ]
})
export class ImportFileComponent {
  @Output() filesSelected = new EventEmitter<File[]>();
  
  public uploadedFiles: Array<File> = [];


  public importData(): void {
    this.filesSelected.emit(this.uploadedFiles);
  }

  public clear(): void {
      this.uploadedFiles = [];
  }

}
