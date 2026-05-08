export enum TaskStatus {
    Pending = 'Pending',
    InProgress = 'InProgress',
    Completed = 'Completed'
}

export interface Task {
    id: number;
    title: string;
    description: string | null;
    status: TaskStatus;
    statusLabel: string;
    dueDate: string | null;
    isOverdue: boolean;
    createdAt: string;
    updatedAt: string;
}

export interface PaginatedResponse<T> {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
}

export interface TaskCreateDto {
    title: string;
    description?: string | null;
    dueDate?: string | null;
}

export interface TaskUpdateDto extends TaskCreateDto {
    status: TaskStatus;
}