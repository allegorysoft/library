import { Component, OnInit } from '@angular/core';
import { MyProjectNameAdminService } from '../services/my-project-name-admin.service';

@Component({
  selector: 'lib-my-project-name-admin',
  template: ` <p>my-project-name-admin works!</p> `,
  styles: [],
})
export class MyProjectNameAdminComponent implements OnInit {
  constructor(private service: MyProjectNameAdminService) { }

  ngOnInit(): void {
    this.service.sample().subscribe(console.log);
  }
}
