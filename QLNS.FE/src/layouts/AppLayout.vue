<script setup lang="ts">
import { ref } from 'vue';
import ToastContainer from '@/components/ToastContainer.vue';
import AppSidebar from '@/components/layout/AppSidebar.vue';
import AppNavbar from '@/components/layout/AppNavbar.vue';

const isSidebarOpen = ref(true);
const isMobileSidebarOpen = ref(false);

const toggleSidebar = () => {
  isSidebarOpen.value = !isSidebarOpen.value;
};

const openMobileSidebar = () => {
  isMobileSidebarOpen.value = true;
};

const closeMobileSidebar = () => {
  isMobileSidebarOpen.value = false;
};
</script>

<template>
  <div class="h-screen bg-slate-50 flex flex-col font-sans text-slate-800 overflow-hidden">
    <ToastContainer />

    <!-- Mobile sidebar backdrop -->
    <div
      v-if="isMobileSidebarOpen"
      class="fixed inset-0 z-40 bg-slate-900/60 backdrop-blur-xs lg:hidden"
      @click="closeMobileSidebar"
    />

    <div class="flex flex-1 min-h-0 overflow-hidden">
      <!-- Modular Sidebar -->
      <AppSidebar
        :is-open="isSidebarOpen"
        :is-mobile-open="isMobileSidebarOpen"
        @close-mobile="closeMobileSidebar"
      />

      <!-- Main Content Area -->
      <div class="flex-1 flex flex-col min-w-0 min-h-0 overflow-hidden">
        <!-- Modular Navbar -->
        <AppNavbar
          @toggle-sidebar="toggleSidebar"
          @open-mobile-sidebar="openMobileSidebar"
        />

        <!-- Page View Container -->
        <main class="flex-1 min-h-0 overflow-y-auto p-4 sm:p-5 lg:p-6">
          <router-view />
        </main>
      </div>
    </div>
  </div>
</template>
