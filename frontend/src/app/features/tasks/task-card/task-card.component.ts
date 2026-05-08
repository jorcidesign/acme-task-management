import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { SlicePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Task, TaskStatus } from '../../../core/models/task.model';
import { STATUS_META } from '../../../shared/constants/task.constants'; // <-- Importación centralizada

@Component({
    selector: 'app-task-card',
    standalone: true,
    imports: [SlicePipe, FormsModule],
    changeDetection: ChangeDetectionStrategy.OnPush,
    template: `
      <div class="group flex items-center gap-4 bg-surface rounded-xl border border-border px-4 py-3.5 hover:border-border-strong hover:shadow-card transition-all duration-150 cursor-pointer"
           (click)="edit.emit()">
        
        <button class="shrink-0 size-5 rounded-full border-2 flex items-center justify-center transition-all"
          [class]="task.status === TaskStatus.Completed ? 'border-emerald-400 bg-emerald-400' : 'border-border-strong hover:border-ink'"
          (click)="onToggleDone($event)">
          @if (task.status === TaskStatus.Completed) {
            <svg class="size-3 text-white" fill="none" stroke="currentColor" stroke-width="3" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7"/></svg>
          }
        </button>

        <div class="flex-1 min-w-0">
          <div class="text-sm text-ink truncate font-medium" 
               [class.line-through]="task.status === TaskStatus.Completed" 
               [class.text-ink-faint]="task.status === TaskStatus.Completed">
            {{ task.title }}
            @if (task.isOverdue && task.status !== TaskStatus.Completed) { <span class="ml-2 text-xs text-red-500 font-medium">Vencida</span> }
          </div>
          
          @if (task.description) {
            <div class="text-xs text-ink-muted truncate mt-0.5"
                 [class.line-through]="task.status === TaskStatus.Completed"
                 [class.opacity-50]="task.status === TaskStatus.Completed">
              {{ task.description }}
            </div>
          }
        </div>

        <div class="relative shrink-0">
          <select 
            [ngModel]="task.status" 
            (ngModelChange)="onStatusChange($event)"
            (click)="$event.stopPropagation()"
            class="appearance-none pl-5 pr-3 py-0.5 rounded-full text-2xs font-medium cursor-pointer outline-none border border-transparent hover:border-border-strong focus:border-accent transition-colors"
            [class]="STATUS_META[task.status].badge">
            <option [value]="TaskStatus.Pending">Por hacer</option>
            <option [value]="TaskStatus.InProgress">En curso</option>
            <option [value]="TaskStatus.Completed">Completada</option>
          </select>
          <span class="size-1.5 rounded-full absolute left-2 top-1/2 -translate-y-1/2 pointer-events-none" [class]="STATUS_META[task.status].dot"></span>
        </div>

        @if (task.dueDate) {
          <span class="text-xs text-ink-faint shrink-0 hidden sm:block w-20 text-right">{{ task.dueDate | slice:0:10 }}</span>
        }

        <div class="opacity-0 group-hover:opacity-100 flex items-center gap-1 shrink-0 transition-opacity">
          <button class="btn-ghost p-1 shrink-0 text-ink-muted hover:text-accent" title="Editar" (click)="onEdit($event)">
            <svg class="size-3.5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="m16.862 4.487 1.687-1.688a1.875 1.875 0 1 1 2.652 2.652L6.832 19.82a4.5 4.5 0 0 1-1.897 1.13l-2.685.8.8-2.685a4.5 4.5 0 0 1 1.13-1.897L16.863 4.487Zm0 0L19.5 7.125"/></svg>
          </button>
          <button class="btn-danger p-1 shrink-0" title="Eliminar" (click)="onDelete($event)">
            <svg class="size-3.5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M19 7l-.867 12.142A2 2 0 0 1 16.138 21H7.862a2 2 0 0 1-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 0 0-1-1h-4a1 1 0 0 0-1 1v3M4 7h16"/></svg>
          </button>
        </div>
      </div>
    `
})
export class TaskCardComponent {
    @Input({ required: true }) task!: Task;
    @Output() statusChange = new EventEmitter<TaskStatus>();
    @Output() edit = new EventEmitter<void>();
    @Output() delete = new EventEmitter<void>();

    readonly STATUS_META = STATUS_META;
    readonly TaskStatus = TaskStatus;

    onToggleDone(event: Event) {
        event.stopPropagation();
        const nextStatus = this.task.status === TaskStatus.Completed ? TaskStatus.Pending : TaskStatus.Completed;
        this.statusChange.emit(nextStatus);
    }

    onStatusChange(newStatus: TaskStatus) {
        if (this.task.status !== newStatus) {
            this.statusChange.emit(newStatus);
        }
    }

    onEdit(event: Event) {
        event.stopPropagation();
        this.edit.emit();
    }

    onDelete(event: Event) {
        event.stopPropagation();
        this.delete.emit();
    }
}