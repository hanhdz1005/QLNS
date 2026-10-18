<script setup lang="ts">
import { X } from 'lucide-vue-next';

withDefaults(
  defineProps<{
    modelValue: boolean;
    title?: string;
    maxWidth?: 'sm' | 'md' | 'lg' | 'xl' | '2xl' | '3xl' | '4xl';
  }>(),
  {
    title: '',
    maxWidth: 'lg',
  }
);

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void;
  (e: 'close'): void;
}>();

const close = () => {
  emit('update:modelValue', false);
  emit('close');
};

const maxWidthClasses = {
  sm: 'max-w-sm',
  md: 'max-w-md',
  lg: 'max-w-lg',
  xl: 'max-w-xl',
  '2xl': 'max-w-2xl',
  '3xl': 'max-w-3xl',
  '4xl': 'max-w-4xl',
};
</script>

<template>
  <teleport to="body">
    <transition
      enter-active-class="transition duration-200 ease-out"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition duration-150 ease-in"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="modelValue"
        class="fixed inset-0 z-50 overflow-y-auto bg-slate-900/50 backdrop-blur-xs flex items-center justify-center p-4 sm:p-6"
        @click.self="close"
      >
        <div
          :class="[
            'w-full bg-white rounded-2xl shadow-2xl border border-slate-100 transform transition-all overflow-hidden flex flex-col max-h-[90vh]',
            maxWidthClasses[maxWidth],
          ]"
        >
          <!-- Modal Header -->
          <div class="px-6 py-4 border-b border-slate-100 flex items-center justify-between shrink-0 bg-slate-50/50">
            <h3 class="text-base font-semibold text-slate-800">
              <slot name="title">{{ title }}</slot>
            </h3>
            <button
              @click="close"
              class="text-slate-400 hover:text-slate-600 transition-colors p-1.5 rounded-lg hover:bg-slate-100"
            >
              <X class="w-5 h-5" />
            </button>
          </div>

          <!-- Modal Body -->
          <div class="px-6 py-5 overflow-y-auto flex-1">
            <slot />
          </div>

          <!-- Modal Footer -->
          <div
            v-if="$slots.footer"
            class="px-6 py-4 bg-slate-50 border-t border-slate-100 flex items-center justify-end gap-3 shrink-0"
          >
            <slot name="footer" />
          </div>
        </div>
      </div>
    </transition>
  </teleport>
</template>
