import { ChangeDetectionStrategy, Component, EventEmitter, Output, input, effect } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Task, TaskStatus, TaskUpdateDto } from '../../../core/models/task.model';

@Component({
    selector: 'app-task-modal',
    standalone: true,
    imports: [FormsModule],
    changeDetection: ChangeDetectionStrategy.OnPush,
    template: `
      <div class="fixed inset-0 z-50 flex items-center justify-center px-4 bg-ink/20 backdrop-blur-sm" (click)="close.emit()">
        <div class="w-full max-w-md bg-surface rounded-2xl border border-border shadow-float p-6 space-y-5" (click)="$event.stopPropagation()">
          
          <div class="flex items-center justify-between">
            <h2 class="font-semibold text-ink">{{ isEdit ? 'Editar tarea' : 'Nueva tarea' }}</h2>
            <button class="btn-ghost p-1.5 rounded-lg" (click)="close.emit()">
              <svg class="size-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M6 18 18 6M6 6l12 12"/></svg>
            </button>
          </div>

          <div class="space-y-3">
            <input type="text" [(ngModel)]="form.title" placeholder="Título de la tarea" class="field" maxlength="255" />
            <textarea [(ngModel)]="form.description" placeholder="Descripción (opcional)" rows="3" class="field resize-none"></textarea>
            
            <div class="grid grid-cols-2 gap-3">
              @if (isEdit) {
                <div class="space-y-1">
                  <label class="text-xs text-ink-muted font-medium uppercase tracking-wider">Estado</label>
                  <select [(ngModel)]="form.status" class="field">
                    <option [value]="TaskStatus.Pending">Por hacer</option>
                    <option [value]="TaskStatus.InProgress">En curso</option>
                    <option [value]="TaskStatus.Completed">Completada</option>
                  </select>
                </div>
              }
              <div class="space-y-1" [class.col-span-2]="!isEdit">
                <label class="text-xs text-ink-muted font-medium uppercase tracking-wider">Fecha límite</label>
                <input type="date" [(ngModel)]="form.dueDate" class="field" />
              </div>
            </div>
          </div>

          <div class="flex gap-2 justify-end pt-1">
            <button class="btn-ghost" (click)="close.emit()">Cancelar</button>
            <button class="btn-primary" [disabled]="saving() || !form.title.trim()" (click)="submit()">
              {{ saving() ? 'Guardando...' : (isEdit ? 'Guardar cambios' : 'Crear tarea') }}
            </button>
          </div>

        </div>
      </div>
    `
})
export class TaskModalComponent {
    task = input<Task | null>(null);
    saving = input(false);

    @Output() close = new EventEmitter<void>();
    @Output() save = new EventEmitter<{ id: number | null, payload: TaskUpdateDto }>();

    readonly TaskStatus = TaskStatus;
    isEdit = false;
    form: TaskUpdateDto = { title: '', description: '', status: TaskStatus.Pending, dueDate: '' };

    constructor() {
        effect(() => {
            const currentTask = this.task();
            this.isEdit = !!currentTask;
            if (currentTask) {
                this.form = {
                    title: currentTask.title,
                    description: currentTask.description,
                    status: currentTask.status,
                    dueDate: currentTask.dueDate ? currentTask.dueDate.substring(0, 10) : ''
                };
            } else {
                this.form = { title: '', description: '', status: TaskStatus.Pending, dueDate: '' };
            }
        });
    }

    submit() {
        if (!this.form.title.trim()) return;
        const currentTask = this.task();
        this.save.emit({
            id: currentTask ? currentTask.id : null,
            payload: {
                ...this.form,
                dueDate: this.form.dueDate ? new Date(this.form.dueDate).toISOString() : null
            }
        });
    }
}