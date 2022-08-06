import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { MyProjectNameAdminComponent } from './components/my-project-name-admin.component';

describe('MyProjectNameAdminComponent', () => {
  let component: MyProjectNameAdminComponent;
  let fixture: ComponentFixture<MyProjectNameAdminComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [MyProjectNameAdminComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MyProjectNameAdminComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
