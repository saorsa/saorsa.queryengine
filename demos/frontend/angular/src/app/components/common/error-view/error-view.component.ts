import {Component, Input, OnInit} from '@angular/core';

@Component({
  selector: 'app-error-view',
  templateUrl: './error-view.component.html',
  styleUrls: ['./error-view.component.sass']
})
export class ErrorViewComponent implements OnInit {

  @Input() error: any;

  constructor() { }

  ngOnInit(): void {
  }

}
