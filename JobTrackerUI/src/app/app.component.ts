import { Component, OnInit, effect, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { JobListComponent } from './components/job-list/job-list.component';
import { JobNotesComponent } from './components/job-notes/job-notes.component';
import { JobModalComponent } from './components/job-modal/job-modal.component';
import { JobService } from './services/job.service';
import { Job } from './models/job.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    SidebarComponent,
    JobListComponent,
    JobNotesComponent,
    JobModalComponent
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'JobTracker';
  jobService = inject(JobService);

  showModal = false;
  isEditMode = false;
  jobToEdit: Job | null = null;
  showMobileNotes = false;

  constructor() {
    // If user clicks "Add New Job" in the sidebar, open the modal
    effect(() => {
      if (this.jobService.activeFilter() === 'add') {
        this.openAddJobModal();
        this.jobService.activeFilter.set('all');
      }
    });

    // On mobile, if a job is selected, bring notes into view
    effect(() => {
      if (this.jobService.selectedJob()) {
        this.showMobileNotes = true;
      }
    });
  }

  ngOnInit() {
    // Initialize lookups and first load of all jobs
    this.jobService.loadJobTypes().subscribe();
    this.jobService.loadJobLocations().subscribe();
    this.jobService.loadJobs().subscribe();
  }

  openAddJobModal() {
    this.isEditMode = false;
    this.jobToEdit = null;
    this.showModal = true;
  }

  openEditJobModal(job: Job) {
    this.isEditMode = true;
    this.jobToEdit = job;
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
    this.jobToEdit = null;
  }

  closeNotesPanel() {
    this.jobService.selectedJob.set(null);
    this.jobService.selectedJobNotes.set([]);
    this.showMobileNotes = false;
  }
}
