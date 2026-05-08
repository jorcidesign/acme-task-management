import { Injectable, computed, effect, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { AuthResponse, User } from '../models/user.model';

const TOKEN_KEY = 'acme_jwt';
const USER_KEY = 'acme_user';
const EXP_KEY = 'acme_exp';

@Injectable({ providedIn: 'root' })
export class AuthService {
    private readonly http = inject(HttpClient);
    private readonly router = inject(Router);

    private readonly _token = signal<string | null>(localStorage.getItem(TOKEN_KEY));
    private readonly _user = signal<User | null>(localStorage.getItem(USER_KEY) ? JSON.parse(localStorage.getItem(USER_KEY)!) : null);

    private expirationTimer: any;

    readonly token = this._token.asReadonly();
    readonly user = this._user.asReadonly();
    readonly isLoggedIn = computed(() => !!this._token());

    constructor() {
        effect(() => {
            const t = this._token();
            const u = this._user();
            if (t) localStorage.setItem(TOKEN_KEY, t); else localStorage.removeItem(TOKEN_KEY);
            if (u) localStorage.setItem(USER_KEY, JSON.stringify(u)); else localStorage.removeItem(USER_KEY);
        });
        this.checkInitialExpiration();
    }

    login(email: string, password: string) {
        return this.http.post<AuthResponse>('/api/auth/login', { email, password }).pipe(
            tap(res => {
                const expDate = new Date().getTime() + res.expiresIn * 1000;
                localStorage.setItem(EXP_KEY, expDate.toString());

                this._token.set(res.accessToken);
                this._user.set({ name: res.fullName, email: res.email });

                this.scheduleLogout(res.expiresIn * 1000);
                this.router.navigate(['/tasks']);
            }),
        );
    }

    logout() {
        clearTimeout(this.expirationTimer);
        localStorage.removeItem(EXP_KEY);
        this._token.set(null);
        this._user.set(null);
        this.router.navigate(['/login']);
    }

    private checkInitialExpiration() {
        const exp = localStorage.getItem(EXP_KEY);
        if (exp) {
            const timeLeft = parseInt(exp, 10) - new Date().getTime();
            if (timeLeft > 0) this.scheduleLogout(timeLeft);
            else this.logout();
        }
    }

    private scheduleLogout(durationMs: number) {
        clearTimeout(this.expirationTimer);
        this.expirationTimer = setTimeout(() => this.logout(), durationMs);
    }
}