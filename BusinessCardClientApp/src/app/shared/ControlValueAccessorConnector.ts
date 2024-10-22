import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Directive, Input, forwardRef } from '@angular/core';

@Directive({
    selector: '[appControlValueAccessorConnector]',
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => ControlValueAccessorConnector),
            multi: true
        }
    ]
})
export class ControlValueAccessorConnector implements ControlValueAccessor {
    @Input() value: any;
    onChange = (value: any) => { };
    onTouched = () => { };

    writeValue(value: any): void {
        this.value = value;
        this.onChange(this.value);
    }

    registerOnChange(fn: any): void {
        this.onChange = fn;
    }

    registerOnTouched(fn: any): void {
        this.onTouched = fn;
    }

    setDisabledState?(isDisabled: boolean): void {
    }
}
