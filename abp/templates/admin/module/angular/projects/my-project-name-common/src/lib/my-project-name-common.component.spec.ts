import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { MyProjectNameCommonComponent } from './components/my-project-name-common.component';

describe('MyProjectNameCommonComponent', () => {
  let component: MyProjectNameCommonComponent;
  let fixture: ComponentFixture<MyProjectNameCommonComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [MyProjectNameCommonComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MyProjectNameCommonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
