import { ChangeDetectorRef, Component, Inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MediaMatcher } from '@angular/cdk/layout';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css',
})
export class LayoutComponent {

  protected mobileQuery: MediaQueryList;
  protected _mobileQueryListener: () => void;

  constructor(
    private readonly changeDetectorRef: ChangeDetectorRef,
    @Inject(MediaMatcher) private readonly media: MediaMatcher,
    protected readonly dialog: MatDialog,

  ) {
    this.mobileQuery = this.media.matchMedia('(max-width: 600px)');
    this._mobileQueryListener = () => this.changeDetectorRef.detectChanges();
    this.mobileQuery.addListener(this._mobileQueryListener);
  }

}
