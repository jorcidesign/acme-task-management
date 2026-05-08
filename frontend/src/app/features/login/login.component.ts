import {
    ChangeDetectionStrategy,
    Component,
    inject,
    signal,
} from '@angular/core';
import {
    FormBuilder,
    ReactiveFormsModule,
    Validators,
} from '@angular/forms';
import { AuthService } from '../../core/auth/auth.service';

@Component({
    selector: 'app-login',
    standalone: true,
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [ReactiveFormsModule],
    template: `
    <div class="min-h-screen bg-surface-subtle flex items-center justify-center px-4">

      <!-- Card -->
      <div class="w-full max-w-sm bg-surface rounded-xl border border-border shadow-float p-8 space-y-6">

        <!-- Header -->
        <div class="space-y-1">
          <div class="flex items-center gap-2 mb-6">
            <span class="size-7 rounded-lg bg-ink flex items-center justify-center">
              <svg class="size-4 text-white" fill="none" stroke="currentColor" stroke-width="2"
                   viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round"
                      d="M9 5H7a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V7a2 2 0 0 0-2-2h-2
                         M9 5a2 2 0 0 0 2 2h2a2 2 0 0 0 2-2M9 5a2 2 0 0 1 2-2h2a2 2 0 0 1 2 2"/>
              </svg>
            </span>
            <span class="font-semibold text-ink tracking-tight">ACME Tasks</span>
          </div>
          <h1 class="text-xl font-semibold text-ink tracking-tight">Bienvenido de vuelta</h1>
          <p class="text-ink-muted text-sm">Ingresa tus credenciales para continuar.</p>
        </div>

        <!-- Formulario -->
        <form [formGroup]="form" (ngSubmit)="submit()" class="space-y-4" novalidate>

          <!-- Email -->
          <div class="space-y-1">
            <label class="text-xs font-medium text-ink-muted uppercase tracking-wider">
              Correo electrónico
            </label>
            <input type="email" formControlName="email" placeholder="tu@empresa.com" class="field"
       [class.border-red-400]="form.controls.email.invalid && form.controls.email.touched" />
@if (form.controls.email.invalid && form.controls.email.touched) {
  <p class="text-xs text-red-500">Ingresa un correo válido.</p>
}
          </div>

          <!-- Password -->
          <div class="space-y-1">
            <label class="text-xs font-medium text-ink-muted uppercase tracking-wider">
              Contraseña
            </label>
           <input type="password" formControlName="password" placeholder="••••••••" class="field"
       [class.border-red-400]="form.controls.password.invalid && form.controls.password.touched" />
@if (form.controls.password.invalid && form.controls.password.touched) {
  <p class="text-xs text-red-500">Mínimo 8 caracteres requeridos.</p>
}
          </div>

          <!-- Error de API -->
          @if (apiError()) {
            <div class="rounded-lg bg-red-50 border border-red-200 px-3 py-2.5 text-sm text-red-600">
              {{ apiError() }}
            </div>
          }

          <!-- Submit -->
          <button
            type="submit"
            class="btn-primary w-full justify-center py-2 mt-2"
            [disabled]="loading()"
          >
            @if (loading()) {
              <svg class="size-4 animate-spin text-white/70" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/>
                <path class="opacity-75" fill="currentColor"
                      d="M4 12a8 8 0 0 1 8-8V0C5.373 0 0 5.373 0 12h4z"/>
              </svg>
            }
            {{ loading() ? 'Verificando...' : 'Iniciar sesión' }}
          </button>

        </form>

      </div>
    </div>
  `,
})
export class LoginComponent {
    private readonly fb = inject(FormBuilder);
    private readonly auth = inject(AuthService);

    readonly loading = signal(false);
    readonly apiError = signal<string | null>(null);

    form = this.fb.nonNullable.group({
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(8)]],
    });

    // Helpers para mostrar errores solo tras tocar el campo
    // emailInvalid = signal(false);
    // passwordInvalid = signal(false);

    submit() {
        this.form.markAllAsTouched();


        // this.emailInvalid.set(email.invalid);
        // this.passwordInvalid.set(password.invalid);

        if (this.form.invalid) return;

        this.loading.set(true);
        this.apiError.set(null);

        this.auth.login(this.form.controls.email.value, this.form.controls.password.value).subscribe({
            error: err => {
                this.loading.set(false);
                this.apiError.set(
                    err.status === 401
                        ? 'Credenciales incorrectas. Intenta de nuevo.'
                        : 'Ocurrió un error. Por favor intenta más tarde.',
                );
            },
        });
    }
}