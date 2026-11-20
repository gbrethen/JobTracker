import { Component, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { JobService } from '../../services/job.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent {
  jobService = inject(JobService);

  // Dynamic counts for navigation badges
  allJobsCount = computed(() => this.jobService.jobs().length);

  statusCounts = computed(() => {
    const jobs = this.jobService.jobs();
    const map: Record<string, number> = {};
    for (const j of jobs) {
      map[j.status] = (map[j.status] || 0) + 1;
    }
    return map;
  });

  locationCounts = computed(() => {
    const jobs = this.jobService.jobs();
    const map: Record<string, number> = {};
    for (const j of jobs) {
      map[j.jobLocationType] = (map[j.jobLocationType] || 0) + 1;
    }
    return map;
  });

  typeCounts = computed(() => {
    const jobs = this.jobService.jobs();
    const map: Record<string, number> = {};
    for (const j of jobs) {
      map[j.jobTypeName] = (map[j.jobTypeName] || 0) + 1;
    }
    return map;
  });

  selectFilter(filterKey: string, filterType?: 'status' | 'location' | 'type') {
    this.jobService.activeFilter.set(filterKey);

    if (filterKey === 'all') {
      this.jobService.loadJobs().subscribe();
    } else if (filterType === 'status') {
      this.jobService.loadJobs({ status: filterKey }).subscribe();
    } else if (filterType === 'location') {
      const loc = this.jobService.jobLocations().find(l => l.locationType === filterKey);
      if (loc) {
        this.jobService.loadJobs({ jobLocationId: loc.jobLocationId }).subscribe();
      }
    } else if (filterType === 'type') {
      const type = this.jobService.jobTypes().find(t => t.typeName === filterKey);
      if (type) {
        this.jobService.loadJobs({ jobTypeId: type.jobTypeId }).subscribe();
      }
    }
  }
}
