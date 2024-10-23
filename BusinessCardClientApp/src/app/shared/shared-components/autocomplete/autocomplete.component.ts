import { Component, forwardRef, Input } from '@angular/core';
import { FormControl, FormsModule, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, startWith, switchMap } from 'rxjs/operators';
import { AsyncPipe, CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { ControlValueAccessorConnector } from '../../ControlValueAccessorConnector';
import { BackendService } from '../../services/backend.service';

@Component({
  standalone: true,
  selector: 'autocomplete',
  templateUrl: './autocomplete.component.html',
  styleUrl: './autocomplete.component.css',
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatAutocompleteModule,
    ReactiveFormsModule,
    AsyncPipe,
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AutocompleteComponent),
      multi: true
    }
  ]
})
export class AutocompleteComponent extends ControlValueAccessorConnector {
  @Input() data: string[] = [];
  @Input() url: string = '';
  @Input() placeholder: string = '';
  filteredOptions!: Observable<any[]>;
  formControl = new FormControl('');

  constructor(private backendService: BackendService) {
    super();
  }

  ngOnInit() {
    this.filteredOptions = this.formControl.valueChanges.pipe(
      startWith(''),
      debounceTime(400),
      distinctUntilChanged(),
      switchMap(value => {
        if (this.url) {
          return this.backendService.getObservable(this.url + `?searchTerm=${value}`).pipe(
            map(options => this._filter(options, value || ''))
          );
        } else {
          return of(this._filter(this.data, value || ''));
        }
      })
    );
  }

  private _filter(options: string[], value: string): string[] {
    const filterValue = value.toLowerCase();
    return options?.filter(option => option.toLowerCase().includes(filterValue));
  }
}
