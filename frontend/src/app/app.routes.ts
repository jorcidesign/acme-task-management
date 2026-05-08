import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
    {
        path: 'login',
        loadComponent: () =>
            import('./features/login/login.component').then(m => m.LoginComponent),
    },
    {
        path: 'tasks',
        canActivate: [authGuard],
        loadComponent: () =>
            import('./features/tasks/task-board/task-board.component').then(
                m => m.TaskBoardComponent,
            ),
    },
    { path: '', redirectTo: 'tasks', pathMatch: 'full' },
    { path: '**', redirectTo: 'tasks' },
];