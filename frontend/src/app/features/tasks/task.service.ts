import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { Task, TaskCreateDto, TaskUpdateDto, PaginatedResponse, TaskStatus } from '../../core/models/task.model';

@Injectable({ providedIn: 'root' })
export class TaskService {
    private readonly http = inject(HttpClient);
    private readonly base = '/api/tasks';

    // Mapeamos directo a la propiedad "items" para mantener simple el tablero por ahora
    getAll() { return this.http.get<PaginatedResponse<Task>>(this.base).pipe(map(res => res.items)); }
    getById(id: number) { return this.http.get<Task>(`${this.base}/${id}`); }
    create(dto: TaskCreateDto) { return this.http.post<Task>(this.base, dto); }
    update(id: number, dto: TaskUpdateDto) { return this.http.put<Task>(`${this.base}/${id}`, dto); }
    updateStatus(id: number, status: TaskStatus) { return this.http.patch<void>(`${this.base}/${id}/status`, { status }); }
    delete(id: number) { return this.http.delete<void>(`${this.base}/${id}`); }
}