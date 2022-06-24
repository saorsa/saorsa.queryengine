import { Component, Input, OnInit } from '@angular/core';
import { ThemePalette } from "@angular/material/core";
import { ButtonType } from "../button-type.type";
import {MatDialog, MatDialogRef} from "@angular/material/dialog";
import {InspectViewComponent} from "../inspect-view/inspect-view.component";


@Component({
  selector: 'app-inspect-button',
  templateUrl: './inspect-button.component.html',
  styleUrls: ['./inspect-button.component.sass']
})
export class InspectButtonComponent implements OnInit {

  @Input() color: ThemePalette = 'primary';
  @Input() data: any = null;
  @Input() buttonText?: string | null;
  @Input() buttonType: ButtonType = 'mini-fab';
  @Input() buttonNgClass: string | string[] | Set<string> | {
    [klass: string]: any;
  } = {};
  @Input() icon: string = 'adb';
  @Input() tooltip: string = 'Inspect debug context';

  protected dialogRef?: MatDialogRef<any>;

  constructor(public dialog: MatDialog) {}

  ngOnInit(): void {
  }

  openDebugDialog(): void {
    this.dialogRef = this.dialog.open(InspectViewComponent, {
      data: {
        dataItem: this.data,
        color: this.color,
      },
      panelClass: 'p-0'
    });
  }
}
