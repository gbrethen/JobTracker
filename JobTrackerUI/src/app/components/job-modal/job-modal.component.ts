import { Component, EventEmitter, Input, OnInit, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { JobService } from '../../services/job.service';
import { Job, JobCreateDto, JobUpdateDto } from '../../models/job.model';

@Component({
  selector: 'app-job-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './job-modal.component.html',
  styleUrls: ['./job-modal.component.css']
})
export class JobModalComponent implements OnInit {
  private fb = inject(FormBuilder);
  jobService = inject(JobService);

  @Input() isEdit = false;
  @Input() jobToEdit: Job | null = null;
  @Output() close = new EventEmitter<void>();

  jobForm!: FormGroup;
  isSubmitting = false;

  statusOptions = ['Applied', 'Interviewing', 'Offered', 'Rejected', 'Withdrawn'];

  ngOnInit() {
    const today = new Date().toISOString().substring(0, 10);

    const initialTypeId = this.jobToEdit?.jobTypeId || (this.jobService.jobTypes().length > 0 ? this.jobService.jobTypes()[0].jobTypeId : null);
    const initialLocationId = this.jobToEdit?.jobLocationId || (this.jobService.jobLocations().length > 0 ? this.jobService.jobLocations()[0].jobLocationId : null);

    this.jobForm = this.fb.group({
      company: [this.jobToEdit?.company || '', [Validators.required, Validators.maxLength(150)]],
      jobTitle: [this.jobToEdit?.jobTitle || '', [Validators.required, Validators.maxLength(150)]],
      jobTypeId: [initialTypeId, [Validators.required]],
      jobLocationId: [initialLocationId, [Validators.required]],
      physicalLocation: [this.jobToEdit?.physicalLocation || '', [Validators.maxLength(150)]],
      applicationDate: [this.jobToEdit?.applicationDate || today, [Validators.required]],
      status: [this.jobToEdit?.status || 'Applied', [Validators.required]],
      salaryRange: [this.jobToEdit?.salaryRange || '', [Validators.maxLength(100)]],
      jobUrl: [this.jobToEdit?.jobUrl || '', [Validators.maxLength(2048)]],
      contactPerson: [this.jobToEdit?.contactPerson || '', [Validators.maxLength(100)]],
      contactEmail: [this.jobToEdit?.contactEmail || '', [Validators.email, Validators.maxLength(256)]],
      initialNote: ['']
    });

    // Populate JobTypes directly from the API
    this.jobService.loadJobTypes().subscribe({
      next: (types) => {
        if (!this.jobToEdit && types.length > 0 && !this.jobForm.get('jobTypeId')?.value) {
          this.jobForm.patchValue({ jobTypeId: types[0].jobTypeId });
        }
      }
    });

    // Populate JobLocations directly from the API
    this.jobService.loadJobLocations().subscribe({
      next: (locations) => {
        if (!this.jobToEdit && locations.length > 0 && !this.jobForm.get('jobLocationId')?.value) {
          this.jobForm.patchValue({ jobLocationId: locations[0].jobLocationId });
        }
      }
    });
  }

  onSubmit() {
    if (this.jobForm.invalid) {
      this.jobForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const val = this.jobForm.value;

    if (this.isEdit && this.jobToEdit) {
      const updateDto: JobUpdateDto = {
        company: val.company.trim(),
        jobTitle: val.jobTitle.trim(),
        jobTypeId: Number(val.jobTypeId),
        jobLocationId: Number(val.jobLocationId),
        physicalLocation: val.physicalLocation?.trim() || undefined,
        applicationDate: val.applicationDate,
        status: val.status,
        salaryRange: val.salaryRange?.trim() || undefined,
        jobUrl: val.jobUrl?.trim() || undefined,
        contactPerson: val.contactPerson?.trim() || undefined,
        contactEmail: val.contactEmail?.trim() || undefined
      };

      this.jobService.updateJob(this.jobToEdit.jobId, updateDto).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.close.emit();
        },
        error: () => this.isSubmitting = false
      });
    } else {
      const createDto: JobCreateDto = {
        company: val.company.trim(),
        jobTitle: val.jobTitle.trim(),
        jobTypeId: Number(val.jobTypeId),
        jobLocationId: Number(val.jobLocationId),
        physicalLocation: val.physicalLocation?.trim() || undefined,
        applicationDate: val.applicationDate,
        status: val.status,
        salaryRange: val.salaryRange?.trim() || undefined,
        jobUrl: val.jobUrl?.trim() || undefined,
        contactPerson: val.contactPerson?.trim() || undefined,
        contactEmail: val.contactEmail?.trim() || undefined,
        initialNote: val.initialNote?.trim() || undefined
      };

      this.jobService.createJob(createDto).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.close.emit();
        },
        error: () => this.isSubmitting = false
      });
    }
  }
}
