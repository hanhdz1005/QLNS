import { defineStore } from 'pinia';
import { ref } from 'vue';

export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface ToastItem {
  id: string;
  type: ToastType;
  message: string;
  duration?: number;
}

export const useToastStore = defineStore('toast', () => {
  const toasts = ref<ToastItem[]>([]);

  const add = (message: string, type: ToastType = 'info', duration = 3500) => {
    const id = `${Date.now()}_${Math.random()}`;
    const item: ToastItem = { id, type, message, duration };
    toasts.value.push(item);

    if (duration > 0) {
      setTimeout(() => {
        remove(id);
      }, duration);
    }
  };

  const remove = (id: string) => {
    toasts.value = toasts.value.filter((t) => t.id !== id);
  };

  const success = (msg: string) => add(msg, 'success');
  const error = (msg: string) => add(msg, 'error', 4500);
  const warning = (msg: string) => add(msg, 'warning');
  const info = (msg: string) => add(msg, 'info');

  return {
    toasts,
    add,
    remove,
    success,
    error,
    warning,
    info,
  };
});
