import { Component, OnInit } from '@angular/core';
import { MyProjectNameCommonService } from '../services/my-project-name-common.service';

@Component({
  selector: 'lib-my-project-name-common',
  template: ` <p>my-project-name-common works!</p> `,
  styles: [],
})
export class MyProjectNameCommonComponent implements OnInit {
  constructor(private service: MyProjectNameCommonService) { }

  ngOnInit(): void {
    this.service.sample().subscribe(console.log);
  }
}
