import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Job, JobCreateDto, JobFilter, JobLocation, JobNote, JobType, JobUpdateDto } from '../models/job.model';

@Injectable({
  providedIn: 'root'
})
export class JobService {
  private readonly apiUrl = 'https://localhost:44367/api';

  // Reactive state signals for smooth cross-panel coordination
  jobs = signal<Job[]>([]);
  selectedJob = signal<Job | null>(null);
  selectedJobNotes = signal<JobNote[]>([]);
  jobTypes = signal<JobType[]>([]);
  jobLocations = signal<JobLocation[]>([]);
  isLoading = signal<boolean>(false);
  activeFilter = signal<string>('all'); // 'all', 'Full Time', 'Remote', 'Applied', etc.
  searchQuery = signal<string>('');

  constructor(private http: HttpClient) {}

  loadJobTypes(): Observable<JobType[]> {
    return this.http.get<JobType[]>(`${this.apiUrl}/jobtypes`).pipe(
      tap(types => this.jobTypes.set(types))
    );
  }

  loadJobLocations(): Observable<JobLocation[]> {
    return this.http.get<JobLocation[]>(`${this.apiUrl}/joblocations`).pipe(
      tap(locations => this.jobLocations.set(locations))
    );
  }

  loadJobs(filter?: JobFilter): Observable<Job[]> {
    this.isLoading.set(true);
    let params = new HttpParams();

    if (filter) {
      if (filter.search) params = params.set('search', filter.search);
      if (filter.status) params = params.set('status', filter.status);
      if (filter.jobTypeId) params = params.set('jobTypeId', filter.jobTypeId.toString());
      if (filter.jobLocationId) params = params.set('jobLocationId', filter.jobLocationId.toString());
      if (filter.company) params = params.set('company', filter.company);
    }

    return this.http.get<Job[]>(`${this.apiUrl}/jobs`, { params }).pipe(
      tap({
        next: (jobs) => {
          this.jobs.set(jobs);
          this.isLoading.set(false);

          // If a job is already selected, update it with fresh data
          const currentSelected = this.selectedJob();
          if (currentSelected) {
            const found = jobs.find(j => j.jobId === currentSelected.jobId);
            if (found) {
              this.selectJob(found.jobId).subscribe();
            } else {
              this.selectedJob.set(null);
              this.selectedJobNotes.set([]);
            }
          }
        },
        error: () => this.isLoading.set(false)
      })
    );
  }

  selectJob(jobId: number): Observable<Job> {
    return this.http.get<Job>(`${this.apiUrl}/jobs/${jobId}`).pipe(
      tap(job => {
        this.selectedJob.set(job);
        this.selectedJobNotes.set(job.notes || []);
      })
    );
  }

  createJob(dto: JobCreateDto): Observable<Job> {
    return this.http.post<Job>(`${this.apiUrl}/jobs`, dto).pipe(
      tap(newJob => {
        this.loadJobs().subscribe({
          next: () => this.selectJob(newJob.jobId).subscribe()
        });
      })
    );
  }

  updateJob(id: number, dto: JobUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/jobs/${id}`, dto).pipe(
      tap(() => {
        this.loadJobs().subscribe({
          next: () => this.selectJob(id).subscribe()
        });
      })
    );
  }

  updateJobStatus(id: number, status: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/jobs/${id}/status`, { status }).pipe(
      tap(() => {
        this.loadJobs().subscribe({
          next: () => this.selectJob(id).subscribe()
        });
      })
    );
  }

  deleteJob(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/jobs/${id}`).pipe(
      tap(() => {
        if (this.selectedJob()?.jobId === id) {
          this.selectedJob.set(null);
          this.selectedJobNotes.set([]);
        }
        this.loadJobs().subscribe();
      })
    );
  }

  loadNotes(jobId: number): Observable<JobNote[]> {
    return this.http.get<JobNote[]>(`${this.apiUrl}/jobs/${jobId}/notes`).pipe(
      tap(notes => this.selectedJobNotes.set(notes))
    );
  }

  addNote(jobId: number, noteText: string): Observable<JobNote> {
    return this.http.post<JobNote>(`${this.apiUrl}/jobs/${jobId}/notes`, { noteText }).pipe(
      tap(newNote => {
        const notes = [newNote, ...this.selectedJobNotes()];
        this.selectedJobNotes.set(notes);
        // Refresh job notes count
        const job = this.selectedJob();
        if (job) {
          job.notesCount = notes.length;
          this.selectedJob.set({ ...job });
        }
      })
    );
  }

  updateNote(noteId: number, noteText: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/notes/${noteId}`, { noteText }).pipe(
      tap(() => {
        const selected = this.selectedJob();
        if (selected) {
          this.loadNotes(selected.jobId).subscribe();
        }
      })
    );
  }

  deleteNote(noteId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/notes/${noteId}`).pipe(
      tap(() => {
        const notes = this.selectedJobNotes().filter(n => n.jobNoteId !== noteId);
        this.selectedJobNotes.set(notes);
        const job = this.selectedJob();
        if (job) {
          job.notesCount = notes.length;
          this.selectedJob.set({ ...job });
        }
      })
    );
  }
}
