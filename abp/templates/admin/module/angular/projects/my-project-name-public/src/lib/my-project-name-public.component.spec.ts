import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { MyProjectNamePublicComponent } from './components/my-project-name-public.component';

describe('MyProjectNamePublicComponent', () => {
  let component: MyProjectNamePublicComponent;
  let fixture: ComponentFixture<MyProjectNamePublicComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [MyProjectNamePublicComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MyProjectNamePublicComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
