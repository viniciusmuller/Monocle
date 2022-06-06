import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

interface ScreenshotDialogData {
  imagePath: string;
}

@Component({
  selector: 'app-screenshot-dialog',
  templateUrl: './screenshot-dialog.component.html',
  styleUrls: ['./screenshot-dialog.component.scss']
})
export class ScreenshotDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<ScreenshotDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ScreenshotDialogData
  ) { }

  close() {
    this.dialogRef.close();
  }
}
