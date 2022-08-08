import { Component, OnInit } from '@angular/core';
import { MyProjectNamePublicService } from '../services/my-project-name-public.service';

@Component({
  selector: 'lib-my-project-name-public',
  template: ` <p>my-project-name-public works!</p> `,
  styles: [],
})
export class MyProjectNamePublicComponent implements OnInit {
  constructor(private service: MyProjectNamePublicService) { }

  ngOnInit(): void {
    this.service.sample().subscribe(console.log);
  }
}
