import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { AuthService } from '../../../core/auth/auth.service';
import { TaskService } from '../task.service';
import { Task, TaskStatus, TaskUpdateDto } from '../../../core/models/task.model';
import { TaskCardComponent } from '../task-card/task-card.component';
import { TaskModalComponent } from '../task-modal/task-modal.component';
import { STATUS_META } from '../../../shared/constants/task.constants';
type Filter = 'all' | TaskStatus;

@Component({
    selector: 'app-task-board',
    standalone: true,
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [TaskCardComponent, TaskModalComponent],
    template: `
    <div class="min-h-screen bg-surface-subtle">
      <header class="sticky top-0 z-10 bg-surface/80 backdrop-blur border-b border-border">
        <div class="max-w-5xl mx-auto px-6 h-14 flex items-center justify-between gap-4">
          <div class="flex items-center gap-2">
            <span class="size-6 rounded-md bg-ink flex items-center justify-center">
              <svg class="size-3.5 text-white" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M9 5H7a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V7a2 2 0 0 0-2-2h-2M9 5a2 2 0 0 0 2 2h2a2 2 0 0 0 2-2M9 5a2 2 0 0 1 2-2h2a2 2 0 0 1 2 2"/></svg>
            </span>
            <span class="font-semibold text-ink tracking-tight text-sm">ACME Tasks</span>
          </div>
          <div class="flex items-center gap-2">
            <span class="text-xs text-ink-muted hidden sm:block">{{ auth.user()?.name }}</span>
            <button class="btn-ghost text-xs px-2 py-1.5" (click)="auth.logout()">Salir</button>
          </div>
        </div>
      </header>

      <main class="max-w-5xl mx-auto px-6 py-8 space-y-6">
        <div class="flex items-start justify-between gap-4">
          <div>
            <h1 class="text-2xl font-semibold text-ink tracking-tight">Mis tareas</h1>
            <p class="text-ink-muted text-sm mt-0.5">{{ totalCount() }} tareas · {{ doneCount() }} completadas</p>
          </div>
          <button class="btn-primary shrink-0" (click)="openModal()">
            <svg class="size-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M12 4v16m8-8H4"/></svg>
            Nueva tarea
          </button>
        </div>

        <div class="flex items-center gap-1 border-b border-border pb-1">
          @for (f of filters; track f.value) {
            <button class="px-3 py-1.5 text-sm rounded-md transition-colors duration-150"
              [class]="activeFilter() === f.value ? 'bg-surface-hover text-ink font-medium' : 'text-ink-muted hover:text-ink hover:bg-surface-hover'"
              (click)="activeFilter.set(f.value)">
              {{ f.label }}
            </button>
          }
        </div>

        @if (loading()) {
          <div class="space-y-2">
            @for (s of [1,2,3,4]; track s) { <div class="h-16 bg-surface rounded-xl border border-border animate-pulse"></div> }
          </div>
        }@else {
          <div class="flex flex-col gap-3">
            @for (task of filteredTasks(); track task.id) {
              <app-task-card 
                class="block"
                [task]="task" 
                (statusChange)="changeStatus(task, $event)"
                (edit)="openModal(task)"
                (delete)="requestDelete(task)">
              </app-task-card>
            } @empty {
              <div class="flex flex-col items-center justify-center py-16 text-center">
                <p class="text-sm font-medium text-ink">Sin tareas aquí</p>
                <p class="text-xs text-ink-muted mt-1">Crea una nueva tarea para empezar.</p>
              </div>
            }
          </div>
        }
      </main>

      @if (showModal()) {
        <app-task-modal 
          [task]="taskToEdit()"
          [saving]="saving()"
          (close)="closeModal()"
          (save)="saveTask($event)">
        </app-task-modal>
      }

      @if (taskToDelete(); as taskToDel) {
        <div class="fixed inset-0 z-50 flex items-center justify-center px-4 bg-ink/20 backdrop-blur-sm">
          <div class="w-full max-w-sm bg-surface rounded-2xl border border-border shadow-float p-6 space-y-4">
            <h3 class="font-semibold text-red-600">Eliminar tarea</h3>
            <p class="text-sm text-ink-muted">¿Estás seguro de que deseas eliminar la tarea <strong>"{{ taskToDel.title }}"</strong>? Esta acción no se puede deshacer.</p>
            <div class="flex gap-2 justify-end pt-2">
              <button class="btn-ghost" (click)="taskToDelete.set(null)">Cancelar</button>
              <button class="btn-danger" (click)="executeDelete(taskToDel.id)">Eliminar</button>
            </div>
          </div>
        </div>
      }

    </div>
  `
})
export class TaskBoardComponent implements OnInit {
    readonly auth = inject(AuthService);
    private readonly svc = inject(TaskService);

    readonly tasks = signal<Task[]>([]);
    readonly loading = signal(false);

    // Estados visuales y temporales
    readonly showModal = signal(false);
    readonly saving = signal(false);
    readonly taskToEdit = signal<Task | null>(null);
    readonly taskToDelete = signal<Task | null>(null); // Signal para la confirmación
    readonly activeFilter = signal<Filter>('all');

    readonly filteredTasks = computed(() => {
        const f = this.activeFilter();
        return f === 'all' ? this.tasks() : this.tasks().filter(t => t.status === f);
    });

    readonly totalCount = computed(() => this.tasks().length);
    readonly doneCount = computed(() => this.tasks().filter(t => t.status === TaskStatus.Completed).length);

    readonly filters: { value: Filter; label: string }[] = [
        { value: 'all', label: 'Todo' },
        { value: TaskStatus.Pending, label: 'Por hacer' },
        { value: TaskStatus.InProgress, label: 'En curso' },
        { value: TaskStatus.Completed, label: 'Completadas' },
    ];

    ngOnInit() { this.loadTasks(); }

    loadTasks() {
        this.loading.set(true);
        this.svc.getAll().subscribe({
            next: tasks => { this.tasks.set(tasks); this.loading.set(false); },
            error: () => this.loading.set(false)
        });
    }

    changeStatus(task: Task, newStatus: TaskStatus) {
        this.tasks.update(list => list.map(t => t.id === task.id ? { ...t, status: newStatus } : t));
        this.svc.updateStatus(task.id, newStatus).subscribe({
            error: () => this.tasks.update(list => list.map(t => t.id === task.id ? { ...t, status: task.status } : t))
        });
    }

    // Abre el modal de confirmación
    requestDelete(task: Task) {
        this.taskToDelete.set(task);
    }

    executeDelete(id: number) {
        const taskToDelete = this.taskToDelete();
        if (!taskToDelete) return;

        this.taskToDelete.set(null); // Ocultar el modal inmediatamente

        // 1. Guardar una copia de seguridad de la lista actual
        const currentTasksBackup = this.tasks();

        // 2. Eliminación optimista de la UI
        this.tasks.update(list => list.filter(t => t.id !== id));

        // 3. Llamada al backend
        this.svc.delete(id).subscribe({
            error: () => {
                // 4. ROLLBACK: Restaurar la lista si el backend falla
                this.tasks.set(currentTasksBackup);
                // Aquí podrías agregar un Toast o Alerta de error en un proyecto real
            }
        });
    }

    openModal(task: Task | null = null) {
        this.taskToEdit.set(task);
        this.showModal.set(true);
    }

    closeModal() {
        this.showModal.set(false);
    }

    saveTask(event: { id: number | null, payload: TaskUpdateDto }) {
        this.saving.set(true);
        const request$ = event.id ? this.svc.update(event.id, event.payload) : this.svc.create(event.payload);

        request$.subscribe({
            next: (savedTask) => {
                if (event.id) {
                    this.tasks.update(list => list.map(t => t.id === event.id ? savedTask : t));
                } else {
                    this.tasks.update(list => [savedTask, ...list]);
                }
                this.saving.set(false);
                this.closeModal();
            },
            error: () => this.saving.set(false),
        });
    }
}