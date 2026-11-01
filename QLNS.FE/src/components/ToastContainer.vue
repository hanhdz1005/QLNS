<script setup lang="ts">
import { useToastStore } from '@/stores/toast';
import { CheckCircle2, AlertCircle, AlertTriangle, Info, X } from 'lucide-vue-next';

const toastStore = useToastStore();

const getIcon = (type: string) => {
  switch (type) {
    case 'success':
      return CheckCircle2;
    case 'error':
      return AlertCircle;
    case 'warning':
      return AlertTriangle;
    default:
      return Info;
  }
};

const getColors = (type: string) => {
  switch (type) {
    case 'success':
      return 'bg-emerald-50 text-emerald-800 border-emerald-300';
    case 'error':
      return 'bg-rose-50 text-rose-800 border-rose-300';
    case 'warning':
      return 'bg-amber-50 text-amber-800 border-amber-300';
    default:
      return 'bg-blue-50 text-blue-800 border-blue-300';
  }
};
</script>

<template>
  <Teleport to="body">
    <div class="fixed top-5 right-5 z-[100] flex flex-col gap-2.5 max-w-sm w-full pointer-events-none">
      <transition-group
        enter-active-class="transform transition ease-out duration-300"
        enter-from-class="translate-y-2 opacity-0 sm:translate-y-0 sm:translate-x-4"
        enter-to-class="translate-y-0 opacity-100 sm:translate-x-0"
        leave-active-class="transition ease-in duration-200"
        leave-from-class="opacity-100"
        leave-to-class="opacity-0 scale-95"
      >
        <div
          v-for="toast in toastStore.toasts"
          :key="toast.id"
          :class="[
            'pointer-events-auto flex items-start gap-3 p-4 rounded-xl border shadow-xl',
            getColors(toast.type),
          ]"
        >
          <component :is="getIcon(toast.type)" class="w-5 h-5 shrink-0 mt-0.5" />
          <div class="flex-1 text-sm font-medium leading-relaxed">
            {{ toast.message }}
          </div>
          <button
            @click="toastStore.remove(toast.id)"
            class="shrink-0 text-slate-400 hover:text-slate-600 transition-colors p-0.5 rounded-lg hover:bg-black/5"
          >
            <X class="w-4 h-4" />
          </button>
        </div>
      </transition-group>
    </div>
  </Teleport>
</template>
