import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckEligibiltyComponent } from './check-eligibilty.component';

describe('CheckEligibiltyComponent', () => {
  let component: CheckEligibiltyComponent;
  let fixture: ComponentFixture<CheckEligibiltyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CheckEligibiltyComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CheckEligibiltyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
