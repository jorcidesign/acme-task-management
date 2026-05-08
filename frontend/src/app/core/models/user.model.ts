export interface AuthResponse {
    accessToken: string;
    tokenType: string;
    expiresIn: number;
    fullName: string;
    email: string;
}

export interface User {
    name: string;
    email: string;
}