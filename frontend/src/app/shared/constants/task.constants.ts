import { TaskStatus } from '../../core/models/task.model';

export const STATUS_META: Record<TaskStatus, { label: string; dot: string; badge: string }> = {
    [TaskStatus.Pending]: { label: 'Por hacer', dot: 'bg-gray-400', badge: 'bg-gray-100 text-gray-600' },
    [TaskStatus.InProgress]: { label: 'En curso', dot: 'bg-amber-400', badge: 'bg-amber-50 text-amber-700' },
    [TaskStatus.Completed]: { label: 'Completada', dot: 'bg-emerald-400', badge: 'bg-emerald-50 text-emerald-700' },
};