import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from "@angular/material/dialog";
import { ThemePalette } from "@angular/material/core";

@Component({
  selector: 'app-inspect-view',
  templateUrl: './inspect-view.component.html',
  styleUrls: ['./inspect-view.component.sass']
})
export class InspectViewComponent implements OnInit {

  get color(): ThemePalette {
    return this.data?.color ?? 'primary';
  }

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: {
      dataItem: any,
      color?: ThemePalette,
    }) { }

  ngOnInit(): void {
  }
}
