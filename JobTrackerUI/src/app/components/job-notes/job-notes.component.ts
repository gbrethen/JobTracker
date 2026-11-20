import { Component, EventEmitter, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { JobService } from '../../services/job.service';
import { Job, JobNote } from '../../models/job.model';

@Component({
  selector: 'app-job-notes',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './job-notes.component.html',
  styleUrls: ['./job-notes.component.css']
})
export class JobNotesComponent {
  jobService = inject(JobService);

  @Output() openEditJob = new EventEmitter<Job>();
  @Output() closePanel = new EventEmitter<void>();
  @Output() deleteJobRequested = new EventEmitter<Job>();

  newNoteText = '';
  editingNoteId: number | null = null;
  editingNoteText = '';

  // Palette of circular timeline dot colors matching Screenshot 2
  private dotColors = ['#5eead4', '#fca5a5', '#c4b5fd', '#fde047', '#93c5fd', '#f472b6'];

  getDotColor(index: number): string {
    return this.dotColors[index % this.dotColors.length];
  }

  onAddNote() {
    const text = this.newNoteText.trim();
    const job = this.jobService.selectedJob();
    if (!text || !job) return;

    this.jobService.addNote(job.jobId, text).subscribe({
      next: () => {
        this.newNoteText = '';
      }
    });
  }

  startEditNote(note: JobNote) {
    this.editingNoteId = note.jobNoteId;
    this.editingNoteText = note.noteText;
  }

  cancelEditNote() {
    this.editingNoteId = null;
    this.editingNoteText = '';
  }

  saveEditNote(note: JobNote) {
    const text = this.editingNoteText.trim();
    if (!text) return;

    this.jobService.updateNote(note.jobNoteId, text).subscribe({
      next: () => {
        this.editingNoteId = null;
        this.editingNoteText = '';
      }
    });
  }

  deleteNote(noteId: number) {
    if (confirm('Are you sure you want to delete this note?')) {
      this.jobService.deleteNote(noteId).subscribe();
    }
  }

  deleteSelectedJob() {
    const job = this.jobService.selectedJob();
    if (!job) return;

    this.deleteJobRequested.emit(job);
  }

  formatDateTime(isoStr: string): string {
    if (!isoStr) return '';
    try {
      const d = new Date(isoStr);
      return d.toLocaleDateString('en-US', {
        month: 'short',
        day: 'numeric',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      });
    } catch {
      return isoStr;
    }
  }
}
