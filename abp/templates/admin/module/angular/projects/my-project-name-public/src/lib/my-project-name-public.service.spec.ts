import { TestBed } from '@angular/core/testing';

import { MyProjectNamePublicService } from './services/my-project-name-public.service';

describe('MyProjectNamePublicService', () => {
  let service: MyProjectNamePublicService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MyProjectNamePublicService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
