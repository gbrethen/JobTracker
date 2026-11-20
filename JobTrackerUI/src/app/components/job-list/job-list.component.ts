import { Component, EventEmitter, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { JobService } from '../../services/job.service';
import { Job } from '../../models/job.model';

@Component({
  selector: 'app-job-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './job-list.component.html',
  styleUrls: ['./job-list.component.css']
})
export class JobListComponent {
  jobService = inject(JobService);

  @Output() openAddJob = new EventEmitter<void>();
  @Output() openEditJob = new EventEmitter<Job>();
  @Output() deleteJobRequested = new EventEmitter<Job>();

  searchTerm = '';

  onSearch() {
    this.jobService.searchQuery.set(this.searchTerm);
    this.jobService.loadJobs({ search: this.searchTerm }).subscribe();
  }

  clearSearch() {
    this.searchTerm = '';
    this.jobService.searchQuery.set('');
    this.jobService.loadJobs().subscribe();
  }

  selectJob(job: Job) {
    this.jobService.selectJob(job.jobId).subscribe();
  }

  onEditJob(event: Event, job: Job) {
    event.stopPropagation();
    this.openEditJob.emit(job);
  }

  onDeleteJob(event: Event, job: Job) {
    event.stopPropagation();
    this.deleteJobRequested.emit(job);
  }

  onAddNoteQuick(event: Event, job: Job) {
    event.stopPropagation();
    this.jobService.selectJob(job.jobId).subscribe();
  }

  getStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'applied': return 'status-applied';
      case 'interviewing': return 'status-interviewing';
      case 'offered': return 'status-offered';
      case 'rejected': return 'status-rejected';
      default: return 'status-default';
    }
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '';
    try {
      const parts = dateStr.split('-');
      if (parts.length === 3) {
        const d = new Date(parseInt(parts[0]), parseInt(parts[1]) - 1, parseInt(parts[2]));
        return d.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
      }
      return dateStr;
    } catch {
      return dateStr;
    }
  }
}
