import { Component, OnInit } from '@angular/core';
import { ActivationEnd, Router } from "@angular/router";


@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.sass']
})
export class LayoutComponent implements OnInit {

  constructor(
    private router: Router,
  ) { }

  ngOnInit(): void {
    this.router.events.subscribe(event => {
      if (event instanceof ActivationEnd) {
      }
    })
  }
}
