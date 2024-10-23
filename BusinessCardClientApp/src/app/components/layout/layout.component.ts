import { ChangeDetectorRef, Component, inject, Inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MediaMatcher } from '@angular/cdk/layout';
import { Router } from '@angular/router';
import { LoaderService } from 'app/shared/services/loader.service';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css',
})
export class LayoutComponent {

  router = inject(Router);
  loader = inject(LoaderService);

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


  navigateToHome() {
    this.router.navigate(['/home']);
  }

}
