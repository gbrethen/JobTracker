export interface Job {
  jobId: number;
  company: string;
  jobTitle: string;
  jobTypeId: number;
  jobTypeName: string;
  jobLocationId: number;
  jobLocationType: string;
  physicalLocation?: string;
  applicationDate: string;
  status: string;
  salaryRange?: string;
  jobUrl?: string;
  contactPerson?: string;
  contactEmail?: string;
  createdAt: string;
  updatedAt: string;
  notesCount: number;
  notes?: JobNote[];
}

export interface JobNote {
  jobNoteId: number;
  jobId: number;
  noteText: string;
  createdAt: string;
}

export interface JobType {
  jobTypeId: number;
  typeName: string;
}

export interface JobLocation {
  jobLocationId: number;
  locationType: string;
}

export interface JobCreateDto {
  company: string;
  jobTitle: string;
  jobTypeId: number;
  jobLocationId: number;
  physicalLocation?: string;
  applicationDate?: string;
  status?: string;
  salaryRange?: string;
  jobUrl?: string;
  contactPerson?: string;
  contactEmail?: string;
  initialNote?: string;
}

export interface JobUpdateDto {
  company: string;
  jobTitle: string;
  jobTypeId: number;
  jobLocationId: number;
  physicalLocation?: string;
  applicationDate: string;
  status: string;
  salaryRange?: string;
  jobUrl?: string;
  contactPerson?: string;
  contactEmail?: string;
}

export interface JobFilter {
  search?: string;
  status?: string;
  jobTypeId?: number;
  jobLocationId?: number;
  company?: string;
}
