import { Component, OnInit } from '@angular/core';
import { DataGeneratorService } from "../../services/data-generator.service";
import {catchError, of} from "rxjs";

@Component({
  selector: 'app-data-generator',
  templateUrl: './data-generator.component.html',
  styleUrls: ['./data-generator.component.sass']
})
export class DataGeneratorComponent implements OnInit {

  loading = false;
  error: any = null;
  result: any = null;

  constructor(
    readonly dataGenerator: DataGeneratorService,
  ) { }

  ngOnInit(): void {
  }

  generateData(): void {
    this.error = null;
    this.loading = true;
    this.result = null;
    this.dataGenerator.fillData()
      .pipe(catchError(err => {
        this.error = err;
        return of(null)
      }))
      .subscribe(result => {
        this.result = result;
        this.loading = false;
      })
  }
}
