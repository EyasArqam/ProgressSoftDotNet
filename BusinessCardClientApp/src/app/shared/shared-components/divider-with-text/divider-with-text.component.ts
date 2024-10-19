import { Component, Input } from '@angular/core';
import { MatDividerModule } from '@angular/material/divider';

@Component({
  selector: 'divider-with-text',
  templateUrl: './divider-with-text.component.html',
  styleUrl: './divider-with-text.component.css',
  standalone: true,
  imports:[MatDividerModule]
})
export class DividerWithTextComponent {
  @Input() text: string = ''; 

}
