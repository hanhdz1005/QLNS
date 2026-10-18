import { ref, watch, onUnmounted } from 'vue';

export function useDebounceSearch(onSearch: (query: string) => void, delay = 350) {
  const searchQuery = ref('');
  let timeoutId: ReturnType<typeof setTimeout> | null = null;

  watch(searchQuery, (newVal) => {
    if (timeoutId) {
      clearTimeout(timeoutId);
    }
    timeoutId = setTimeout(() => {
      onSearch(newVal.trim());
    }, delay);
  });

  onUnmounted(() => {
    if (timeoutId) {
      clearTimeout(timeoutId);
    }
  });

  const clearSearch = () => {
    searchQuery.value = '';
    onSearch('');
  };

  return {
    searchQuery,
    clearSearch,
  };
}
