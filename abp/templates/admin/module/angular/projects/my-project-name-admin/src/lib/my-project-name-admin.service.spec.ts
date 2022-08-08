import { TestBed } from '@angular/core/testing';

import { MyProjectNameAdminService } from './services/my-project-name-admin.service';

describe('MyProjectNameAdminService', () => {
  let service: MyProjectNameAdminService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MyProjectNameAdminService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
